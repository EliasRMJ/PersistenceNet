using PersistenceNet.Enuns;

namespace PersistenceNet.Interfaces
{
    public interface IElement
    {
        long Id { get; set; }
        ElementStatesEnum ElementStates { get; set; }
    }
}