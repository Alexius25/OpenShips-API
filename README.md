# OpenShips API

OpenShips-API is a .NET 10 Web API that ingests AIS (Automatic Identification System) streams, stores current and historical vessel positions, resolves ship destinations and ports, and exposes REST endpoints to query ports, vessel positions and tracks.

Key features
- Real-time AIS stream ingestion (AisStream, Pelyr)
- Stores current and historical AIS positions (PostgreSQL)
- Port database with aliases and time zone support
- Destination resolution and processing queues
- OpenAPI/Swagger UI for interactive exploration

Tech stack
- .NET 10 (C#)
- Entity Framework Core (migrations included)
- PostgreSQL (Npgsql)
- Docker & docker-compose
- Swashbuckle (Swagger)

Table of contents
- Quick start
- Configuration
- Running locally
- Docker
- Database migrations
- API endpoints (overview)
- Development
- Contributing
- License

Quick start (local)
Prerequisites
- .NET 10 SDK
- PostgreSQL (or use Docker compose below)
- (optional) dotnet-ef tool: `dotnet tool install --global dotnet-ef`

1) Configure the database connection
- Set the ConnectionStrings:Default connection string. You can either edit OpenShips-API/appsettings.json or set the environment variable:
  ConnectionStrings__Default="Host=localhost;Port=5432;Database=aisdb;Username=aisuser;Password=secret"

2) Restore, build, and apply migrations
- Restore and build:
  dotnet restore
  dotnet build

- Apply EF migrations (from repository root):
  dotnet ef database update --project OpenShips-API --startup-project OpenShips-API

3) Run the API
- From the repo root (or the OpenShips-API folder):
  dotnet run --project OpenShips-API

4) Open the API docs
- Swagger UI is available at: http://localhost:5000/swagger (port may vary; check console output or Properties/launchSettings.json)
- Example HTTP requests are in OpenShips-API/OpenShips-API.http (IDE REST clients like VS Code REST Client or JetBrains HTTP client can run them)

Docker (recommended for quick local env)
- Build and start API + Postgres with compose (compose.yaml included):
  docker compose -f compose.yaml up --build
- The compose file creates a postgres:17 container and passes a ConnectionStrings__Default environment variable to the API container. Edit compose.yaml if you need custom credentials.

Configuration
- OpenShips-API/appsettings.json contains the following keys to configure streams and API keys:
  - ConnectionStrings:Default  -> PostgreSQL connection string
  - Streams:AisStream           -> WebSocket AIS stream URL (default in file)
  - Streams:Pelyr               -> Pelyr stream URL
  - ApiKeys:AisStream, ApiKeys:Pelyr -> API keys for stream providers (if required)

Database & Migrations
- Migrations are included in the OpenShips-API project (Migrations/...). Use dotnet-ef to update the database.
- The DbContext is OpenShipsAPI.Infrastructure.Database.AppDbContext.

API endpoints (overview)
- Health
  GET /health
  GET /health/ais  -> AIS stream health

- Ports (versioned API)
  GET /api/v1/ports/{id}
  GET /api/v1/ports/locode/{id}
  GET /api/v1/ports/box?minLat=...&maxLat=...&minLon=...&maxLon=...
  GET /api/v1/ports/{country}/{location}
  GET /api/v1/ports/{country}
  GET /api/v1/ports/timezone/{country}/{location}

- Vessels - position & track
  GET /api/v1/vessels/position/current/{mmsi}
  GET /api/v1/vessels/position/current/box?minLat=...&maxLat=...&minLon=...&maxLon=...
  GET /api/v1/vessels/position/track/{mmsi}?from=...&to=...&limit=...

For full API contract and models, use the Swagger UI or inspect the Api/Models folder.

Development
- Recommended workflow:
  1. Create a feature branch from main
  2. Add code, tests, and migrations
  3. Run `dotnet build` and `dotnet test` (if tests are added)
  4. Submit a PR with a clear description

- Useful files:
  - OpenShips-API.http — sample requests
  - compose.yaml — docker-compose setup for local testing
  - Dockerfile — container image definition

Contributing
- Please open issues or pull requests with tests and a clear description of the change. Follow standard GitHub flow.

License
- See LICENSE.md for license terms.