Database First Command
-> dotnet ef dbcontext scaffold "Name=ConnectionStrings:Mssql" Microsoft.EntityFrameworkCore.SqlServer  --context-dir Data --output-dir Models --data-annotations --force