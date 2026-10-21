namespace PropertyManager.Application.DTOs.FinancialRecordSubtypes
{
    public class EditFinancialRecordSubtypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
