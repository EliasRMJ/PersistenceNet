using PersistenceNet.Extensions;

namespace PersistenceNet.Enuns
{
    public enum ElementStatesEnum
    {
        [DescriptionEnumAttribute("Novo")]
        New = 0,
        [DescriptionEnumAttribute("Alterado")]
        Update = 1,
        [DescriptionEnumAttribute("Deletado")]
        Delete = 2
    }
}