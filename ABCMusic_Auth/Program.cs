using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using hanna80_ABCMusic_Auth.Data;
using Microsoft.EntityFrameworkCore;

namespace hanna80_ABCMusic_Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var host = BuildWebHost(args);

			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;

				// use dependency injection to retrieve a database context
				using (var context = services.GetRequiredService<AngelicBeatsDbContext>())
				{
					// perform migrations to the database automatically
					context.Database.Migrate();

					// seed the database if empty
					AngelicBeatsDbInitializer.Initialize(context);
				}
			}

			host.Run();
		}

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
