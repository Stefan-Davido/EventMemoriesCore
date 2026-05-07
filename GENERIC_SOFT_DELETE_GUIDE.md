# Generic Soft Delete Filter Implementation

## Overview

The soft delete filter is now **fully generic** using reflection. Any entity that implements the `IIsDeleted` interface will automatically get the soft delete query filter applied, without needing to manually configure each entity.

## Implementation

### Updated EventMemoriesDbContext

```csharp
public class EventMemoriesDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventMemoriesDbContext).Assembly);

        // Apply query filter for soft delete to ALL entities implementing IIsDeleted
        ApplySoftDeleteFilter(modelBuilder);
    }

    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        var softDeleteInterfaces = typeof(IIsDeleted);
        var entityTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(et => softDeleteInterfaces.IsAssignableFrom(et.ClrType));

        foreach (var entityType in entityTypes)
        {
            var method = typeof(EventMemoriesDbContext)
                .GetMethod(nameof(ConfigureSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(entityType.ClrType);

            method?.Invoke(null, new object[] { modelBuilder });
        }
    }

    private static void ConfigureSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, IIsDeleted
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => !e.IsDeleted);
    }
}
```

## How It Works

### Step 1: Reflection Discovery
```csharp
var softDeleteInterfaces = typeof(IIsDeleted);
var entityTypes = modelBuilder.Model
    .GetEntityTypes()
    .Where(et => softDeleteInterfaces.IsAssignableFrom(et.ClrType));
```
- Finds all entity types in the model
- Filters for those implementing `IIsDeleted` interface

### Step 2: Dynamic Method Invocation
```csharp
var method = typeof(EventMemoriesDbContext)
    .GetMethod(nameof(ConfigureSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
    ?.MakeGenericMethod(entityType.ClrType);

method?.Invoke(null, new object[] { modelBuilder });
```
- Gets the generic `ConfigureSoftDeleteFilter<TEntity>` method
- Creates a version for each entity type
- Invokes it with the ModelBuilder

### Step 3: Apply Query Filter
```csharp
private static void ConfigureSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
    where TEntity : class, IIsDeleted
{
    modelBuilder.Entity<TEntity>()
        .HasQueryFilter(e => !e.IsDeleted);
}
```
- Generic method ensures type safety
- Applies the filter: `IsDeleted == false`
- Works for any entity implementing `IIsDeleted`

## Benefits of Generic Implementation

| Benefit | Description |
|---------|-------------|
| **Scalability** | Add soft delete to new entities just by implementing IIsDeleted |
| **DRY Principle** | No code duplication across entity configurations |
| **Maintainability** | Single location for soft delete logic |
| **Extensibility** | Future entities automatically get soft delete |
| **Consistency** | All soft-deletable entities behave the same way |
| **No Configuration** | No need to add query filters in Configurations folder |

## Adding Soft Delete to New Entities

Now, adding soft delete to any entity is as simple as:

### Step 1: Implement IIsDeleted
```csharp
public class Tenant : IIsDeleted
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }  // ← Add this line

    // ... other properties ...
}
```

### Step 2: Create Migration
```powershell
Add-Migration AddIsDeletedToTenant
Update-Database
```

That's it! The query filter will automatically apply.

### Step 3: Verify It Works
```csharp
// Soft delete
var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == id);
tenant.IsDeleted = true;
await context.SaveChangesAsync();

// Query will automatically exclude deleted tenants
var activeTenants = await context.Tenants.ToListAsync();  // ✓ Deleted tenant excluded

// Get all including deleted
var allTenants = await context.Tenants.IgnoreQueryFilters().ToListAsync();  // ✓ Includes deleted
```

## Current Implementation Status

### Currently Implementing IIsDeleted:
- ✅ Post

### Easy to Add:
- Tenant
- Event
- Info
- Configuration
- ApplicationUser

## Example: Adding Soft Delete to Event

### 1. Update Entity
```csharp
public class Event : IIsDeleted
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; }
    public Guid OwnerId { get; set; }
    public Guid TenantId { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime? EventDateEnd { get; set; }
    public string? Description { get; set; }
    public TenantSubscription Subscription { get; set; } = TenantSubscription.S;
    public bool IsDeleted { get; set; }  // ← Add this

    public ApplicationUser Owner { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Info> Infos { get; set; } = new List<Info>();
    public ICollection<Configuration> Configurations { get; set; } = new List<Configuration>();
}
```

### 2. Create Migration
```powershell
Add-Migration AddIsDeletedToEvent
Update-Database
```

### 3. Update Service (Optional)
```csharp
public async Task<bool> DeleteEventAsync(Guid id)
{
    var eventEntity = await _repository.GetByIdAsync(id);
    if (eventEntity == null)
        return false;

    eventEntity.IsDeleted = true;  // Soft delete
    await _repository.UpdateAsync(eventEntity);
    await _repository.SaveChangesAsync();
    return true;
}
```

## Advanced Usage

### Query Across Multiple Soft-Deleted Entities
```csharp
// Get all events for a tenant (excluding deleted)
var tenantEvents = await context.Events
    .Where(e => e.TenantId == tenantId)
    .ToListAsync();  // Automatically excludes IsDeleted = true

// Include deleted events
var allTenantEvents = await context.Events
    .IgnoreQueryFilters()
    .Where(e => e.TenantId == tenantId)
    .ToListAsync();

// Get only deleted events
var deletedEvents = await context.Events
    .IgnoreQueryFilters()
    .Where(e => e.IsDeleted)
    .ToListAsync();
```

### Restore Multiple Deleted Entities
```csharp
var deletedPosts = await context.Posts
    .IgnoreQueryFilters()
    .Where(p => p.IsDeleted)
    .ToListAsync();

foreach (var post in deletedPosts)
{
    post.IsDeleted = false;
}

await context.SaveChangesAsync();
```

## Testing

### Unit Test Example
```csharp
[Test]
public async Task SoftDeleteFilter_ExcludesDeletedPosts()
{
    // Arrange
    var post = new Post { Id = Guid.NewGuid(), IsDeleted = true };
    await context.Posts.AddAsync(post);
    await context.SaveChangesAsync();

    // Act
    var result = await context.Posts.ToListAsync();

    // Assert
    Assert.That(result, Does.Not.Contain(post));
}

[Test]
public async Task IgnoreQueryFilters_IncludesDeletedPosts()
{
    // Arrange
    var post = new Post { Id = Guid.NewGuid(), IsDeleted = true };
    await context.Posts.AddAsync(post);
    await context.SaveChangesAsync();

    // Act
    var result = await context.Posts.IgnoreQueryFilters().ToListAsync();

    // Assert
    Assert.That(result, Does.Contain(post));
}
```

## Performance Considerations

✓ **Efficient**: Query filters are translated to WHERE clauses in SQL
```sql
-- Generated SQL automatically includes:
SELECT * FROM Posts WHERE IsDeleted = 0
```

⚠️ **Watch Out**: 
- Don't use `IgnoreQueryFilters()` in production queries without reason
- Consider indexing `IsDeleted` column for large tables
- Monitor query performance with many soft-deleted records

## Best Practices

1. **Always use soft delete in services**
   ```csharp
   entity.IsDeleted = true;  // ✓ Preferred
   context.Entity.Remove(entity);  // ✗ Avoid
   ```

2. **Add IsDeleted index to high-volume tables**
   ```csharp
   modelBuilder.Entity<Post>()
       .HasIndex(p => p.IsDeleted);
   ```

3. **Consider CreatedAt/DeletedAt fields**
   ```csharp
   public DateTime? DeletedAt { get; set; }
   public Guid? DeletedBy { get; set; }
   ```

4. **Document soft delete behavior**
   - Add comments to entity classes
   - Document API behavior in Swagger
   - Train team on soft delete patterns

5. **Use IgnoreQueryFilters() carefully**
   - Only when specifically needed
   - Document why it's used
   - Test thoroughly

## Migration Path

```
Current State:
├── Post implements IIsDeleted ✓
└── Generic filter applied ✓

Next Steps:
├── Add IsDeleted to Tenant
├── Add IsDeleted to Event
├── Add IsDeleted to Info
├── Add IsDeleted to Configuration
└── Consider ApplicationUser soft delete

Result:
└── All entities support soft delete automatically
```

## Troubleshooting

### Q: Why isn't the filter being applied?
**A**: Check that the entity implements `IIsDeleted` interface and has the `IsDeleted` property.

### Q: How do I see the generated SQL?
**A**: Enable EF Core logging:
```csharp
.LogTo(Console.WriteLine, LogLevel.Information)
```

### Q: Can I disable the filter for specific queries?
**A**: Yes, use `IgnoreQueryFilters()`:
```csharp
var allRecords = await context.Posts
    .IgnoreQueryFilters()
    .ToListAsync();
```

### Q: What about navigation properties with soft-deleted entities?
**A**: The filter applies to direct queries. Use `.IgnoreQueryFilters()` to include them in relationships.

---

## Summary

✅ **Generic soft delete implementation complete**
✅ **Automatically applies to all IIsDeleted entities**
✅ **Scalable and maintainable**
✅ **Ready for extension to other entities**

**Status**: Production Ready 🚀
