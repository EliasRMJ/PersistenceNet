using PersistenceNet.Interface;

namespace PersistenceNet.Views
{
    public abstract class ViewBase : IViewConvert
    {
        public virtual string ClassName => this.ToString()!.Split('.').Last();

        public int Id { get; set; }
        public bool IsDelete { get; set; }
        public bool IsCheck { get; set; }
        public bool IsNew { get { return (Id == 0 && !IsCheck); } }
        public virtual bool IsActive { get; set; }
        public string? OrganizationCode { get; set; }
        public Guid IdentifyObject { get; set; }

        public string ViewId => $"{Id.ToString().PadLeft(7, '0')}";

        public virtual object ConvertTo(object gDomain)
        {
            return gDomain;
        }
    }
}