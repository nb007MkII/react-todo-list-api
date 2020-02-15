using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_api.Models;

namespace todo_api.Components
{
    public class AddToDoToDB
    {
        private readonly InMemoryDBContext _dBContext;

        internal class Request
        {
            public ToDo ToDo { get; set; }
        }

        internal class Response
        {
            public bool Success { get; set; }
            public int? NewToDoId { get; set; }
        }

        public AddToDoToDB(InMemoryDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        internal AddToDoToDB.Response Execute(AddToDoToDB.Request request)
        {
            // the data store is an in-memory EF database and has does not issue new identity keys. calculate new id here.
            var nextToDoID = (_dBContext.ToDos.Max<ToDo>(t => t.todoid).Value + 1); // not safe if there are no ToDos in the table

            request.ToDo.todoid = nextToDoID;

            _dBContext.ToDos.Add(request.ToDo);

            try
            {
                _dBContext.SaveChanges();

                return new AddToDoToDB.Response()
                {
                    Success = true,
                    NewToDoId = request.ToDo.todoid,
                };
            }
            catch (Exception ex)
            {
                return new AddToDoToDB.Response()
                {
                    Success = false,
                };
            }
        }
    }
}