using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace todo_api.Models
{
    public class ToDo
    {
        [Key]
        public int? todoid { get; set; }
        public string description { get; set; }
        public DateTime? dueDate { get; set; }
        public decimal? dollarValue { get; set; }
        public string notes { get; set; }
        public bool? completed { get; set; }
        public string color { get; set; }

        public override string ToString()
        {
            return $"{todoid} {description}";
        }
    }
}
