using PersistenceNet.Enuns;

namespace PersistenceNet.Test.Domain.ViewModels
{
    public class ClassificationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ActiveEnum Active { get; set; }
    }
}