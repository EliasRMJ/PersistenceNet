using PersistenceNet.Entitys;
using PersistenceNet.Enuns;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace PersistenceNet.Test.Domain.Entitys
{
    [DebuggerDisplay("{Name}")]
    [DisplayName("Classification")]
    [Table("classification")]
    public class Classification : EntityBase
    {
        [Required(ErrorMessage = "Enter the description of the classification type!")]
        [MaxLength(60, ErrorMessage = "Classification type name cannot contain more than 60 characters!")]
        [Column("name", Order = 2)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Enter 'S' for active or 'N' for inactive!")]
        [MaxLength(1, ErrorMessage = "Active cannot contain more than 1 character!")]
        [Column("active", Order = 3)]
        public ActiveEnum Active { get; set; }
    }
}