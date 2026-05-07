# Migration Guide for EventMemoriesCore

## Before Running Migrations

1. **Update Connection String** in `EventMemories/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=EventMemoriesDb;Trusted_Connection=true;Encrypt=false;"
     }
   }
   ```

   Replace:
   - `YOUR_SERVER` with your SQL Server instance (e.g., `localhost`, `(local)`, `DESKTOP-ABC123\SQLEXPRESS`)
   - `EventMemoriesDb` with your desired database name (optional)

2. **Ensure SQL Server is running**

## Option 1: Using Package Manager Console (PMC)

### Step 1: Open Package Manager Console
- In Visual Studio: Tools → NuGet Package Manager → Package Manager Console

### Step 2: Set Default Project to "Dal"
```
Select "Dal" from the "Default project" dropdown in PMC
```

### Step 3: Create Initial Migration
```powershell
Add-Migration InitialCreate
```

This creates a migration file in `Dal/Migrations/` directory.

### Step 4: Update Database
```powershell
Update-Database
```

This creates the database and all tables.

### Verify Success
You should see messages like:
```
Build started...
Build succeeded.
Run-time version of the database
Applying migration '20240XXX000000_InitialCreate'.
Done.
```

## Option 2: Using .NET CLI (Command Line)

### Step 1: Open Command Prompt or PowerShell
Navigate to the solution root directory.

### Step 2: Create Initial Migration
```bash
dotnet ef migrations add InitialCreate --project Dal
```

### Step 3: Update Database
```bash
dotnet ef database update --project Dal
```

### Verify Success
Check that the database was created in SQL Server.

## Troubleshooting

### Issue: "No parameterless constructor found"
**Solution**: Ensure DbContext has a constructor that accepts `DbContextOptions<EventMemoriesDbContext>`.

### Issue: "Metadata file could not be found"
**Solution**: 
1. Clean the solution (Build → Clean Solution)
2. Rebuild the solution (Build → Rebuild Solution)
3. Try migrations again

### Issue: "Database connection failed"
**Solution**:
1. Check that SQL Server is running
2. Verify connection string is correct
3. Check Windows Authentication or SQL Authentication credentials
4. Ensure firewall allows SQL Server connections

### Issue: "A network-related or instance-specific error occurred"
**Solution**:
1. SQL Server instance name might be incorrect
2. Try using `(local)` or `.` instead of `localhost`
3. Check SQL Server Configuration Manager for service status

### Issue: "Login failed for user"
**Solution**:
1. If using SQL Authentication, verify username/password
2. If using Windows Authentication, ensure your user account has access
3. Create a SQL Server login if needed

## After Migration Success

### Database Tables Created
- `AspNetUsers` (ApplicationUser)
- `AspNetRoles` (ApplicationRole)
- `AspNetUserRoles`, `AspNetUserClaims`, `AspNetUserLogins` (Identity tables)
- `Tenants`
- `Events`
- `Posts`
- `Infos`
- `Configurations`

### Test Connection
You can verify the database was created by:
1. Opening SQL Server Management Studio (SSMS)
2. Connecting to your server
3. Looking for `EventMemoriesDb` database
4. Expanding it to see the tables

## Adding Future Migrations

When you modify entity models:

### Step 1: Make changes to entity classes
Example: Add a new property to the `Tenant` class

### Step 2: Create migration
```powershell
Add-Migration AddTenantProperty
```

### Step 3: Apply migration
```powershell
Update-Database
```

## Removing Migrations

If you need to undo a migration before applying to database:

### Step 1: Remove the migration
```powershell
Remove-Migration
```

### Step 2: Revert database if already applied
```powershell
Update-Database -Migration PreviousMigrationName
```

## Rollback Entire Database

To remove all tables and start over:

```powershell
Update-Database -Migration 0
```

Or delete the database manually in SSMS and run `Update-Database` again.

## Seed Initial Data (Optional)

After migration, you may want to seed test data. Add this to `Program.cs`:

```csharp
// After app.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventMemoriesDbContext>();
    // Add seed data here
}
```

## Important Notes

⚠️ **Production Considerations**:
- Always backup database before migrations
- Test migrations on staging environment first
- Keep migration scripts for audit trail
- Document migration steps in deployment guides
- Consider using Identity Server or Azure AD for user management
- Implement data encryption for sensitive fields
- Set up indexes on frequently queried fields

## Quick Reference Commands

```powershell
# Create migration
Add-Migration MigrationName

# Apply migration
Update-Database

# Remove last migration
Remove-Migration

# List migrations
Get-Migration

# Script migration (generates SQL)
Script-Migration

# Generate SQL script to file
Script-Migration -OutputScript > migration.sql

# Update to specific migration
Update-Database -Migration MigrationName

# Rollback all
Update-Database -Migration 0
```

---

**Next Step**: Once migrations are applied, you can start testing the API endpoints!
