# RealDox-2.0
A sample Web Api application, made on top of ASP.NET Core, EF Core, and using authentication through JWT.
Besides has the implementation of Repository pattern and 
# How to Use
1. Restore dependencies: dotnet restore
2. Adding migrations for both DbContexts:
  dotnet ef migrations add Initial-Scheme -c SecurityDbContext -o Data/Migrations/IdentityMigrations
  dotnet ef migrations add Initial-Scheme -c ToDoContext -o Data/Migrations/DbMigrations
3. Committing changes to database store.
  dotnet ef database update -c SecurityDbContext
  dotnet ef database update -c ToDoContext
  
 3. dotnet run

# How to test
1. Go to: http://localhost:5000/swagger and register a user using the api endpoint Authentication/register
2. Go to: http://localhost:5000/Home/Index and login with the previous registered user, and then try to get details about that user using the button bellow the login form.
