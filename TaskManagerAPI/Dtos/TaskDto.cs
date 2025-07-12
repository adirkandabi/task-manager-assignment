using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagerAPI.Enums;
using System.Text.Json.Serialization;

namespace TaskManagerAPI.Dtos
{
    public class TaskDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskItemStatus Status { get; set; }  // Enum
    }
}
