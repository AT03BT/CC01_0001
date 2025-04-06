README.md
Version: 1.0.0
(c) 2024, Minh Tri Tran, with assistance from Google's Gemini - Licensed under CC BY 4.0
https://creativecommons.org/licenses/by/4.0/


HISTORY
===================================


STEP 1
------
dotnet add package Microsoft.EntityFrameworkCore.Sqlite -v 8.*
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design -v 8.*
dotnet add package Microsoft.EntityFrameworkCore.Design -v 8.*

dotnet add package Microsoft.AspNetCore.Identity.UI -v 8.*
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore -v 8.*
dotnet aspnet-codegenerator identity -dc CC01_0001.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout;Account.Manage.Index;Account.Manage.ChangePassword;Account.Manage.Email;Account.Manage.TwoFactorAuthentication;Account.Manage.ExternalLogins"

dotnet ef migrations add InitialCreate
dotnet ef database update
