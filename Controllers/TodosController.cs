using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using todo_api.Components;
using todo_api.Models;

namespace todo_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDosController : ControllerBase
    {
        private readonly GetToDosFromDBByFilter _GetToDosFromDBByFilter;
        private readonly GetToDoFromDBById _GetToDoFromDBById;
        private readonly AddToDoToDB _AddToDoToDB;
        private readonly UpdateToDoInDB _UpdateToDoInDB;
        private readonly DeleteToDoFromDB _DeleteToDoFromDB;
        private readonly GetSummaryFromDB _GetSummaryFromDB;

        public ToDosController(GetToDosFromDBByFilter getToDosFromDBByFilter,
            GetToDoFromDBById getToDoFromDBById,
            AddToDoToDB addToDoToDB,
            UpdateToDoInDB updateToDoInDB,
            DeleteToDoFromDB deleteToDoFromDB,
            GetSummaryFromDB getSummaryFromDB)
        {
            _GetToDosFromDBByFilter = getToDosFromDBByFilter;
            _GetToDoFromDBById = getToDoFromDBById;
            _AddToDoToDB = addToDoToDB;
            _UpdateToDoInDB = updateToDoInDB;
            _DeleteToDoFromDB = deleteToDoFromDB;
            _GetSummaryFromDB = getSummaryFromDB;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<GetToDosResult> Get()
        {
            GetToDosFromDBByFilter.FilterOptions filterOption, parseFilterOption;

            filterOption = GetToDosFromDBByFilter.FilterOptions.All;

            if (Request.Query.ContainsKey("filteroption")
                && Enum.TryParse<GetToDosFromDBByFilter.FilterOptions>(Request.Query["filteroption"], out parseFilterOption))
            {
                filterOption = parseFilterOption;
            }

            var getTodosResult = _GetToDosFromDBByFilter.Execute(new GetToDosFromDBByFilter.Request() { filterOption = filterOption });

            // jump through lots of hoops to deal with Enum.GetValues returning an Array and not an array.
            var otherFilterOptions = new List<GetToDosFromDBByFilter.FilterOptions>((IList<GetToDosFromDBByFilter.FilterOptions>)Enum.GetValues(typeof(GetToDosFromDBByFilter.FilterOptions)));

            otherFilterOptions.Remove(filterOption);

            var getSummariesResult = _GetSummaryFromDB.Execute(new GetSummaryFromDB.Request() { filterOptions = otherFilterOptions });

            if (getTodosResult?.ToDos != null)
            {
                getSummariesResult.Summaries.Add(new Summary() { FilterOption = filterOption, totalToDos = getTodosResult.ToDos.Count() });
                return new OkObjectResult(new GetToDosResult()
                {
                    FilterOption = filterOption, 
                    ToDos = getTodosResult.ToDos,
                    Summaries = getSummariesResult.Summaries
                });
            }

            return StatusCode((int)Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<ToDo> Get(int id)
        {
            var getToDoResult = _GetToDoFromDBById.Execute(new GetToDoFromDBById.Request() { ToDoId = id, });

            if (getToDoResult?.ToDo == null)
                return NotFound();

            return Ok(getToDoResult.ToDo);
        }

        // POST api/values
        [HttpPost]
        public ActionResult<ToDo> Post([FromBody] ToDo addThisToDo)
        {
            if (addThisToDo == null 
                || addThisToDo.todoid != null)
                return BadRequest();

            if (!OKToSaveToDo(addThisToDo))
                return BadRequest();

            var addNewToDoResult = _AddToDoToDB.Execute(new AddToDoToDB.Request() { ToDo = addThisToDo, });

            if (addNewToDoResult.Success 
                && addNewToDoResult.NewToDoId.HasValue)
            {
                var addedToTo = _GetToDoFromDBById.Execute(new GetToDoFromDBById.Request() { ToDoId = addNewToDoResult.NewToDoId.Value }).ToDo;
                return Ok(addThisToDo);
            }

            return StatusCode((int)Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
        }

        // PUT api/values/5
        [HttpPut()]
        public ActionResult<ToDo> Put([FromBody] ToDo updatedToDo)
        {
            if (updatedToDo == null)
                return BadRequest();

            if (!updatedToDo.todoid.HasValue)
                return BadRequest();

            if (!OKToSaveToDo(updatedToDo))
                return BadRequest();

            var updateToDoResult = _UpdateToDoInDB.Execute(new UpdateToDoInDB.Request() { ToDo = updatedToDo, });

            if (updateToDoResult.Success)
            {
                var dbUpdatedToDo = _GetToDoFromDBById.Execute(new GetToDoFromDBById.Request() { ToDoId = updatedToDo.todoid.Value }).ToDo;

                return Ok(dbUpdatedToDo);
            }

            return StatusCode((int)Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
        }

        // DELETE api/values/5
        [HttpDelete("{deletedToDoId}")]
        public ActionResult Delete(int deletedToDoId)
        {
            var deleteToDoResult = _DeleteToDoFromDB.Execute(new DeleteToDoFromDB.Request() { ToDoId = deletedToDoId, });

            if (deleteToDoResult.Success)
            {
                return Ok();
            }

            return StatusCode((int)Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
        }

        private bool OKToSaveToDo(ToDo todo)
        {
            return !(todo == null
                || string.IsNullOrWhiteSpace(todo?.description));
        }
    }
}
