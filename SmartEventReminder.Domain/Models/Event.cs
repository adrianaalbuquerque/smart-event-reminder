using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEventReminder.Domain.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime? ultimanotificacao1h { get; set; }
        public DateTime? ultimanotificacao10m { get; set; }

    }
}
