using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using todo_api.Models;

namespace todo_api
{
    public class InMemoryDBContext : DbContext
    {
        public InMemoryDBContext(DbContextOptions<InMemoryDBContext> options)
            : base(options)
        {
        }

        public DbSet<ToDo> ToDos { get; set; }
    }
}
