using Microsoft.AspNetCore.Mvc.Rendering;
using PropertyManager.Application.DTOs.Clients;
using PropertyManager.Application.DTOs.Ownership;

namespace PropertyManager.WEB.ViewModels.Ownership
{
    public class OwnershipShareFormRow
    {
        public int? ClientId { get; set; }
        public decimal? SharePercent { get; set; }
    }

    public class OwnershipOwnerRowViewModel
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = null!;
        public decimal SharePercent { get; set; }
    }

    public class OwnershipPeriodViewModel
    {
        public string Period { get; set; } = null!;
        public string Owners { get; set; } = null!;
    }

    public class OwnershipPageViewModel
    {
        public int? PropertyId { get; set; }
        public int? UnitId { get; set; }
        public string Title { get; set; } = "Owners";
        public string TargetName { get; set; } = null!;
        public DateTime AsOf { get; set; }
        public bool IsInherited { get; set; }
        public string? BuildingName { get; set; }
        public IReadOnlyList<OwnershipOwnerRowViewModel> Owners { get; set; } = [];
        public IReadOnlyList<OwnershipPeriodViewModel> History { get; set; } = [];
        public DateTime EffectiveDate { get; set; } = DateTime.Today;
        public List<OwnershipShareFormRow> Shares { get; set; } = [];
        public bool EndOwnership { get; set; }
        public IReadOnlyList<SelectListItem> Clients { get; set; } = [];
    }

    public class ClientOwnershipPageViewModel
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = null!;
        public IReadOnlyList<ClientOwnershipRowViewModel> Current { get; set; } = [];
        public IReadOnlyList<ClientOwnershipRowViewModel> History { get; set; } = [];
    }

    public class ClientOwnershipRowViewModel
    {
        public string TargetName { get; set; } = null!;
        public string Kind { get; set; } = null!;
        public string Share { get; set; } = null!;
        public string Period { get; set; } = null!;
        public int? PropertyId { get; set; }
        public int? UnitId { get; set; }
    }

    public static class OwnershipDisplay
    {
        public static string Share(decimal share) => share.ToString("0.##") + "%";

        public static string Period(DateTime start, DateTime? exclusiveEnd)
        {
            var from = start.ToString("yyyy-MM-dd");
            if (exclusiveEnd == null)
                return from + " – current";
            if (exclusiveEnd <= start)
                return from + " (replaced the same day)";
            return from + " – " + exclusiveEnd.Value.AddDays(-1).ToString("yyyy-MM-dd");
        }

        public static OwnershipPageViewModel FromTarget(
            TargetOwnershipDto dto,
            IReadOnlyList<ClientListItemDto> clients,
            bool prefillOwners)
        {
            var owners = prefillOwners && !dto.IsInherited ? dto.Owners : [];
            var shares = owners.Select(owner => new OwnershipShareFormRow
            {
                ClientId = owner.ClientId,
                SharePercent = owner.SharePercent
            }).ToList();
            while (shares.Count < 1)
                shares.Add(new OwnershipShareFormRow());

            return new OwnershipPageViewModel
            {
                PropertyId = dto.PropertyId,
                UnitId = dto.UnitId,
                Title = dto.UnitId == null ? "Building owners" : "Unit owners",
                TargetName = dto.TargetName,
                AsOf = dto.AsOf,
                IsInherited = dto.IsInherited,
                BuildingName = dto.BuildingName,
                Owners = dto.Owners.Select(owner => new OwnershipOwnerRowViewModel
                {
                    ClientId = owner.ClientId,
                    ClientName = owner.ClientName,
                    SharePercent = owner.SharePercent
                }).ToList(),
                History = dto.History.Select(period => new OwnershipPeriodViewModel
                {
                    Period = Period(period.StartDate, period.EndDate),
                    Owners = period.Shares.Count == 0
                        ? "No owners"
                        : string.Join(", ", period.Shares.Select(share => $"{share.ClientName} {Share(share.SharePercent)}"))
                }).ToList(),
                EffectiveDate = DateTime.Today,
                Shares = shares,
                Clients = clients
                    .OrderBy(client => client.DisplayName)
                    .Select(client => new SelectListItem(client.DisplayName, client.Id.ToString()))
                    .ToList()
            };
        }
    }
}
