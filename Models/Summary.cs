using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_api.Components;

namespace todo_api.Models
{
    public class Summary
    {
        public GetToDosFromDBByFilter.FilterOptions FilterOption { get; set; }
        public int totalToDos { get; set; }
    }
}
