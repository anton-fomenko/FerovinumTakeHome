namespace FerovinumTakeHome.Domain
{
    public class SellOrder : OrderBase
    {
        public int Remaining { get; set; }
        public override bool IsClosed => Remaining == 0;

        public override string ToString()
        {
            if (IsClosed)
            {
                return $"sell {SKU} {OriginalQuantity} closed";
            }
            else
            {
                return $"sell {SKU} {OriginalQuantity} remaining:{Remaining}";
            }
        }
    }
}
