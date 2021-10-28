# EFCoreSqlServerToOracle
Demonstrates using SqlServer and Oracle same time or migrate.

You can create dbcontext and models from terminal screen with fallowing commands.
```powershell
$constr = "data source=(localdb)\MsSqlLocalDb;initial catalog=testdb;Integrated Security=SSPI;persist security info=true;attachdbfilename=" + $pwd.Path + "\testdb.mdf"
dotnet ef dbcontext scaffold $constr Microsoft.EntityFrameworkCore.SqlServer -o Model\Tables --no-build -c TestDbContext --context-dir Model -v -f -d --use-database-names --no-pluralize 
```
