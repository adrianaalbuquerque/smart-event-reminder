using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEventReminder.Domain.DTOs
{
    public class EventDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }  // já UTC se vem com Z
        public string? Notificacao { get; set; } // opcional
        public string? Email { get; set; }       // novo campo opcional
    }

}
