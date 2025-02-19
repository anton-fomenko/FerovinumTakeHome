using FerovinumTakeHome.Commands;

namespace FerovinumTakeHome.Services
{
    public class CommandFactory : ICommandFactory
    {
        public IFerovinumCommand? CreateCommand(string[] tokens)
        {
            // We assume tokens.Length >= 1 here
            string commandType = tokens[0].ToLowerInvariant();

            switch (commandType)
            {
                case "sell":
                    {
                        // Expect 3 tokens => [0]: "sell", [1]: SKU, [2]: quantity
                        if (tokens.Length < 3) return null;
                        string sku = tokens[1];
                        if (!int.TryParse(tokens[2], out int sellQty) || sellQty <= 0)
                            return null;

                        return new SellCommand(sku, sellQty);
                    }

                case "buy":
                    {
                        // Expect 3 tokens => [0]: "buy", [1]: SKU, [2]: quantity
                        if (tokens.Length < 3) return null;
                        string sku = tokens[1];
                        if (!int.TryParse(tokens[2], out int buyQty) || buyQty <= 0)
                            return null;

                        return new BuyCommand(sku, buyQty);
                    }

                // Future commands like "reserve", "transfer", etc.:
                // case "reserve":
                //    if (tokens.Length < 4) return null;
                //    string sku = tokens[1];
                //    int reserveQty = int.Parse(tokens[2]);
                //    string reason = tokens[3]; // or parse more tokens
                //    return new ReserveCommand(sku, reserveQty, reason);

                default:
                    // Unknown command => return null
                    return null;
            }
        }
    }
}
