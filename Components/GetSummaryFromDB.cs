using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_api.Models;

namespace todo_api.Components
{
    public class GetSummaryFromDB
    {
        private GetToDosFromDBByFilter _GetToDosFromDBByFilter;

        internal class Request
        {
            public IEnumerable<GetToDosFromDBByFilter.FilterOptions> filterOptions { get; set; }
        }

        internal class Response
        {
            internal Response()
            {
                Summaries = new List<Summary>();
            }
            public List<Summary> Summaries { get; set; }
        }

        public GetSummaryFromDB(GetToDosFromDBByFilter getToDosFromDBByFilter)
        {
            _GetToDosFromDBByFilter = getToDosFromDBByFilter;
        }

        internal GetSummaryFromDB.Response Execute(GetSummaryFromDB.Request request)
        {
            var response = new GetSummaryFromDB.Response();

            foreach (GetToDosFromDBByFilter.FilterOptions filterOption in request.filterOptions)
            {
                var todos = _GetToDosFromDBByFilter.Execute(new GetToDosFromDBByFilter.Request() { filterOption = filterOption });

                response.Summaries.Add(new Summary() { FilterOption = filterOption, totalToDos = todos.ToDos.Count() });
            }

            return response;
        }
    }
}