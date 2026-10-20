namespace PropertyManager.WEB.ViewModels.Units
{
    public class UnitDetailsViewModel
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string UnitNumber { get; set; } = null!;
        public decimal Area { get; set; }
        public int Floor { get; set; }
        public string Status { get; set; } = null!;

        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = null!;
        public int? ClientId { get; set; }

        public IReadOnlyList<UnitPriceLineViewModel> PriceLines { get; set; } = Array.Empty<UnitPriceLineViewModel>();
        public IReadOnlyList<UnitPriceTotalViewModel> Totals { get; set; } = Array.Empty<UnitPriceTotalViewModel>();
        public UnitPriceFormViewModel PriceForm { get; set; } = new();
        public bool HasActiveCatalogComponents { get; set; }
    }
}
