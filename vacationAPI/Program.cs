using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationAPI.Data;
using VacationAPI.Repositories;
using VacationAPI.Services;

namespace VacationAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userService = services.GetRequiredService<IUserService>();
                    var vacationRequestService = services.GetRequiredService<IVacationRequestService>();
                    var calendarificApiService = services.GetRequiredService<ICalendarificApiService>();
                    var vacationRequestRepository = services.GetRequiredService<IVacationRequestRepository>();

                    var context = services.GetRequiredService<ApplicationDbContext>();
                    await context.Database.MigrateAsync();

                    // Seed the database with initial data
                    await DbInitializer.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<Startup>();
              });
    }
}