using FerovinumTakeHome.Domain;

namespace FerovinumTakeHome.Commands
{
    public class BuyCommand : IFerovinumCommand
    {
        private readonly string _sku;
        private readonly int _requestQuantity;

        public BuyCommand(string sku, int quantity)
        {
            _sku = sku;
            _requestQuantity = quantity;
        }

        public void Execute(FerovinumContext context)
        {
            if (!context.AllowedSkus.Contains(_sku))
            {
                return;
            }

            // If no sells exist for this SKU, or the queue is empty, nothing to fulfill
            if (!context.SellQueuesBySku.ContainsKey(_sku) || context.SellQueuesBySku[_sku].Count == 0)
            {
                return; // do nothing, no partial fulfillment
            }

            int remainingToBuy = _requestQuantity;
            int totalFilled = 0;
            var sellQueue = context.SellQueuesBySku[_sku];

            // FIFO consumption
            while (sellQueue.Count > 0 && remainingToBuy > 0)
            {
                var firstStock = sellQueue.Peek();
                if (firstStock.Remaining > 0)
                {
                    int fillAmount = Math.Min(firstStock.Remaining, remainingToBuy);
                    firstStock.Remaining -= fillAmount;
                    remainingToBuy -= fillAmount;
                    totalFilled += fillAmount;
                }

                if(firstStock.IsClosed)
                {
                    sellQueue.Dequeue();
                }
            }

            // Only create a BUY order if we actually filled something
            if (totalFilled > 0)
            {
                var buyOrder = new BuyOrder
                {
                    SKU = _sku,
                    OriginalQuantity = _requestQuantity,
                    FilledQuantity = totalFilled
                };

                context.AllOrders.Add(buyOrder);
            }
        }
    }
}
