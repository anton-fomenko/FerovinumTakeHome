using FerovinumTakeHome.Domain;

namespace FerovinumTakeHome.Commands
{
    // Common interface for all commands
    public interface IFerovinumCommand
    {
        void Execute(FerovinumContext context);
    }
}
