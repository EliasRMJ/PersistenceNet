using PersistenceNet.Enuns;
using PersistenceNet.Interfaces;
using PersistenceNet.Test.Domain.Views;
using PersistenceNet.Views;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace PersistenceNet.Test.Domain.Entitys
{
    [DebuggerDisplay("{Name}")]
    [DisplayName("Classification")]
    [Table("classification")]
    public class Classification : IElement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column("id", Order = 1)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the description of the classification type!")]
        [MaxLength(60, ErrorMessage = "Classification type name cannot contain more than 60 characters!")]
        [Column("name", Order = 2)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Enter 'S' for active or 'N' for inactive!")]
        [MaxLength(1, ErrorMessage = "Active cannot contain more than 1 character!")]
        [Column("active", Order = 3)]
        public ActiveEnum Active { get; set; }

        [NotMapped]
        public ElementStatesEnum ElementStates { get => Id.Equals(0) ? ElementStatesEnum.New : ElementStatesEnum.Update; set { } }

        public ViewBase GetView()
        {
            return new ClassificationView
            {
                Id = this.Id,
                Name = this.Name,
                IdentifyObject = Guid.NewGuid(),
                IsActive = true,
                IsDelete = false,
                IsCheck = false
            };
        }
    }
}