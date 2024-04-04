using System.ComponentModel.DataAnnotations;

namespace Conso.Providers.Entities
{
    public class ClassEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
