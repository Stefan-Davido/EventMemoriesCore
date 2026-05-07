# EventMemoriesCore - Quick Reference

## Project Structure

```
EventMemoriesCore/
├── EventMemories/                 # ASP.NET Core Web API
│   ├── Controllers/
│   │   ├── TenantController.cs
│   │   ├── EventController.cs
│   │   ├── PostController.cs
│   │   ├── InfoController.cs
│   │   ├── ConfigurationController.cs
│   │   └── UserController.cs
│   ├── Program.cs                 # DI & Configuration
│   └── appsettings.example.json
│
├── EventMemoriesServices/         # Business Logic Layer
│   ├── Services/
│   │   ├── TenantService.cs
│   │   ├── EventService.cs
│   │   ├── PostService.cs
│   │   ├── InfoService.cs
│   │   ├── ConfigurationService.cs
│   │   └── UserService.cs
│   └── DTOs/
│       ├── TenantDto.cs
│       ├── EventDto.cs
│       ├── PostDto.cs
│       ├── InfoDto.cs
│       ├── ConfigurationDto.cs
│       └── UserDto.cs
│
├── Dal/                           # Data Access Layer
│   ├── EventMemoriesDbContext.cs  # EF Core Context
│   ├── Configurations/            # Entity Mappings
│   │   ├── TenantConfiguration.cs
│   │   ├── EventConfiguration.cs
│   │   ├── PostConfiguration.cs
│   │   ├── InfoConfiguration.cs
│   │   └── ConfigurationConfiguration.cs
│   └── Repositories/              # Data Access
│       ├── IRepository.cs
│       ├── Repository.cs
│       ├── ITenantRepository.cs & TenantRepository.cs
│       ├── IEventRepository.cs & EventRepository.cs
│       ├── IPostRepository.cs & PostRepository.cs
│       ├── IInfoRepository.cs & InfoRepository.cs
│       ├── IConfigurationRepository.cs & ConfigurationRepository.cs
│       └── IUserRepository.cs & UserRepository.cs
│
└── DalEntities/                   # Domain Models
    ├── ApplicationUser.cs         # Custom Identity User
    ├── ApplicationRole.cs         # Custom Identity Role
    ├── Tenant.cs
    ├── Event.cs
    ├── Post.cs
    ├── Info.cs
    ├── Configuration.cs
    ├── TenantSubscriptionEnum.cs
    └── InfoLevelEnum.cs
```

## API Endpoints

### User Endpoints
```
POST   /api/user                   Create user (anonymous)
GET    /api/user                   Get all users
GET    /api/user/{id}              Get user by ID
GET    /api/user/email/{email}     Get user by email
PUT    /api/user/{id}              Update user
DELETE /api/user/{id}              Delete user
```

### Tenant Endpoints
```
POST   /api/tenant                 Create tenant
GET    /api/tenant                 Get all tenants
GET    /api/tenant/{id}            Get tenant by ID
GET    /api/tenant/owner/{ownerId} Get tenants by owner
PUT    /api/tenant/{id}            Update tenant
DELETE /api/tenant/{id}            Delete tenant
```

### Event Endpoints
```
POST   /api/event                  Create event
GET    /api/event                  Get all events
GET    /api/event/{id}             Get event by ID
GET    /api/event/tenant/{tenantId} Get events by tenant
GET    /api/event/owner/{ownerId}  Get events by owner
PUT    /api/event/{id}             Update event
DELETE /api/event/{id}             Delete event
```

### Post Endpoints
```
POST   /api/post                   Create post
GET    /api/post                   Get all posts
GET    /api/post/{id}              Get post by ID
GET    /api/post/event/{eventId}   Get posts by event
GET    /api/post/user/{userId}     Get posts by user
PUT    /api/post/{id}              Update post
DELETE /api/post/{id}              Delete post
```

### Info Endpoints
```
POST   /api/info                   Create info
GET    /api/info                   Get all infos
GET    /api/info/{id}              Get info by ID
GET    /api/info/event/{eventId}   Get infos by event
GET    /api/info/user/{userId}     Get infos by user
GET    /api/info/level/{level}     Get infos by level (1-5)
PUT    /api/info/{id}              Update info
DELETE /api/info/{id}              Delete info
```

### Configuration Endpoints
```
POST   /api/configuration          Create configuration
GET    /api/configuration          Get all configurations
GET    /api/configuration/{id}     Get configuration by ID
GET    /api/configuration/event/{eventId}     Get configs by event
GET    /api/configuration/event/{eventId}/name/{name}  Get config by name
PUT    /api/configuration/{id}     Update configuration
DELETE /api/configuration/{id}     Delete configuration
```

## Key Classes & Interfaces

### Entities
| Entity | Purpose | Key Properties |
|--------|---------|-----------------|
| ApplicationUser | Identity user | Id, Email, Posts, OwnedTenants |
| Tenant | Multi-tenant container | Id, Name, OwnerId, Events |
| Event | Event within tenant | Id, Name, TenantId, EventDate, Subscription |
| Post | Media upload | Id, EventId, UserId, MediaUrls (List) |
| Info | Event information | Id, EventId, UserId, Level (1-5), Text |
| Configuration | Event config | Id, EventId, Name, Value, NumberValue |

### Services
All services follow the pattern:
```csharp
public interface IXxxService
{
    Task<XxxDto?> GetXxxByIdAsync(Guid id);
    Task<IEnumerable<XxxDto>> GetAllXxxAsync();
    // Filter methods...
    Task<XxxDto> CreateXxxAsync(CreateXxxDto dto, ...);
    Task<XxxDto?> UpdateXxxAsync(Guid id, UpdateXxxDto dto);
    Task<bool> DeleteXxxAsync(Guid id);
}
```

## Typical Workflow

1. **Create User** (no auth required)
   ```
   POST /api/user
   {
     "userName": "john.doe",
     "email": "john@example.com",
     "password": "SecurePass123!",
     "phoneNumber": "+1234567890"
   }
   ```

2. **Login** (via Azure AD)
   - Get JWT token from Azure AD

3. **Create Tenant**
   ```
   POST /api/tenant (with JWT token)
   {
     "name": "My Family",
     "description": "Our family photos"
   }
   ```

4. **Create Event**
   ```
   POST /api/event (with JWT token)
   {
     "name": "Summer Vacation 2024",
     "tenantId": "{tenantId}",
     "eventDate": "2024-07-15",
     "eventDateEnd": "2024-07-22",
     "description": "Beach trip",
     "subscription": 2
   }
   ```

5. **Upload Post**
   ```
   POST /api/post (with JWT token)
   {
     "eventId": "{eventId}",
     "mediaUrls": [
       "https://storage.example.com/photo1.jpg",
       "https://storage.example.com/video1.mp4"
     ]
   }
   ```

6. **Add Info**
   ```
   POST /api/info (with JWT token)
   {
     "eventId": "{eventId}",
     "level": 3,
     "text": "Great moment!",
     "date": "2024-07-15T15:30:00Z"
   }
   ```

## Database Relationships

```
ApplicationUser (1) ←→ (∞) Tenant (Owner)
ApplicationUser (1) ←→ (∞) Event (Owner)
ApplicationUser (1) ←→ (∞) Post
ApplicationUser (1) ←→ (∞) Info

Tenant (1) ←→ (∞) Event
Event (1) ←→ (∞) Post
Event (1) ←→ (∞) Info
Event (1) ←→ (∞) Configuration
```

## Getting Started Checklist

- [ ] Update `appsettings.json` with SQL Server connection string
- [ ] Run `Add-Migration InitialCreate`
- [ ] Run `Update-Database`
- [ ] Configure Azure AD credentials in `appsettings.json`
- [ ] Test API endpoints with Swagger
- [ ] Implement authorization policies for tenant isolation
- [ ] Add logging and error handling middleware
- [ ] Deploy to production

## Common Patterns

### Extracting User ID from Token
```csharp
var userIdString = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
if (!Guid.TryParse(userIdString, out var userId))
    return BadRequest("Unable to identify user.");
```

### Using Services
```csharp
var tenant = await _tenantService.GetTenantByIdAsync(id);
if (tenant == null)
    return NotFound();
```

### Creating Resources
```csharp
var tenant = await _tenantService.CreateTenantAsync(dto, userId);
return CreatedAtAction(nameof(GetTenantById), new { id = tenant.Id }, tenant);
```

---
**Build Status**: ✓ Successful - Ready for development!
