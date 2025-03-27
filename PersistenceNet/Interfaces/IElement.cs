using PersistenceNet.Enuns;

namespace PersistenceNet.Interfaces
{
    public interface IElement
    {
        int Id { get; set; }
        ElementStatesEnum ElementStates { get; set; }
    }
}