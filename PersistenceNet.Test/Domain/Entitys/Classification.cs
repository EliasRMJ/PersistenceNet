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

        [Required(ErrorMessage = "Informe a descrição do tipo de classificação!")]
        [MaxLength(60, ErrorMessage = "Nome do tipo de classificação não pode conter mais de 60 caracteres!")]
        [Column("name", Order = 2)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Informe 'S' para ativo ou 'N' para inativo!")]
        [MaxLength(1, ErrorMessage = "Active não pode conter mais de 1 caracteres!")]
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