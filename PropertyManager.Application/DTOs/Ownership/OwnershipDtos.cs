namespace PropertyManager.Application.DTOs.Ownership
{
    public class OwnershipShareDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = null!;
        public decimal SharePercent { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class OwnershipPeriodDto
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IReadOnlyList<OwnershipShareDto> Shares { get; set; } = [];
    }

    public class TargetOwnershipDto
    {
        public DateTime AsOf { get; set; }
        public bool IsInherited { get; set; }
        public int? PropertyId { get; set; }
        public int? UnitId { get; set; }
        public string TargetName { get; set; } = null!;
        public string? BuildingName { get; set; }
        public IReadOnlyList<OwnershipShareDto> Owners { get; set; } = [];
        public IReadOnlyList<OwnershipPeriodDto> History { get; set; } = [];
    }

    public class OwnershipShareInputDto
    {
        public int ClientId { get; set; }
        public decimal SharePercent { get; set; }
    }

    public class SetOwnershipDto
    {
        public DateTime EffectiveDate { get; set; }
        public List<OwnershipShareInputDto> Shares { get; set; } = [];
    }

    public class ClientOwnershipItemDto
    {
        public string TargetName { get; set; } = null!;
        public string Kind { get; set; } = null!;
        public decimal SharePercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? PropertyId { get; set; }
        public int? UnitId { get; set; }
    }

    public class ClientOwnershipDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = null!;
        public IReadOnlyList<ClientOwnershipItemDto> Items { get; set; } = [];
    }
}
