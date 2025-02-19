using FerovinumTakeHome.Domain;

namespace FerovinumTakeHome.Services
{
    public class FerovinumProcessor
    {
        private readonly FerovinumContext _context;
        private readonly CommandFactory _commandFactory;

        public FerovinumProcessor()
        {
            _context = new FerovinumContext();
            _commandFactory = new CommandFactory();
        }

        public List<string> ProcessCommand(string input)
        {
            var tokens = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                return GetAllOrders();
            }

            var command = _commandFactory.CreateCommand(tokens);
            if (command != null)
            {
                command.Execute(_context);
            }

            return GetAllOrders();
        }

        private List<string> GetAllOrders()
        {
            var lines = new List<string>();
            foreach (var order in _context.AllOrders)
            {
                lines.Add(order.ToString());
            }
            return lines;
        }
    }
}
