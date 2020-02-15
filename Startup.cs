using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using todo_api.Components;
using todo_api.Models;

namespace todo_api
{
    public class Startup
    {
        private IFileProvider _IFileProvider;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000");
                    builder.WithMethods(new string[] { "GET", "PUT", "POST", "DELETE" });
                    builder.AllowAnyHeader();
                });
            });

            _IFileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly(), "todo_api");
            services.AddSingleton<IFileProvider>(_IFileProvider);

            services.AddDbContext<InMemoryDBContext>(opt => opt.UseInMemoryDatabase("todoapi"));

            services.AddScoped<GetToDosFromDBByFilter>();
            services.AddScoped<GetToDoFromDBById>();
            services.AddScoped<AddToDoToDB>();
            services.AddScoped<UpdateToDoInDB>();
            services.AddScoped<DeleteToDoFromDB>();
            services.AddScoped<GetSummaryFromDB>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(MyAllowSpecificOrigins);

            SetUpInMemorySampleData(app);

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void SetUpInMemorySampleData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<InMemoryDBContext>();

                var sampleToDos = GetSampleToDos(app);

                context.ToDos.AddRange(sampleToDos);

                context.SaveChanges();
            }
        }

        private IEnumerable<ToDo> GetSampleToDos(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                //var embeddedProvider = serviceScope.ServiceProvider.GetService<EmbeddedFileProvider>();
                var file = _IFileProvider.GetDirectoryContents("").FirstOrDefault(fi => fi.Name == "SampleToDos.json");
                string json = string.Empty;

                if (file != null)
                {
                    using (var stream = file.CreateReadStream())
                    {
                        using (TextReader reader = new StreamReader(stream))
                        {
                            json = reader.ReadToEnd();
                        }
                    }
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ToDo>>(json);
            }
        }
    }
}
