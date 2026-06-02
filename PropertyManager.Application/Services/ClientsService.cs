using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.Services
{
    public class ClientsService : IClientsService
    {
        private readonly PropertyManagerDbContext _context;

        public ClientsService(PropertyManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ClientListItemDto>> GetAllAsync()
        {
            return await _context.Clients
                .AsNoTracking()
                .OrderBy(c => c.ClientType)
                .ThenBy(c => c.CompanyName)
                .ThenBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .Select(c => new ClientListItemDto
                {
                    Id = c.Id,
                    ClientType = c.ClientType,
                    DisplayName = c.ClientType == ClientType.LegalEntity
                        ? c.CompanyName!
                        : c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    Phone = c.Phone
                })
                .ToListAsync();
        }

        public async Task<ClientDto?> GetByIdAsync(int id)
        {
            var client = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return null;

            return MapToDto(client);
        }

        public async Task<int> CreateAsync(CreateClientDto dto)
        {
            ValidateClientFields(dto.ClientType, dto.FirstName, dto.LastName, dto.CompanyName, dto.CompanyNumber);

            var client = new Client
            {
                ClientType = dto.ClientType,
                Email = dto.Email,
                Phone = dto.Phone,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalId = dto.PersonalId,
                CompanyName = dto.CompanyName,
                CompanyNumber = dto.CompanyNumber,
                VatNumber = dto.VatNumber,
                LegalRepresentative = dto.LegalRepresentative
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return client.Id;
        }

        public async Task EditAsync(EditClientDto dto)
        {
            ValidateClientFields(dto.ClientType, dto.FirstName, dto.LastName, dto.CompanyName, dto.CompanyNumber);

            var client = await _context.Clients.FindAsync(dto.Id);
            if (client == null)
                throw new InvalidOperationException("Client not found.");

            client.ClientType = dto.ClientType;
            client.Email = dto.Email;
            client.Phone = dto.Phone;
            client.FirstName = dto.FirstName;
            client.LastName = dto.LastName;
            client.PersonalId = dto.PersonalId;
            client.CompanyName = dto.CompanyName;
            client.CompanyNumber = dto.CompanyNumber;
            client.VatNumber = dto.VatNumber;
            client.LegalRepresentative = dto.LegalRepresentative;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }

        private static void ValidateClientFields(
            ClientType clientType,
            string? firstName,
            string? lastName,
            string? companyName,
            string? companyNumber)
        {
            if (clientType == ClientType.Individual)
            {
                if (string.IsNullOrWhiteSpace(firstName))
                    throw new InvalidOperationException("First name is required for individual clients.");
                if (string.IsNullOrWhiteSpace(lastName))
                    throw new InvalidOperationException("Last name is required for individual clients.");
            }
            else if (clientType == ClientType.LegalEntity)
            {
                if (string.IsNullOrWhiteSpace(companyName))
                    throw new InvalidOperationException("Company name is required for legal entity clients.");
                if (string.IsNullOrWhiteSpace(companyNumber))
                    throw new InvalidOperationException("Company number is required for legal entity clients.");
            }
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
            CompanyName = client.CompanyName,
            CompanyNumber = client.CompanyNumber,
            VatNumber = client.VatNumber,
            LegalRepresentative = client.LegalRepresentative
        };
    }
}
