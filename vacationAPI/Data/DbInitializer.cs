using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VacationAPI.Models;
using VacationAPI.Services;

namespace VacationAPI.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    //Check if the database has already been seeded
                    if (await context.Users.AnyAsync())
                    {
                        return;
                    }

                    var userService = services.GetRequiredService<IUserService>();
                    var vacationRequestService = services.GetRequiredService<IVacationRequestService>();
                    var calendarificApiService = services.GetRequiredService<ICalendarificApiService>();

                    
                    // Seed users
                    await userService.CreateUser("John", "Doe", "johndoe", "password", "US", "User", 9, 17);
                    await userService.CreateUser("Jane", "Doe", "janedoe", "password", "US", "User", 9, 17);
                    await userService.CreateUser("Madalin", "Frincu", "madalinfr", "password", "RO", "Admin", 9, 17);


                    var johnDoe = await userService.GetByUsername("johndoe");
                    var janeDoe = await userService.GetByUsername("janedoe");
                    var madalinFr = await userService.GetByUsername("madalinfr");

                    // Seed vacation requests for John Doe
                    if (johnDoe != null)
                    {
                        await vacationRequestService.CreateVacationRequest(johnDoe.UserName, new DateTime(2022, 1, 15), new DateTime(2022, 1, 20));
                        await vacationRequestService.CreateVacationRequest(johnDoe.UserName, new DateTime(2022, 4, 11), new DateTime(2022, 4, 13));
                        await vacationRequestService.CreateVacationRequest(johnDoe.UserName, new DateTime(2022, 8, 5), new DateTime(2022, 8, 13), comment: "Summer holiday vacation");
                    }
                    // Seed vacation requests for Jane Doe
                    if (janeDoe != null)
                    {

                        await vacationRequestService.CreateVacationRequest(janeDoe.UserName, new DateTime(2023, 3, 25), new DateTime(2023, 4, 1), comment: "Going on a camping trip with friends");
                        await vacationRequestService.CreateVacationRequest(janeDoe.UserName, new DateTime(2023, 5, 10), new DateTime(2023, 5, 14), comment: "Taking a mental health break");
                    }
                
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}