namespace PropertyManager.Application.DTOs.UnitFinancialRecords;

public class UnitIncomeTotalDto
{
    public int PropertyId { get; set; }
    public int UnitId { get; set; }
    public string UnitNumber { get; set; } = "";
    public string CurrencyCode { get; set; } = "";
    public string CurrencySymbol { get; set; } = "";
    public decimal Amount { get; set; }
    public int IncomeCount { get; set; }
}
