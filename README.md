# Concert & Music Platform

A .NET 9 backend for managing concerts, artists, venues and ticket sales, built with the
**Onion architecture** (Domain → Repository → Service → Web), mirroring the structure of the
`ConsultationsApplication` reference exam solution.

## Projects

| Project | Responsibility |
|---|---|
| **Domain** | Entities (`Models`), `Common` base entities, `Dto`, `Enums`, `Config` option classes, `ExternalModels`. No dependencies. |
| **Repository** | `ApplicationDbContext` (EF Core + Identity), generic `IRepository<T>`/`Repository<T>`, bulk `ConcertsRepository`. |
| **Service** | Business logic (`Interface` + `Implementation`), integration services, and hosted `BackgroundService`s. |
| **Web** | Controllers, `Mapper`s, `Request`/`Response` records, `Extensions`, API-key `Middleware`, audit `Interceptor`, `DbSeeder`, `Program.cs`. |

## Domain model (9 entities, incl. a ternary relation)

- **Artist**, **Venue**, **Concert**, **TicketCategory** — core catalog entities.
- **Ticket** — **ternary relation** connecting `User` × `Concert` × `TicketCategory`.
- **Performance** — concert lineup (`Artist` × `Concert`).
- **ApiClient**, **EtlSyncLog**, **InboundEventEntry** — integration/infra entities.
- **ConcertApplicationUser** — ASP.NET Identity user.

## Exam requirements → where they live

| Requirement | Implementation |
|---|---|
| Onion architecture | 4 projects above |
| ≥5 models incl. ternary | 9 entities; `Ticket` is the ternary |
| Full CRUD for all entities | `ArtistController`, `VenueController`, `ConcertController`, `TicketCategoryController`, `TicketController`, `PerformanceController` |
| Domain-specific business logic | Venue **capacity / sold-out** enforcement, **category-multiplier pricing**, **24h cancellation rule**, `TicketsSold` counter, **revenue report** |
| **ETL** | `EtlSyncService` (extract artists from iTunes → transform to `Artist` + genre mapping → bulk upsert), logged in `EtlSyncLog`, scheduled by `SyncArtistsBackgroundService`, or triggered via `POST /api/report/etl/run` |
| **External API integration** | `ItunesMusicApiClient` typed `HttpClient` against the keyless **iTunes Search API** (`MusicApiSettings`) — imports real artists |
| **Async queue** | `POST /api/external/tickets/register` enqueues an `InboundEventEntry`; `ProcessInboundEventsBackgroundService` + `InboundEventEntryProcessor` drain it into tickets |
| **Email integration** | `EmailService` (MailKit); sends ticket confirmations. Dev writes to `wwwroot/outbox` when SMTP is disabled |
| **Excel export** | `ExcelExportService` (ClosedXML); `GET /api/report/revenue/excel` |
| Extras | JWT auth (`AuthController`), API-key middleware, fixed-window rate limiting, `IMemoryCache`, audit interceptor, Evolve SQL migrations |

## Running

```bash
cd Web
dotnet run --launch-profile http     # http://localhost:5240
```

On first run (Development) the schema is created and `DbSeeder` seeds ~60 users, 20 artists,
10 venues, 40 concerts, performances and hundreds of tickets. A seeded external `ApiClient`
key `concert-external-key-123` is available for the `/api/external/*` endpoints.

### Quick smoke test

```bash
B=http://localhost:5240
curl $B/api/concert
curl $B/api/report/revenue
curl -o revenue.xlsx $B/api/report/revenue/excel
# enqueue an external ticket purchase
curl -X POST $B/api/external/tickets/register -H "X-Api-Key: concert-external-key-123" \
  -H "Content-Type: application/json" \
  -d '{"concertId":"<id>","ticketCategoryId":"<id>","userId":"<id>"}'
```

## Configuration (`appsettings*.json`)

- `ConnectionStrings:DefaultConnection` — SQLite database.
- `MusicApiSettings` — external iTunes Search API base address, search terms, and `Enabled` toggle for the background ETL.
- `EmailSettings` — SMTP; set `Enabled: true` and fill host/credentials to send real mail.
- `ApiKeySettings` / `RateLimitSettings` / `CacheSettings` — integration knobs.
- `Jwt` — token signing settings.
