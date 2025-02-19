namespace FerovinumTakeHome.Domain
{
    public abstract class OrderBase
    {
        public string SKU { get; set; }
        public int OriginalQuantity { get; set; }
        public abstract bool IsClosed { get; }
        public abstract override string ToString();
    }
}
