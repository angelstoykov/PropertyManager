namespace PropertyManager.Domain.Contracts
{
    internal interface IUnit
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public decimal Area { get; set; }
    }
}
