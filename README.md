# VirtualNursery

A multi-user virtual plant management application built with **C# and .NET 8**.

VirtualNursery lets registered users build and manage a personal collection of plants through a console client backed by a RESTful API. The shared `Nursery.Core` library separates all storage and domain logic from both the CLI and API surface, keeping each layer independent.

## Tech Stack

- **Language:** C# / .NET 8
- **API framework:** ASP.NET Core Web API
- **Database:** SQLite via `Microsoft.Data.Sqlite`
- **IDE:** JetBrains Rider

## Project Structure
```
VirtualNursery/
├── Nursery.Core/                     # Shared class library
│   ├── Common/
│   │   ├── LifeCycleType.cs
│   │   ├── PlantType.cs
│   │   └── UserType.cs
│   ├── Infrastructure/
│   │   ├── DataPaths.cs
│   │   ├── DatabaseServices.cs
│   │   ├── PlantDatabaseServices.cs
│   │   ├── UserDatabaseServices.cs
│   │   └── UserIDGenerator.cs
│   ├── Models/
│   │   ├── Plant.cs
│   │   ├── User.cs
│   │   └── UserSession.cs
│   └── Nursery.Core.csproj
├── Nursery.Clientlogin/              # Console client
│   ├── Infrastructure/
│   │   └── NurseryApiClient.cs
│   ├── PresentationLayer/
│   │   └── Menus/
│   │       ├── LoginMenu.cs
│   │       └── PlantMenu.cs
│   ├── Services/
│   │   └── AuthServices.cs
│   ├── Program.cs
│   └── Nursery.Clientlogin.csproj
├── Nursery.Api/                      # ASP.NET Core Web API
│   ├── Controllers/
│   │   └── PlantsController.cs
│   ├── Program.cs
│   └── Nursery.Api.csproj
├── data/
│   └── nursery.db                    # Shared SQLite database
└── Nursery.sln
```

## Features

- User registration with SHA-256 password hashing
- User login with credential validation
- Duplicate username detection — stops at username entry, before password
- Plant catalog — each unique plant name maps to exactly one `PlantID`
- Per-user nursery via junction table (`UserNursery`)
- Duplicate plant detection — stops immediately if a plant is already in your nursery; reuses existing catalog data if another user registered it first
- Console client routes all plant operations through the REST API — `Nursery.Api` is the single point of access to the database
- Input validation on plant type and life cycle — reprompts on invalid input instead of crashing

## Database Schema

Three normalised tables stored in a single shared `nursery.db` file:

```sql
Users      (UserID PK, Username UNIQUE, PasswordHash, Role)
Plant      (PlantID PK, Name UNIQUE, Type, LifeCycle, FloweringStatus)
UserNursery(UserID FK, PlantID FK)  -- composite PK
```

**Find-or-create pattern:** when a user adds a plant by name, the service checks the shared `Plant` catalogue first. If a matching entry exists its `PlantID` is reused; otherwise a new record is inserted. This keeps the catalogue normalised across all users while giving each user an independent nursery record.

## API Endpoints

| Method | Route | Description | Status |
|--------|-------|-------------|--------|
| `GET` | `/api/plants` | Retrieve the full plant catalog | `200 OK` |
| `GET` | `/api/plants/{name}` | Retrieve a specific plant by name | `200 OK` |
| `GET` | `/api/plants/nursery/{userId}` | Retrieve all plants for a user | `200 OK` |
| `POST` | `/api/plants/nursery/{userId}` | Add a plant to a user's nursery | `201 Created` |
| `DELETE` | `/api/plants/nursery/{userId}/{plantName}` | Remove a plant from a user's nursery | `204 No Content` |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A trusted HTTPS dev certificate:
```bash
  dotnet dev-certs https --trust
```

### Run Locally

Both `Nursery.Api` and `Nursery.Clientlogin` need to run simultaneously — the console client communicates with the API over HTTP.

```bash
git clone https://github.com/thunder-blight/VirtualNursery.git
cd VirtualNursery
dotnet restore
```

**Terminal 1 — start the API:**
```bash
dotnet run --project Nursery.Api
```
The API starts at `https://localhost:7288` and Swagger UI is available at `https://localhost:7288/swagger`.

**Terminal 2 — start the console client:**
```bash
dotnet run --project Nursery.Clientlogin
```

### Quick Test via curl

```bash
# Get the full plant catalog
curl https://localhost:7288/api/plants

# Get all plants for a user
curl https://localhost:7288/api/plants/nursery/{userId}

# Add a plant to a user's nursery
curl -X POST https://localhost:7288/api/plants/nursery/{userId} \
  -H "Content-Type: application/json" \
  -d '{"name": "Monstera", "type": "Shrub", "lifeCycle": "Perennial", "floweringStatus": false}'

# Remove a plant from a user's nursery
curl -X DELETE https://localhost:7288/api/plants/nursery/{userId}/Monstera
```

## Roadmap

- [x] SQLite database with normalised schema
- [x] Console client backed by REST API
- [x] CRUD-complete plant endpoints
- [ ] JWT authentication
- [ ] Admin role functionality
- [ ] Users API endpoints

## License

MIT
