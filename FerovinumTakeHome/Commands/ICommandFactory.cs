using FerovinumTakeHome.Commands;

namespace FerovinumTakeHome.Services
{
    public interface ICommandFactory
    {
        IFerovinumCommand? CreateCommand(string[] tokens);
    }
}
