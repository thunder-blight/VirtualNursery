# VirtualNursery
A multi-user virtual plant management application built with **C# and .NET 8**.

VirtualNursery lets registered users build and manage a personal collection of plants through a RESTful HTTP API. The service layer is fully abstracted from the API surface, making the storage backend swappable without touching any endpoint code.


## Tech stack
* **Language:** C# (.NET 8)
* **API framework:** ASP.NET Core Web API
* **Storage** JSON-file persistence (SQLite migration in progress)
* **IDE** JetBrains Rider


## Features
* User registration with SHA-256 password hashing
* User login with credential validation
* Plant catalog — each unique plant name maps to exactly one PlantID
* Per-user nursery via junction table
* Duplicate detection — stops immediately if a plant is already in your nursery, reuses existing catalog data if another user registered it first

## Project structure

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
│   ├── PresentationLayer/
│   │   └── Menus/
│   │       ├── LoginMenu.cs
│   │       └── PlantMenu.cs
│   ├── Services/
│   │   └── AuthServices.cs
│   ├── Program.cs
│   └── Nursery.Clientlogin.csproj
├── Nursery.Api/                      # ASP.NET Core Web API (in progress)
│   ├── Controllers/
│   │   └── PlantsController.cs
│   ├── Program.cs
│   └── Nursery.Api.csproj
└── Nursery.sln
```
The API project will depend on the core service layer but will not have any knowledge of how data is stored


## API endpoints

| Method | Route | Description | Status |
|--------|-------|-------------|--------|
| `GET` | `/api/plants` | Retrieve all plants for a user | `200 OK` |
| `POST` | `/api/plants` | Add a plant to a user's nursery | `201 Created` |
| `PUT` | `/api/plants/{id}` | Update a plant's details | 🚧 WIP |

> `DELETE` endpoint is on the roadmap.


## Database schema (planned)

The SQLite migration will use three normalised tables:

```
sql
Users      (UserID PK, Username UNIQUE, PasswordHash, Role)
Plant      (PlantID PK, Name UNIQUE, Type, LifeCycle, FloweringStatus)
UserNursery(UserID FK, PlantID FK)  -- composite PK
```

**Find-or-freate pattern:** when a user adds a plant by name, the service checks the shared `Plants` catalogue first. If a matching entry exists its `PlantID` is reused; otherwise a new record is inserted. This keeps the catalogue normalised across all users while giving eac user an independent nursery record.


## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run locally

```
bash
git clone https://github.com/thunder-bloght/VirtualNurser.git
cd VirtualNursery
dotnet restore
dotnet run --project Nursery.Api
```

The API starts at `http://localhost:5000` / `https://localhost:5001`.

### Quick test

```bash
# Get plants (replace with a valid userId)
curl http://localhost:5000/api/plants?userId=1
 
# Add a plant
curl -X POST http://localhost:5000/api/plants \
  -H "Content-Type: application/json" \
  -d '{"userId": 1, "plantName": "Monstera"}'
```


## Roadmap

- [ ] REST API endpoints via `Nursery.Api`
- [ ] JWT authentication
- [ ] Admin role functionality

## License

MIT
