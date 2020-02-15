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
    public class SummaryController : ControllerBase
    {
        private readonly GetSummaryFromDB _GetSummaryFromDB;

        public SummaryController(GetSummaryFromDB getSummaryFromDB)
        {
            _GetSummaryFromDB = getSummaryFromDB;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Summary>>> Get()
        {
            var allFilterOptions = (IList<GetToDosFromDBByFilter.FilterOptions>)Enum.GetValues(typeof(GetToDosFromDBByFilter.FilterOptions));

            var getSummariesResult = _GetSummaryFromDB.Execute(new GetSummaryFromDB.Request() { filterOptions = allFilterOptions });

            return new OkObjectResult(getSummariesResult.Summaries);
        }
    }
}
