namespace FerovinumTakeHome.Domain
{
    // Holds the entire in-memory state of our Ferovinum system
    public class FerovinumContext
    {
        // A complete list of all orders in chronological order
        public List<OrderBase> AllOrders { get; } = new List<OrderBase>();

        // For each SKU, keep a FIFO queue of SELL orders that still have Remaining > 0
        public Dictionary<string, Queue<SellOrder>> SellQueuesBySku { get; }
            = new Dictionary<string, Queue<SellOrder>>();

        public readonly HashSet<string> AllowedSkus = new()
        {
            "wine",
            "whisky"
        };
    }
}