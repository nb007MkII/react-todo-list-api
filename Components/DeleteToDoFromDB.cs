using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_api.Models;

namespace todo_api.Components
{
    public class DeleteToDoFromDB
    {
        private readonly InMemoryDBContext _dBContext;

        internal class Request
        {
            public int ToDoId { get; set; }
        }

        internal class Response
        {
            public bool Success { get; set; }
        }

        public DeleteToDoFromDB(InMemoryDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        internal DeleteToDoFromDB.Response Execute(DeleteToDoFromDB.Request request)
        {
            var dbToDo = _dBContext.ToDos.FirstOrDefault<ToDo>(t => t.todoid == request.ToDoId);

            if (dbToDo != null)
            {
                try
                {
                    _dBContext.ToDos.Remove(dbToDo);

                    _dBContext.SaveChanges();

                    return new DeleteToDoFromDB.Response()
                    {
                        Success = true,
                    };
                }
                catch (Exception ex)
                {
                    return new DeleteToDoFromDB.Response()
                    {
                        Success = false,
                    };
                }
            }

            return new DeleteToDoFromDB.Response()
            {
                Success = false,
            };
        }
    }
}