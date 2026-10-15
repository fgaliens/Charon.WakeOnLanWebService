# Charon.WakeOnLanTool

A small ASP.NET Core Minimal API for managing network devices and waking them
up remotely via Wake-on-LAN (WoL) magic packets. Devices are persisted in a
local SQLite database, keyed by MAC address, and can be renamed, re-IP'd,
pinged, and deleted through a REST API.

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
dotnet run --project Charon.WakeOnLanTool
```

By default the app listens on `http://localhost:5042` (see
[`Properties/launchSettings.json`](Charon.WakeOnLanTool/Properties/launchSettings.json)
for the HTTPS profile). A ready-to-use set of sample requests is available in
[`Charon.WakeOnLanTool.http`](Charon.WakeOnLanTool/Charon.WakeOnLanTool.http)
(works with the HTTP Client built into Rider/Visual Studio, or the
[REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)
extension for VS Code).

In the Development environment, the OpenAPI document is served at `/openapi/v1.json`.

## Configuration

Connection string is read from `ConnectionStrings:SqliteConnection` in
[`appsettings.json`](Charon.WakeOnLanTool/appsettings.json) (defaults to a
local `m.db` file created next to the executable on first run).

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

## Known limitations

This project is in early development. Notably, the `POST /devices/{mac}/wake`
endpoint does not yet send a magic packet — see the code review notes for
details and other open issues before relying on this in production.
