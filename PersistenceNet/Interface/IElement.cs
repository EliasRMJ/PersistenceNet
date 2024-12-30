using PersistenceNet.Enuns;
using PersistenceNet.Views;

namespace PersistenceNet.Interfaces
{
    public interface IElement
    {
        ElementStatesEnum ElementStates { get; set; }
        ViewBase GetView();
    }
}