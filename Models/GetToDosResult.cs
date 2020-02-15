using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_api.Components;

namespace todo_api.Models
{
    public class GetToDosResult
    {
        public GetToDosFromDBByFilter.FilterOptions FilterOption { get; set; }
        public IEnumerable<ToDo> ToDos { get; set; }
        public IEnumerable<Summary> Summaries { get; set; }
    }
}
