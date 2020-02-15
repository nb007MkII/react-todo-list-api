using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_api.Models;

namespace todo_api.Components
{
    public class UpdateToDoInDB
    {
        private readonly InMemoryDBContext _dBContext;

        internal class Request
        {
            public ToDo ToDo { get; set; }
        }

        internal class Response
        {
            public bool Success { get; set; }
        }

        public UpdateToDoInDB(InMemoryDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        internal UpdateToDoInDB.Response Execute(UpdateToDoInDB.Request request)
        {
            var dbToDo = _dBContext.ToDos.FirstOrDefault<ToDo>(t => t.todoid == request.ToDo.todoid);

            if (dbToDo != null)
            {
                dbToDo.description = request.ToDo.description;
                dbToDo.dueDate = request.ToDo.dueDate;
                dbToDo.dollarValue = request.ToDo.dollarValue;
                dbToDo.notes = request.ToDo.notes;
                dbToDo.completed = request.ToDo.completed;
                dbToDo.color = request.ToDo.color;

                try
                {
                    _dBContext.SaveChanges();

                    return new UpdateToDoInDB.Response()
                    {
                        Success = true,
                    };
                }
                catch (Exception ex)
                {
                    return new UpdateToDoInDB.Response()
                    {
                        Success = false,
                    };
                }
            }

            return new UpdateToDoInDB.Response()
            {
                Success = false,
            };
        }
    }
}