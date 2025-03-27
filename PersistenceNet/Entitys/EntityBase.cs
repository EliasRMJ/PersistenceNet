using PersistenceNet.Enuns;
using PersistenceNet.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistenceNet.Entitys
{
    public abstract class EntityBase : IElement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column("Id", Order = 1)]
        public virtual long Id { get; set; }

        [NotMapped]
        public ElementStatesEnum ElementStates { get => Id.Equals(0) ? ElementStatesEnum.New : ElementStatesEnum.Update; set { } }
    }
}