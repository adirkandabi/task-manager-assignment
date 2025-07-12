using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagerAPI.Enums;
using System.Text.Json.Serialization;

namespace TaskManagerAPI.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskItemStatus Status { get; set; }  // Enum

        [Required]
        public int ProjectId { get; set; }  // Foreign key to Project

        [ForeignKey("ProjectId")]
        [JsonIgnore]
        public Project? Project { get; set; }
    }
}
