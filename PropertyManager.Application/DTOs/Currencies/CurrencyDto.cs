namespace PropertyManager.Application.DTOs.Currencies
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public int DecimalPlaces { get; set; }
        public bool IsActive { get; set; }
    }
}
