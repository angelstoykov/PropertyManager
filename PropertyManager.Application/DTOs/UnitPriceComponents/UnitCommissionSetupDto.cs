namespace PropertyManager.Application.DTOs.UnitPriceComponents;

public class UnitCommissionSetupDto
{
    public int UnitId { get; set; }
    public string CurrencyCode { get; set; } = "";
    public decimal IncludedAmount { get; set; }
    public decimal ExcludedAmount { get; set; }
}
