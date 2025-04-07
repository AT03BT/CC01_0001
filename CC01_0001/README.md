README.md
Version: 1.0.0
(c) 2024, Minh Tri Tran, with assistance from Google's Gemini - Licensed under CC BY 4.0
https://creativecommons.org/licenses/by/4.0/

# CC01_0001 - Data Analytics and Visualization
==============================================

MARKET OVERVIEW
                        |
    +-------------------+-------------------+
    |                   |                   |
 SEGMENT A          SEGMENT B          SEGMENT C
    |                   |                   |
+---+---+           +---+---+           +---+---+
|       |           |       |           |       |
COMP1   COMP2     COMP3   COMP4      COMP5   COMP6
|   |   |   |     |   |   |   |      |   |   |   |
$   $   $   $     $   $   $   $      $   $   $   $

ANALYTICS DASHBOARD
+----------------+
|  KPI METRICS   |
| Revenue: $$$   |
| Growth: ↑25%   |
| Share:  35%    |
+----------------+
       |
   DECISION TREE
       |
    [Y/N]─────┐
      |       |
  ACTION 1  ACTION 2
      |       |
   IMPACT   IMPACT
    [↑↓]     [↑↓]


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
 
