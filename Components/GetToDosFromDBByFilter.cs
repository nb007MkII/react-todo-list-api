using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_api.Models;

namespace todo_api.Components
{
    public class GetToDosFromDBByFilter
    {
        internal const decimal HIGH_DOLLAR_VALUE_THRESHOLD = 500;

        private readonly InMemoryDBContext _dBContext;

        public enum FilterOptions
        {
            All = 0,
            Completed = 1,
            Incomplete = 2,
            Overdue = 4,
            HighDollarValue = 5,
        }

        public GetToDosFromDBByFilter(InMemoryDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        internal class Request
        {
            public FilterOptions filterOption { get; set; }
        }

        internal class Response
        {
            public IEnumerable<ToDo> ToDos { get; set; }
        }

        internal GetToDosFromDBByFilter.Response Execute(GetToDosFromDBByFilter.Request request)
        {
            var todos = _dBContext.ToDos.AsEnumerable();

            switch (request.filterOption)
            {
                case FilterOptions.Completed:
                    {
                        todos = todos.Where(t => t.completed.HasValue && t.completed.Value);
                        break;
                    }
                case FilterOptions.Incomplete:
                    {
                        todos = todos.Where(t => !(t.completed.HasValue && t.completed.Value));
                        break;
                    }
                case FilterOptions.Overdue:
                    {
                        todos = todos.Where(t => !(t.completed.HasValue && t.completed.Value)
                            && t.dueDate.HasValue
                            && t.dueDate < DateTime.Now);
                        break;
                    }
                case FilterOptions.HighDollarValue:
                    {
                        todos = todos.Where(t => t.dollarValue.HasValue && t.dollarValue >= HIGH_DOLLAR_VALUE_THRESHOLD);
                        break;
                    }
            }

            return new GetToDosFromDBByFilter.Response()
            {
                ToDos = todos,
            };
        }
    }
}