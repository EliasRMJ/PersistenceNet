using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Views;

namespace PersistenceNet.Test.Domain.Views
{
    public class ClassificationView: ViewBase
    {
        public override string ClassName => "Classification";
        public override bool IsActive { get => true; set => base.IsActive = value; }

        public string? Name { get; set; }

        public override object ConvertTo(object gDomain)
        {
            var classification = (gDomain == null) ? new() { Id = this.Id } : (Classification)gDomain;
            classification.Name = this.Name;
            classification.Active = IsActive ? "S" : "N";

            return classification;
        }
    }
}