namespace PropertyManager.Application.DTOs.UnitFinancialRecords;

public class BuildingFinancialTotalDto
{
    public int PropertyId { get; set; }
    public string CurrencyCode { get; set; } = "";
    public string CurrencySymbol { get; set; } = "";
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
}
