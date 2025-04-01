using PersistenceNet.Enuns;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistenceNet.Entitys
{
    public abstract class EntityTypeBase : EntityBase
    {
        [Required(ErrorMessage = "Name is mandatory!")]
        [MaxLength(100, ErrorMessage = "Name cannot contain more than 100 characters!")]
        [Column("Name")]
        public virtual string? Name { get; set; }

        [Required(ErrorMessage = "The status ('S or N') of the entity must be reported!")]
        [Column("Active")]
        public virtual ActiveEnum Active { get; set; }
    }
}