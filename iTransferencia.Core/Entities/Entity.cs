using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace iTransferencia.Core.Entities
{
    public abstract class Entity<TPrimaryKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TPrimaryKey Id { get; set; }

        public bool IsEquals(TPrimaryKey id)
        {
            return Id.Equals(id);
        }
    }
}
