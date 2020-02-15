using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_api.Models;

namespace todo_api.Components
{
    public class GetToDoFromDBById
    {
        private readonly InMemoryDBContext _dBContext;

        internal class Request
        {
            public int ToDoId { get; set; }
        }

        internal class Response
        {
            public ToDo ToDo { get; set; }
        }

        public GetToDoFromDBById(InMemoryDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        internal GetToDoFromDBById.Response Execute(GetToDoFromDBById.Request request)
        {
            return new GetToDoFromDBById.Response()
            {
                ToDo = _dBContext.ToDos.FirstOrDefault(t => t.todoid == request.ToDoId),
            };
        }
    }
}