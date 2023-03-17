# VacationAPI

VacationAPI is a RESTful web API built with ASP.NET Core that allows users to submit vacation requests and manage their vacation schedules.

## Getting Started

To get started with VacationAPI, you can follow these steps:

1. Clone the repository to your local machine.
2. Open the solution in Visual Studio.
3. Build and run the application.
4. Use a tool like Postman to interact with the API endpoints. Or use Swagger for more user-friendly approach.

## API Documentation

The API documentation is available through Swagger. You can access the Swagger UI by navigating to `/swagger` in your browser.

## Authentication

VacationAPI uses JWT authentication. To authenticate, you must first create an account by sending a `POST` request to the `/api/users/register` endpoint. You can then send a `POST` request to the `/api/users/login` endpoint to obtain a JWT token. You must include this token in the `Authorization` header of your subsequent API requests.

## Endpoints

Here's a list of the available API endpoints:


### POST

- `POST /api/auth/login` `Anonymous`    Authenticates a user and returns a JWT token for authorization on API endpoints 
- `POST /api/VacationRequests` `User`  Allows a user to create a new vacation request.
- `POST: api/Users/register` `Anonymous` Registers a new user in the system.

### GET

- `GET /api/auth/checkauth` `Anonymous`  Checks if the user is authenticated via a Bearer JWT token and returns the username and role if the user is authorized.
- `GET /api/VacationRequests/{username}` `Admin` `User` Returns a list of vacation requests made by a user with the specified username.
- `GET /api/VacationRequests` `Admin` Admin only. Returns a list of all vacation requests.
- `GET /api/VacationRequests/holidays` `Admin` `User`  Retrieves a list of national holidays for the current user's country.
- `GET /api/VacationRequests/{username}/vacationdays` `Admin` `User` - Returns the number of available vacation days for a given year based on the user's vacation history.
- `GET: api/User` `Anonymous` Shows a list of all users in the system.



### PUT

- `PUT /api/VacationRequests/{requestId}` `User`  Allows a user admin to update a vacation request.
- `PUT /api/VacationRequests/{requestId}/approve` `Admin` Approves a vacation request with the specified ID.
- `PUT /api/VacationRequests/{requestId}/reject` `Admin` Rejects a vacation request with the specified ID.

### DELETE

- `DELETE /api/VacationRequests/{requestId}` `User` Deletes a vacation request with the specified ID. Only authenticated users with the "User" role can access this endpoint.
- `DELETE: api/Users/{username}` `Anonymous`  Deletes a user with the specified username from the system (but queries password)


## Usage

We recommend Swagger because it has user-friendly interface when doing API calls. First get your token and Authorize yourself.


![image](https://user-images.githubusercontent.com/19687103/225907519-99329d24-485c-47a0-abf8-22c3fd66213b.png)



## Dependencies

Here's a list of the main dependencies used in this project:

- `Microsoft.EntityFrameworkCore`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.OpenApi.Models`
- `AutoMapper`

## Testing

The VacationAPI project includes comprehensive testing to ensure its functionality and reliability. The unit tests have been written using NUnit, a widely-used testing framework for .NET applications that provides a range of features for writing and executing unit tests. In addition, the project also utilizes Moq, a popular mocking library that enables the creation of mock objects for testing.

The tests cover a wide range of scenarios, including validating input data, testing the API endpoints, and verifying the system's behavior under various conditions. These tests have been designed to run automatically as part of the build process and can be easily executed using the NUnit Test Runner. Through this thorough testing process, the VacationAPI project ensures a high level of quality and robustness.


## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

Of course, feel free to customize the readme to fit the specific needs of your project. Let me know if you have any questions or if you need any help!
