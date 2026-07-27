# VirtualNursery

A multi-user virtual plant management application built with **C# and .NET 8**.

VirtualNursery lets registered users build and manage a personal collection of plants through a console client and React web frontend, both backed by a RESTful API. The shared `Nursery.Core` library separates all storage and domain logic from the CLI, API, and frontend, keeping each layer independent.

## Tech Stack

- **Language:** C# / .NET 8
- **API framework:** ASP.NET Core Web API
- **Database:** SQLite via `Microsoft.Data.Sqlite`
- **Frontend:** React 19 + Vite + TypeScript
- **IDE:** JetBrains Rider / VS Code

## Project Structure

```
VirtualNursery/
├── Nursery.Core/ # Shared class library
│ ├── Common/
│ │ ├── LifeCycleType.cs
│ │ ├── PlantType.cs
│ │ └── UserType.cs
│ ├── Infrastructure/
│ │ ├── DataPaths.cs
│ │ ├── DatabaseServices.cs
│ │ ├── PlantDatabaseServices.cs
│ │ ├── UserDatabaseServices.cs
│ │ └── UserIDGenerator.cs
│ ├── Models/
│ │ ├── Plant.cs
│ │ ├── User.cs
│ │ └── UserSession.cs
│ └── Nursery.Core.csproj
├── Nursery.Clientlogin/ # Console client
│ ├── Infrastructure/
│ │ └── NurseryApiClient.cs
│ ├── PresentationLayer/
│ │ └── Menus/
│ │ ├── LoginMenu.cs
│ │ └── PlantMenu.cs
│ ├── Services/
│ │ └── AuthServices.cs
│ ├── Program.cs
│ └── Nursery.Clientlogin.csproj
├── Nursery.Api/ # ASP.NET Core Web API
│ ├── Controllers/
│ │ ├── OptionsController.cs
│ │ └── PlantsController.cs
│ ├── Program.cs
│ └── Nursery.Api.csproj
├── Nursery.Web/ # React + TypeScript frontend
│ ├── src/
│ │ ├── components/
│ │ │ └── PlantForm.tsx
│ │ ├── pages/
│ │ │ ├── CreatePlant.tsx
│ │ │ ├── Home.tsx
│ │ │ └── PlantDetail.tsx
│ │ ├── types/
│ │ │ ├── Options.ts
│ │ │ └── Plant.ts
│ │ ├── App.tsx
│ │ ├── App.css
│ │ ├── main.tsx
│ │ └── vite-env.d.ts
│ ├── index.html
│ ├── tsconfig.json
│ └── package.json
├── data/
│ └── nursery.db # Shared SQLite database
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
- React + TypeScript frontend with full CRUD — browse the catalog, view plant details, create new plants, edit existing plants, and delete plants from the catalog
- URL routing — each plant has its own page at `/plant/{plantId}`
- Live search on the home page — filters the plant catalog by name as you type

## Database Schema

Three normalised tables stored in a single shared `nursery.db` file:

```
sql
Users      (UserID PK, Username UNIQUE, PasswordHash, Role)
Plant      (PlantID PK, Name UNIQUE, Type, LifeCycle, FloweringStatus)
UserNursery(UserID FK, PlantID FK)  -- composite PK
```

**Find-or-create pattern:** when a user adds a plant by name, the service checks the shared `Plant` catalogue first. If a matching entry exists its `PlantID` is reused; otherwise a new record is inserted. This keeps the catalogue normalised across all users while giving each user an independent nursery record.

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/plants` | Retrieve the full plant catalog |
| `GET` | `/api/plants/name/{name}` | Retrieve a specific plant by name |
| `GET` | `/api/plants/id/{plantId}` | Retrieve a specific plant by ID |
| `GET` | `/api/plants/nursery/{userId}` | Retrieve all plants for a user |
| `POST` | `/api/plants` | Create a new plant in the catalog |
| `POST` | `/api/plants/nursery/{userId}` | Add a plant to a user's nursery |
| `PUT` | `/api/plants/id/{plantId}` | Update a plant's details |
| `DELETE` | `/api/plants/id/{plantId}` | Delete a plant from the catalog |
| `DELETE` | `/api/plants/nursery/{userId}/{plantName}` | Remove a plant from a user's nursery |
| `GET` | `/api/options` | Retrieve available plant type and life cycle options |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Node.js](https://nodejs.org) v18 or later
- A trusted HTTPS dev certificate:
```bash
dotnet dev-certs https --trust
```

### Run Locally

`Nursery.Api` must be running before starting either the console client or the React frontend.

```bash
git clone https://github.com/thunder-blight/VirtualNursery.git
cd VirtualNursery
dotnet restore
```

**Terminal 1 — start the API:**
```bash
dotnet run --project Nursery.Api
```
The API starts at `https://localhost:7288`. Swagger UI is available at `https://localhost:7288/swagger`.

**Terminal 2 — start the console client:**
```bash
dotnet run --project Nursery.Clientlogin
```

**Terminal 3 — start the React frontend:**
```bash
cd Nursery.Web
npm install
npm run dev
```
The frontend is available at `http://localhost:5173`.

### Quick Test via curl

```bash
# Get the full plant catalog
curl https://localhost:7288/api/plants

# Get a specific plant by ID
curl https://localhost:7288/api/plants/id/1

# Get all plants for a user
curl https://localhost:7288/api/plants/nursery/{userId}

# Create a new plant
curl -X POST https://localhost:7288/api/plants \
  -H "Content-Type: application/json" \
  -d '{"name": "Monstera", "type": "Shrub", "lifeCycle": "Perennial", "floweringStatus": false}'

# Add a plant to a user's nursery
curl -X POST https://localhost:7288/api/plants/nursery/{userId} \
  -H "Content-Type: application/json" \
  -d '{"name": "Monstera", "type": "Shrub", "lifeCycle": "Perennial", "floweringStatus": false}'

# Update a plant
curl -X PUT https://localhost:7288/api/plants/id/1 \
  -H "Content-Type: application/json" \
  -d '{"name": "Monstera", "type": "Herb", "lifeCycle": "Annual", "floweringStatus": true}'

# Delete a plant from the catalog
curl -X DELETE https://localhost:7288/api/plants/id/1

# Remove a plant from a user's nursery
curl -X DELETE https://localhost:7288/api/plants/nursery/{userId}/Monstera
```

## Roadmap

- [x] SQLite database with normalised schema
- [x] Console client backed by REST API
- [x] CRUD-complete plant endpoints
- [x] React + TypeScript frontend with full CRUD
- [x] Plant detail page with URL routing (`/plant/:plantId`)
- [x] Live search on home page
- [ ] xUnit test suite
- [ ] Responsive CSS for mobile and smaller screens
- [ ] Per-user nursery view in the React frontend
- [ ] JWT authentication
- [ ] Admin role functionality
- [ ] Users API endpoints

## License

MIT
