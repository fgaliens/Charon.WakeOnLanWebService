# Charon.WakeOnLanWebService

A small ASP.NET Core Minimal API for managing network devices and waking them
up remotely via Wake-on-LAN (WoL) magic packets. Devices are persisted in a
local SQLite database, keyed by MAC address, and can be renamed, re-IP'd,
pinged, and deleted through a REST API. A React frontend lives in
[`client/`](client/) (see [Frontend](#frontend) below).

## Features

- Register network devices by MAC address, with an optional IP address and title
- List all registered devices / look up a single device by MAC address
- Send a Wake-on-LAN magic packet to a device
- Ping a device and report round-trip time / status
- Rename a device or update its stored IP address
- Delete a device

## Tech stack

- [.NET 10](https://dotnet.microsoft.com/) / ASP.NET Core Minimal APIs
- Entity Framework Core with SQLite (`Microsoft.EntityFrameworkCore.Sqlite`)
- OpenAPI document generation (`Microsoft.AspNetCore.OpenApi`) in the Development environment

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Getting started

```bash
# restore & build
dotnet build

# run (applies pending EF Core migrations automatically on startup)
dotnet run --project Charon.WakeOnLanWebService
```

By default the app listens on `http://localhost:5042` (see
[`Properties/launchSettings.json`](Charon.WakeOnLanWebService/Properties/launchSettings.json)
for the HTTPS profile). A ready-to-use set of sample requests is available in
[`Charon.WakeOnLanTool.http`](Charon.WakeOnLanWebService/Charon.WakeOnLanTool.http)
(works with the HTTP Client built into Rider/Visual Studio, or the
[REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)
extension for VS Code).

In the Development environment, the OpenAPI document is served at `/openapi/v1.json`.

## Configuration

Connection string is read from `ConnectionStrings:SqliteConnection` in
[`appsettings.json`](Charon.WakeOnLanWebService/appsettings.json) (defaults to a
local `m.db` file created next to the executable on first run).

CORS origins allowed to call the API are read from `Cors:AllowedOrigins`
(an array). The Development profile allows `http://localhost:5173` (Vite's
default dev server port). When deploying for real, set this to the actual
deployed frontend origin, e.g. via the environment variable
`Cors__AllowedOrigins__0=https://your-frontend-domain` — backend and frontend
are deployed as two separate services, so this must be set explicitly per
environment.

## API overview

| Method | Route                       | Description                                   |
|--------|------------------------------|------------------------------------------------|
| POST   | `/devices`                  | Register a new device                         |
| GET    | `/devices`                  | List all devices                               |
| GET    | `/devices/{mac}`            | Get a device by MAC address                    |
| POST   | `/devices/{mac}/wake`       | Send a Wake-on-LAN packet to the device        |
| POST   | `/devices/{mac}/ping`       | Ping the device's stored IP address            |
| PATCH  | `/devices/{mac}/title`      | Rename a device                                |
| PATCH  | `/devices/{mac}/ip`         | Update a device's stored IP address            |
| DELETE | `/devices/{mac}`            | Remove a device                                |

MAC addresses are accepted in any format understood by
[`PhysicalAddress.Parse`](https://learn.microsoft.com/dotnet/api/system.net.networkinformation.physicaladdress.parse)
(e.g. `00-11-22-33-44-55`, `00:11:22:33:44:55`, or `001122334455`).

## Frontend

A React + TypeScript frontend (built with Vite) lives in [`client/`](client/).
It is styled like Windows 98 — see [`client/README.md`](client/README.md)
for details and how to run it alongside the backend.

```bash
cd client
npm install
npm run dev
```

The dev server runs on `http://localhost:5173` and talks to the backend at
the URL configured in `client/.env.development` (`http://localhost:5042` by
default). Make sure the backend's `Cors:AllowedOrigins` includes the
frontend's origin (already the case for the Development profile above).

## Deployment (linux-arm64 device + nginx)

[`deploy/deploy-on-linux-arm64.sh`](deploy/deploy-on-linux-arm64.sh) builds
the backend (self-contained, `linux-arm64`) and the frontend, copies them to
the device over SSH, installs a systemd service and an nginx site, and
restarts both:

```bash
./deploy/deploy-on-linux-arm64.sh SSH_CON FE_HOST BE_HOST
# e.g.
./deploy/deploy-on-linux-arm64.sh user@192.168.2.10 wake-on-lan.home api.wake-on-lan.home
```

- `SSH_CON` — SSH connection to the device (`user@host`)
- `FE_HOST` — hostname for the frontend
- `BE_HOST` — hostname for the backend API

Both hostnames must resolve to the device (A records in your DNS). The
device needs nginx and rsync installed; the SQLite database lives in
`/var/lib/wake-on-lan` and survives redeploys.

## Known limitations

This project is in early development.
