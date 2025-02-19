namespace FerovinumTakeHome.Domain
{
    public class BuyOrder : OrderBase
    {
        public override bool IsClosed => true;

        public int FilledQuantity { get; set; }

        public override string ToString()
        {
            return $"buy {SKU} {FilledQuantity} closed";
        }
    }
}
