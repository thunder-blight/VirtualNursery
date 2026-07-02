# VirtualNursery
A multi-user virtual plant management application built with **C# and .NET 8**.

VirtualNursery lets registered users build and manage a personal collection of plants through a RESTful HTTP API. The service layer is fully abstracted from the API surface, making the storage backend swappable without touching any endpoint code.

---

## Tech stack
* **Language:** C# (.NET 8)
* **API framework:** ASP.NET Core Web API
* **Storage** JSON-file persistence (SQLite migration in progress)
* **IDE** JetBrains Rider

---

## Features
* **Multi-user plant collections** that accomodate for each user to maintain their own nursery
* **RESTful API**
* **Service layer abstraction** PlantsController won't require any changes when switching backends
* **Normalised data model** the planned SQLite schema using a shared plant catalogue with a find-or-create pattern such that common plant names are never duplicated across users

---

## Project structure

```
VirtualNursery/
├── VirtualNursery/               # Core domain — models and data services
│   ├── Models/
│   ├── Services/
│   │   ├── UserDataServices.cs
│   │   └── UserPlantDataServices.cs
│   └── ...
└── Nursery.Api/                  # ASP.NET Core Web API
    ├── Controllers/
    │   └── PlantsController.cs
    └── Program.cs
```
The API project will depend on the core service layer but will not have any knowledge of how data is stored

---

## API endpoints
## API endpoints

| Method | Route | Description | Status |
|--------|-------|-------------|--------|
| `GET` | `/api/plants` | Retrieve all plants for a user | `200 OK` |
| `POST` | `/api/plants` | Add a plant to a user's nursery | `201 Created` |
| `PUT` | `/api/plants/{id}` | Update a plant's details | 🚧 WIP |

> `DELETE` endpoint is on the roadmap.

---

## Database schema (planned)

The SQLite migration will use three normalised tables:

```
sql
Users      (UserId PK, Username, PasswordHash, CreatedAt)
Plants     (PlantId PK, CommonName, Species, Notes)       -- shared catalogue
UserNursery(UserId FK, PlantId FK, AddedAt, CustomNotes)  -- junction table
```

**Find-or-freate pattern:** when a user adds a plant by name, the service checks the shared `Plants` catalogue first. If a matching entry exists its `PlantID` is reused; otherwise a new record is inserted. This keeps the catalogue normalised across all users while giving eac user an independent nursery record.

---

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

---

## Roadmap

- [ ] Wire `PlantsController` to `PlantDatabaseServices` (SQLite)
- [ ] `PUT /api/plants/{id}` — update plant details
- [ ] `DELETE /api/plants/{id}` — remove a plant from a nursery
- [ ] Swagger / OpenAPI documentation (Swashbuckle)
- [ ] GitHub Actions CI — `dotnet build` + `dotnet test` on push
- [ ] Unit tests (xUnit) — service layer and controller validation
- [ ] Docker support for local development

---

## License

MIT
