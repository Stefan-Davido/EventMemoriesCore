# Soft Delete Implementation Guide

## Overview

Soft delete functionality has been implemented using:
- **IIsDeleted Interface** - Common interface for entities supporting soft delete
- **Query Filter** - Automatic filtering at DbContext level
- **Post Entity** - Now implements IIsDeleted for soft delete support

## What Changed

### 1. New Interface: IIsDeleted
**File**: `DalEntities/IIsDeleted.cs`

```csharp
public interface IIsDeleted
{
    bool IsDeleted { get; set; }
}
```

**Purpose**: Define the contract for entities that support soft delete.

### 2. Updated Post Entity
**File**: `DalEntities/Post.cs`

```csharp
public class Post : IIsDeleted
{
    // ... existing properties ...
    public bool IsDeleted { get; set; }
}
```

### 3. Updated DbContext with Query Filter
**File**: `Dal/EventMemoriesDbContext.cs`

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Apply entity configurations
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventMemoriesDbContext).Assembly);

    // Add query filter for soft delete
    modelBuilder.Entity<Post>()
        .HasQueryFilter(p => !p.IsDeleted);
}
```

## How It Works

### Soft Delete (Instead of Hard Delete)
When you delete a Post, set `IsDeleted = true` instead of removing from database:

```csharp
public async Task<bool> DeletePostAsync(Guid id)
{
    var post = await _repository.GetByIdAsync(id);
    if (post == null)
        return false;

    post.IsDeleted = true;  // Soft delete
    await _repository.UpdateAsync(post);
    await _repository.SaveChangesAsync();
    return true;
}
```

### Automatic Filtering
The query filter automatically excludes soft-deleted items:

```csharp
// This will NOT return deleted posts
var posts = await _dbSet.ToListAsync();  
// WHERE IsDeleted = 0 is automatically added
```

### Query Deleted Items (If Needed)
Use `IgnoreQueryFilters()` to bypass the filter:

```csharp
// Get ALL posts, including deleted ones
var allPosts = await _dbSet.IgnoreQueryFilters().ToListAsync();

// Get only deleted posts
var deletedPosts = await _dbSet.IgnoreQueryFilters()
    .Where(p => p.IsDeleted)
    .ToListAsync();
```

## Database Migration

After implementing soft delete, run migration:

```powershell
Add-Migration AddIsDeletedToPost
Update-Database
```

This will add the `IsDeleted` column to the `Posts` table with default value `false`.

## Benefits

✓ **Data Recovery** - Deleted posts can be recovered
✓ **Audit Trail** - Historical data remains for compliance
✓ **Referential Integrity** - Foreign keys remain valid
✓ **Transparent Filtering** - Query filters work automatically
✓ **Performance** - No cascading deletes needed

## Future Implementation

To add soft delete to other entities:

### Step 1: Make Entity Implement IIsDeleted
```csharp
public class Tenant : IIsDeleted
{
    // ... existing properties ...
    public bool IsDeleted { get; set; }
}
```

### Step 2: Add Query Filter in DbContext
```csharp
modelBuilder.Entity<Tenant>()
    .HasQueryFilter(t => !t.IsDeleted);
```

### Step 3: Create and Apply Migration
```powershell
Add-Migration AddIsDeletedToTenant
Update-Database
```

## Example Usage in Controller

```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> DeletePost(Guid id)
{
    var post = await _repository.GetByIdAsync(id);
    if (post == null)
        return NotFound();

    post.IsDeleted = true;
    await _repository.UpdateAsync(post);
    await _repository.SaveChangesAsync();
    return NoContent();
}

[HttpPost("{id}/restore")]
public async Task<IActionResult> RestorePost(Guid id)
{
    // Use IgnoreQueryFilters to get deleted post
    var post = await _context.Posts
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(p => p.Id == id);

    if (post == null)
        return NotFound();

    post.IsDeleted = false;
    await _repository.UpdateAsync(post);
    await _repository.SaveChangesAsync();
    return Ok(post);
}
```

## Service Layer Updates

Update `PostService.DeletePostAsync`:

```csharp
public async Task<bool> DeletePostAsync(Guid id)
{
    var post = await _repository.GetByIdAsync(id);
    if (post == null)
        return false;

    post.IsDeleted = true;
    await _repository.UpdateAsync(post);
    await _repository.SaveChangesAsync();
    return true;
}
```

## Important Notes

⚠️ **Query Filters Have Limitations**:
- Only apply to queries directly on DbSet
- Don't apply to `Find()` method
- Don't apply to `FromSqlInterpolated()` queries
- Must explicitly use `IgnoreQueryFilters()` when needed

⚠️ **Always Test**:
- Verify soft-deleted items are not returned by queries
- Test `IgnoreQueryFilters()` functionality
- Verify relationships still work with soft delete

## Testing Soft Delete

```csharp
[Test]
public async Task DeletePost_ShouldSoftDelete()
{
    var post = new Post { Id = Guid.NewGuid(), IsDeleted = false };
    await context.Posts.AddAsync(post);
    await context.SaveChangesAsync();

    // Soft delete
    post.IsDeleted = true;
    context.Posts.Update(post);
    await context.SaveChangesAsync();

    // Query filter should exclude it
    var posts = await context.Posts.ToListAsync();
    Assert.That(posts, Does.Not.Contain(post));

    // IgnoreQueryFilters should include it
    var allPosts = await context.Posts.IgnoreQueryFilters().ToListAsync();
    Assert.That(allPosts, Does.Contain(post));
}
```

---

**Next Steps**: 
1. Run migration: `Add-Migration AddIsDeletedToPost`
2. Update database: `Update-Database`
3. Update PostService if needed
4. Consider adding soft delete to other entities

**Status**: ✓ Implementation Complete
