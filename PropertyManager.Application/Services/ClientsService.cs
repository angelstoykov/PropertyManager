using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PropertyManager.Application.Configuration;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Properties;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.Services
{
    public class ClientsService : IClientsService
    {
        private static readonly Regex IbanPattern = new(
            "^[A-Z]{2}[0-9]{2}[A-Z0-9]{11,30}$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private readonly PropertyManagerDbContext _context;
        private readonly HashSet<ClientSearchField> _searchFields;

        public ClientsService(PropertyManagerDbContext context, IOptions<ClientSearchOptions> searchOptions)
        {
            _context = context;
            _searchFields = ParseSearchFields(searchOptions.Value.Fields);
        }

        public async Task<IReadOnlyList<ClientListItemDto>> GetAllAsync(string? search = null)
        {
            var clients = await _context.Clients
                .AsNoTracking()
                .OrderByDescending(c => c.IsActive)
                .ThenBy(c => c.ClientType)
                .ThenBy(c => c.CompanyName)
                .ThenBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .Select(c => new ClientListItemDto
                {
                    Id = c.Id,
                    ClientType = c.ClientType,
                    DisplayName = c.ClientType == ClientType.Company
                        ? c.CompanyName!
                        : c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    IsActive = c.IsActive,
                    HasLinkedRecords = c.ClientUnits.Any()
                })
                .ToListAsync();

            if (string.IsNullOrWhiteSpace(search) || _searchFields.Count == 0)
                return clients;

            var term = search.Trim();
            return clients.Where(c => MatchesSearch(c, term)).ToList();
        }

        public async Task<ClientDto?> GetByIdAsync(int id)
        {
            var client = await _context.Clients
                .AsNoTracking()
                .Include(c => c.ClientUnits)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return null;

            return MapToDto(client);
        }

        public async Task<int> CreateAsync(CreateClientDto dto)
        {
            var client = new Client { IsActive = true };
            ApplyClient(client, dto.ClientType, dto.Email, dto.Phone, dto.FirstName, dto.LastName,
                dto.PersonalId, dto.DateOfBirth, dto.CompanyName, dto.CompanyNumber, dto.VatNumber,
                dto.LegalRepresentative, dto.Address, dto.BankAccount, dto.Notes);
            await EnsureIdentifierIsUniqueAsync(null, client.PersonalId, client.CompanyNumber);

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return client.Id;
        }

        public async Task EditAsync(EditClientDto dto)
        {
            var client = await _context.Clients.FindAsync(dto.Id);
            if (client == null)
                throw new InvalidOperationException("Client not found.");

            ApplyClient(client, dto.ClientType, dto.Email, dto.Phone, dto.FirstName, dto.LastName,
                dto.PersonalId, dto.DateOfBirth, dto.CompanyName, dto.CompanyNumber, dto.VatNumber,
                dto.LegalRepresentative, dto.Address, dto.BankAccount, dto.Notes);
            client.IsActive = dto.IsActive;
            await EnsureIdentifierIsUniqueAsync(client.Id, client.PersonalId, client.CompanyNumber);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _context.Clients
                .Include(c => c.ClientUnits)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return;

            if (client.ClientUnits.Count > 0)
                throw new InvalidOperationException(
                    "This client is linked to records and can only be deactivated.");

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                throw new InvalidOperationException("Client not found.");

            client.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<ClientRentedUnitDto>> GetRentedUnitsAsync(int clientId)
        {
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
                throw new InvalidOperationException("Client not found.");

            return await _context.Clients
                .AsNoTracking()
                .Where(c => c.Id == clientId)
                .SelectMany(c => c.ClientUnits.Select(cu => cu.Unit))
                .Select(p => new ClientRentedUnitDto
                {
                    UnitId = p.Id,
                    Name = p.Property.Name,
                    Address = p.Property.Address,
                    UnitNumber = p.UnitNumber
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<PropertyListItemDto>> GetAvailablePropertiesAsync()
        {
            return await _context.Properties
                .AsNoTracking()
                .Where(p => p.Units.Any(u => !u.ClientUnits.Any()))
                .OrderBy(p => p.Name)
                .Select(p => new PropertyListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<UnitListItemDto>> GetAvailableUnitsByPropertyIdAsync(int propertyId)
        {
            var propertyExists = await _context.Properties.AnyAsync(p => p.Id == propertyId);
            if (!propertyExists)
                throw new InvalidOperationException("Property not found.");

            return await _context.Units
                .AsNoTracking()
                .Where(u => u.PropertyId == propertyId && !u.ClientUnits.Any())
                .OrderBy(u => u.UnitNumber)
                .Select(u => new UnitListItemDto
                {
                    Id = u.Id,
                    UnitNumber = u.UnitNumber,
                    PropertyName = u.Property.Name,
                    Floor = u.Floor,
                    Area = u.Area,
                    Status = u.Status
                })
                .ToListAsync();
        }

        public async Task AddRentedUnitAsync(int clientId, int unitId)
        {
            var client = await _context.Clients
                .Include(c => c.ClientUnits)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (client == null)
                throw new InvalidOperationException("Client not found.");

            var unit = await _context.Units.FindAsync(unitId);
            if (unit == null)
                throw new InvalidOperationException("Unit not found.");

            var isAlreadyAdded = client.ClientUnits.Any(cu => cu.UnitId == unitId);
            if (isAlreadyAdded)
                return;

            client.ClientUnits.Add(new ClientUnit { ClientId = clientId, UnitId = unitId });
            unit.Status = UnitStatus.Occupied;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRentedUnitAsync(int clientId, int unitId)
        {
            var client = await _context.Clients
                .Include(c => c.ClientUnits)
                .ThenInclude(cu => cu.Unit)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (client == null)
                throw new InvalidOperationException("Client not found.");

            var unit = client.ClientUnits.FirstOrDefault(cu => cu.UnitId == unitId);
            if (unit == null)
                return;

            client.ClientUnits.Remove(unit);
            await _context.SaveChangesAsync();
        }

        private static HashSet<ClientSearchField> ParseSearchFields(IEnumerable<string>? fields)
        {
            var result = new HashSet<ClientSearchField>();
            if (fields == null)
                return result;

            foreach (var field in fields)
            {
                if (Enum.TryParse<ClientSearchField>(field, ignoreCase: true, out var parsed))
                    result.Add(parsed);
            }

            return result;
        }

        private bool MatchesSearch(ClientListItemDto client, string term)
        {
            foreach (var field in _searchFields)
            {
                var value = field switch
                {
                    ClientSearchField.Name => client.DisplayName,
                    ClientSearchField.Email => client.Email,
                    ClientSearchField.Phone => client.Phone,
                    _ => null
                };

                if (!string.IsNullOrEmpty(value) &&
                    value.Contains(term, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static void ApplyClient(
            Client client,
            ClientType clientType,
            string email,
            string phone,
            string? firstName,
            string? lastName,
            string? personalId,
            DateTime? dateOfBirth,
            string? companyName,
            string? companyNumber,
            string? vatNumber,
            string? legalRepresentative,
            string? address,
            string? bankAccount,
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("Email is required.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new InvalidOperationException("Phone is required.");

            var iban = NormalizeIban(bankAccount);
            if (iban != null && !IbanPattern.IsMatch(iban))
                throw new InvalidOperationException("Bank account must be a valid IBAN.");

            client.ClientType = clientType;
            client.Email = email.Trim();
            client.Phone = phone.Trim();
            client.Address = Normalize(address);
            client.BankAccount = iban;
            client.Notes = Normalize(notes);

            if (clientType == ClientType.Individual)
            {
                client.FirstName = Normalize(firstName);
                client.LastName = Normalize(lastName);
                client.PersonalId = Normalize(personalId);
                client.DateOfBirth = dateOfBirth?.Date;

                if (client.FirstName == null)
                    throw new InvalidOperationException("First name is required for individual clients.");
                if (client.LastName == null)
                    throw new InvalidOperationException("Last name is required for individual clients.");
                if (client.PersonalId == null)
                    throw new InvalidOperationException("Personal ID is required for individual clients.");
                if (client.DateOfBirth > DateTime.Today)
                    throw new InvalidOperationException("Date of birth cannot be in the future.");

                client.CompanyName = null;
                client.CompanyNumber = null;
                client.VatNumber = null;
                client.LegalRepresentative = null;
            }
            else if (clientType == ClientType.Company)
            {
                client.CompanyName = Normalize(companyName);
                client.CompanyNumber = Normalize(companyNumber);
                client.VatNumber = Normalize(vatNumber);
                client.LegalRepresentative = Normalize(legalRepresentative);

                if (client.CompanyName == null)
                    throw new InvalidOperationException("Company name is required for company clients.");
                if (client.CompanyNumber == null)
                    throw new InvalidOperationException("Company ID is required for company clients.");
                if (client.LegalRepresentative == null)
                    throw new InvalidOperationException("Legal representative is required for company clients.");

                client.FirstName = null;
                client.LastName = null;
                client.PersonalId = null;
                client.DateOfBirth = null;
            }
            else
            {
                throw new InvalidOperationException("Client type is required.");
            }
        }

        private async Task EnsureIdentifierIsUniqueAsync(int? clientId, string? personalId, string? companyNumber)
        {
            if (personalId != null)
            {
                var personalIdTaken = await _context.Clients.AnyAsync(c =>
                    (clientId == null || c.Id != clientId) &&
                    (c.PersonalId == personalId || c.CompanyNumber == personalId));

                if (personalIdTaken)
                    throw new InvalidOperationException("This personal ID is already used by another client.");
            }

            if (companyNumber != null)
            {
                var companyIdTaken = await _context.Clients.AnyAsync(c =>
                    (clientId == null || c.Id != clientId) &&
                    (c.CompanyNumber == companyNumber || c.PersonalId == companyNumber));

                if (companyIdTaken)
                    throw new InvalidOperationException("This company ID is already used by another client.");
            }
        }

        private static string? Normalize(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static string? NormalizeIban(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Replace(" ", "", StringComparison.Ordinal).ToUpperInvariant();
        }

        private static ClientDto MapToDto(Client client) => new()
        {
            Id = client.Id,
            ClientType = client.ClientType,
            Email = client.Email,
            Phone = client.Phone,
            FirstName = client.FirstName,
            LastName = client.LastName,
            PersonalId = client.PersonalId,
            DateOfBirth = client.DateOfBirth,
            CompanyName = client.CompanyName,
            CompanyNumber = client.CompanyNumber,
            VatNumber = client.VatNumber,
            LegalRepresentative = client.LegalRepresentative,
            Address = client.Address,
            BankAccount = client.BankAccount,
            Notes = client.Notes,
            IsActive = client.IsActive,
            HasLinkedRecords = client.ClientUnits.Count > 0
        };
    }
}
