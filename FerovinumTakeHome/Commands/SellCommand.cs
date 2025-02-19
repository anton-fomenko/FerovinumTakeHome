using FerovinumTakeHome.Domain;

namespace FerovinumTakeHome.Commands
{
    public class SellCommand : IFerovinumCommand
    {
        private readonly string _sku;
        private readonly int _quantity;

        public SellCommand(string sku, int quantity)
        {
            _sku = sku;
            _quantity = quantity;
        }

        public void Execute(FerovinumContext context)
        {
            if (!context.AllowedSkus.Contains(_sku))
            {
                return;
            }

            var order = new SellOrder
            {
                SKU = _sku,
                OriginalQuantity = _quantity,
                Remaining = _quantity
            };

            // Add to the history list
            context.AllOrders.Add(order);

            // Enqueue it into the FIFO for this SKU
            if (!context.SellQueuesBySku.ContainsKey(_sku))
            {
                context.SellQueuesBySku[_sku] = new Queue<SellOrder>();
            }
            context.SellQueuesBySku[_sku].Enqueue(order);
        }
    }
}
