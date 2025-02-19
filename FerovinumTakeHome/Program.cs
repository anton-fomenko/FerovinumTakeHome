using FerovinumTakeHome.Services;

namespace FerovinumTakeHome
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var processor = new FerovinumProcessor();
            while (true)
            {
                string input = Console.ReadLine();
                if (input == null)
                    break; // no more input => exit

                var lines = processor.ProcessCommand(input);

                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
                Console.WriteLine(); // blank line to signal end of output block
            }
        }
    }
}
