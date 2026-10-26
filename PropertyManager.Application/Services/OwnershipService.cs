using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.DTOs.Ownership;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Domain.Models.Entities;
using PropertyManager.Domain.Models.Enums;

namespace PropertyManager.Application.Services
{
    public class OwnershipService : IOwnershipService
    {
        private readonly PropertyManagerDbContext _context;
        private readonly IClientsService _clientsService;

        public OwnershipService(PropertyManagerDbContext context, IClientsService clientsService)
        {
            _context = context;
            _clientsService = clientsService;
        }

        public async Task<TargetOwnershipDto> GetPropertyOwnershipAsync(int propertyId, DateTime? onDate)
        {
            var property = await _context.Properties.AsNoTracking()
                .Where(p => p.Id == propertyId)
                .Select(p => new { p.Id, p.Name })
                .FirstOrDefaultAsync();
            if (property == null)
                throw new InvalidOperationException("Building not found.");

            var records = await LoadRecordsAsync(propertyId, null);
            var asOf = (onDate ?? DateTime.Today).Date;
            return new TargetOwnershipDto
            {
                AsOf = asOf,
                IsInherited = false,
                PropertyId = property.Id,
                TargetName = property.Name,
                BuildingName = property.Name,
                Owners = ActiveShares(records, asOf),
                History = GroupHistory(records)
            };
        }

        public async Task<TargetOwnershipDto> GetUnitOwnershipAsync(int unitId, DateTime? onDate)
        {
            var unit = await _context.Units.AsNoTracking()
                .Where(u => u.Id == unitId)
                .Select(u => new { u.Id, u.UnitNumber, u.PropertyId, PropertyName = u.Property.Name })
                .FirstOrDefaultAsync();
            if (unit == null)
                throw new InvalidOperationException("Unit not found.");

            var asOf = (onDate ?? DateTime.Today).Date;
            var ownRecords = await LoadRecordsAsync(null, unit.Id);
            var ownOwners = ActiveShares(ownRecords, asOf);
            var targetName = $"{unit.PropertyName} · {unit.UnitNumber}";
            if (Covers(ownRecords, asOf))
            {
                return new TargetOwnershipDto
                {
                    AsOf = asOf,
                    IsInherited = false,
                    PropertyId = unit.PropertyId,
                    UnitId = unit.Id,
                    TargetName = targetName,
                    BuildingName = unit.PropertyName,
                    Owners = ownOwners,
                    History = GroupHistory(ownRecords)
                };
            }

            var buildingRecords = await LoadRecordsAsync(unit.PropertyId, null);
            return new TargetOwnershipDto
            {
                AsOf = asOf,
                IsInherited = true,
                PropertyId = unit.PropertyId,
                UnitId = unit.Id,
                TargetName = targetName,
                BuildingName = unit.PropertyName,
                Owners = ActiveShares(buildingRecords, asOf),
                History = GroupHistory(ownRecords)
            };
        }

        public Task SetPropertyOwnershipAsync(int propertyId, SetOwnershipDto dto) =>
            ReplaceAsync(propertyId, null, dto);

        public Task SetUnitOwnershipAsync(int unitId, SetOwnershipDto dto) =>
            ReplaceAsync(null, unitId, dto);

        public async Task<ClientOwnershipDto> GetClientOwnershipAsync(int clientId)
        {
            var client = await _context.Clients.AsNoTracking()
                .Where(c => c.Id == clientId)
                .Select(c => new
                {
                    c.Id,
                    c.ClientType,
                    c.CompanyName,
                    c.FirstName,
                    c.LastName
                })
                .FirstOrDefaultAsync();
            if (client == null)
                throw new InvalidOperationException("Client not found.");

            var direct = await _context.OwnershipRecords.AsNoTracking()
                .Where(o => o.ClientId == clientId)
                .Select(o => new DirectShare
                {
                    PropertyId = o.PropertyId,
                    UnitId = o.UnitId,
                    SharePercent = o.SharePercent,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    PropertyName = o.PropertyId != null ? o.Property!.Name : o.Unit!.Property.Name,
                    UnitNumber = o.UnitId != null ? o.Unit!.UnitNumber : null
                })
                .ToListAsync();

            var items = direct.Select(row => new ClientOwnershipItemDto
            {
                TargetName = row.UnitId == null
                    ? row.PropertyName
                    : $"{row.PropertyName} · {row.UnitNumber}",
                Kind = row.UnitId == null ? "Building" : "Unit",
                SharePercent = row.SharePercent,
                StartDate = row.StartDate,
                EndDate = row.EndDate,
                PropertyId = row.PropertyId,
                UnitId = row.UnitId
            }).ToList();

            var propertyIds = direct.Where(row => row.PropertyId != null)
                .Select(row => row.PropertyId!.Value)
                .Distinct()
                .ToList();
            if (propertyIds.Count > 0)
            {
                var units = await _context.Units.AsNoTracking()
                    .Where(u => propertyIds.Contains(u.PropertyId))
                    .Select(u => new { u.Id, u.UnitNumber, u.PropertyId, PropertyName = u.Property.Name })
                    .ToListAsync();
                var unitIds = units.Select(u => u.Id).ToList();
                var blocks = await _context.OwnershipRecords.AsNoTracking()
                    .Where(o => o.UnitId != null && unitIds.Contains(o.UnitId.Value))
                    .Select(o => new { UnitId = o.UnitId!.Value, o.StartDate, o.EndDate })
                    .ToListAsync();

                foreach (var share in direct.Where(row => row.PropertyId != null))
                {
                    foreach (var unit in units.Where(u => u.PropertyId == share.PropertyId))
                    {
                        var unitBlocks = blocks
                            .Where(block => block.UnitId == unit.Id)
                            .Select(block => (block.StartDate, block.EndDate))
                            .ToList();
                        foreach (var gap in Gaps(share.StartDate, share.EndDate, unitBlocks))
                        {
                            items.Add(new ClientOwnershipItemDto
                            {
                                TargetName = $"{unit.PropertyName} · {unit.UnitNumber}",
                                Kind = "Inherited unit",
                                SharePercent = share.SharePercent,
                                StartDate = gap.Start,
                                EndDate = gap.End,
                                PropertyId = unit.PropertyId,
                                UnitId = unit.Id
                            });
                        }
                    }
                }
            }

            return new ClientOwnershipDto
            {
                ClientId = client.Id,
                ClientName = client.ClientType == ClientType.Company
                    ? client.CompanyName ?? string.Empty
                    : $"{client.FirstName} {client.LastName}".Trim(),
                Items = items
                    .OrderByDescending(item => item.EndDate == null)
                    .ThenByDescending(item => item.StartDate)
                    .ThenBy(item => item.TargetName)
                    .ToList()
            };
        }

        private async Task ReplaceAsync(int? propertyId, int? unitId, SetOwnershipDto dto)
        {
            if (propertyId == null && unitId == null)
                throw new InvalidOperationException("Ownership target is required.");

            if (propertyId != null && !await _context.Properties.AnyAsync(p => p.Id == propertyId))
                throw new InvalidOperationException("Building not found.");
            if (unitId != null && !await _context.Units.AnyAsync(u => u.Id == unitId))
                throw new InvalidOperationException("Unit not found.");

            var effectiveDate = dto.EffectiveDate.Date;
            if (effectiveDate == default)
                throw new InvalidOperationException("Effective date is required.");

            var shares = NormalizeShares(dto.Shares);
            var records = await _context.OwnershipRecords
                .Where(o => o.PropertyId == propertyId && o.UnitId == unitId)
                .ToListAsync();

            if (records.Any(record => record.StartDate > effectiveDate))
                throw new InvalidOperationException(
                    "Choose an effective date on or after the latest ownership period.");

            if (records.Any(record =>
                    record.EndDate != null &&
                    record.StartDate < effectiveDate &&
                    record.EndDate > effectiveDate))
            {
                throw new InvalidOperationException("This date falls inside an earlier ownership period.");
            }

            var open = records.Where(record => record.EndDate == null).ToList();
            if (shares.Count == 0 && open.Count == 0)
                throw new InvalidOperationException("There is no current ownership to end.");

            foreach (var record in open)
                record.EndDate = effectiveDate;

            foreach (var share in shares)
            {
                var exists = await _context.Clients.AnyAsync(c => c.Id == share.ClientId);
                if (!exists)
                    throw new InvalidOperationException("Owner was not found.");

                await _clientsService.EnsureRoleAsync(share.ClientId, ClientRoleType.Owner, effectiveDate);
                _context.OwnershipRecords.Add(new OwnershipRecord
                {
                    PropertyId = propertyId,
                    UnitId = unitId,
                    ClientId = share.ClientId,
                    SharePercent = share.SharePercent,
                    StartDate = effectiveDate
                });
            }

            await _context.SaveChangesAsync();
        }

        private static List<(int ClientId, decimal SharePercent)> NormalizeShares(IEnumerable<OwnershipShareInputDto>? shares)
        {
            var rows = (shares ?? [])
                .Where(share => share.ClientId > 0)
                .Select(share => (
                    share.ClientId,
                    Original: share.SharePercent,
                    Share: decimal.Round(share.SharePercent, 2, MidpointRounding.AwayFromZero)))
                .ToList();

            if (rows.Count == 0)
                return [];

            if (rows.GroupBy(row => row.ClientId).Any(group => group.Count() > 1))
                throw new InvalidOperationException("Each owner can appear only once in the same period.");

            foreach (var row in rows)
            {
                if (row.Original != row.Share || row.Share <= 0 || row.Share > 100)
                    throw new InvalidOperationException("Each share must be greater than 0 and at most 100, with up to two decimal places.");
            }

            if (rows.Sum(row => row.Share) != 100m)
                throw new InvalidOperationException("Ownership shares must total 100%.");

            return rows.Select(row => (row.ClientId, row.Share)).ToList();
        }

        private async Task<List<OwnershipRow>> LoadRecordsAsync(int? propertyId, int? unitId)
        {
            return await _context.OwnershipRecords.AsNoTracking()
                .Where(o => o.PropertyId == propertyId && o.UnitId == unitId)
                .Select(o => new OwnershipRow
                {
                    ClientId = o.ClientId,
                    ClientName = o.Client.ClientType == ClientType.Company
                        ? o.Client.CompanyName!
                        : o.Client.FirstName + " " + o.Client.LastName,
                    SharePercent = o.SharePercent,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate
                })
                .ToListAsync();
        }

        private static bool Covers(IEnumerable<OwnershipRow> records, DateTime date) =>
            records.Any(record => record.StartDate <= date && (record.EndDate == null || date < record.EndDate));

        private static IReadOnlyList<OwnershipShareDto> ActiveShares(IEnumerable<OwnershipRow> records, DateTime date) =>
            records
                .Where(record => record.StartDate <= date && (record.EndDate == null || date < record.EndDate))
                .OrderByDescending(record => record.SharePercent)
                .ThenBy(record => record.ClientName)
                .Select(record => new OwnershipShareDto
                {
                    ClientId = record.ClientId,
                    ClientName = record.ClientName,
                    SharePercent = record.SharePercent,
                    StartDate = record.StartDate
                })
                .ToList();

        private static IReadOnlyList<OwnershipPeriodDto> GroupHistory(IEnumerable<OwnershipRow> records) =>
            records
                .GroupBy(record => new { record.StartDate, record.EndDate })
                .OrderByDescending(group => group.Key.StartDate)
                .ThenByDescending(group => group.Key.EndDate == null)
                .Select(group => new OwnershipPeriodDto
                {
                    StartDate = group.Key.StartDate,
                    EndDate = group.Key.EndDate,
                    Shares = group
                        .OrderByDescending(record => record.SharePercent)
                        .ThenBy(record => record.ClientName)
                        .Select(record => new OwnershipShareDto
                        {
                            ClientId = record.ClientId,
                            ClientName = record.ClientName,
                            SharePercent = record.SharePercent,
                            StartDate = record.StartDate
                        })
                        .ToList()
                })
                .ToList();

        private static List<(DateTime Start, DateTime? End)> Gaps(
            DateTime start,
            DateTime? end,
            IReadOnlyList<(DateTime Start, DateTime? End)> blocks)
        {
            var gaps = new List<(DateTime Start, DateTime? End)>();
            var cursor = start;
            var coveredThroughEnd = false;

            foreach (var block in blocks.OrderBy(block => block.Start).ThenBy(block => block.End ?? DateTime.MaxValue))
            {
                if (end != null && cursor >= end)
                    break;
                if (block.End != null && block.End <= cursor)
                    continue;
                if (end != null && block.Start >= end)
                    break;

                if (block.Start > cursor)
                {
                    var gapEnd = end == null || block.Start < end ? block.Start : end;
                    if (gapEnd == null || gapEnd > cursor)
                        gaps.Add((cursor, gapEnd));
                }

                if (block.End == null || (end != null && block.End >= end))
                {
                    coveredThroughEnd = true;
                    break;
                }

                if (block.End > cursor)
                    cursor = block.End.Value;
            }

            if (!coveredThroughEnd && (end == null || cursor < end))
                gaps.Add((cursor, end));

            return gaps.Where(gap => gap.End == null || gap.End > gap.Start).ToList();
        }

        private sealed class OwnershipRow
        {
            public int ClientId { get; set; }
            public string ClientName { get; set; } = null!;
            public decimal SharePercent { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        private sealed class DirectShare
        {
            public int? PropertyId { get; set; }
            public int? UnitId { get; set; }
            public decimal SharePercent { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string PropertyName { get; set; } = null!;
            public string? UnitNumber { get; set; }
        }
    }
}
