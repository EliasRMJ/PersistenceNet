using PersistenceNet.Entitys;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace PersistenceNet.Test.Domain.Entitys
{
    [DebuggerDisplay("{Name}")]
    [DisplayName("Classification")]
    [Table("Classification")]
    public class Classification : EntityTypeBase { }
}