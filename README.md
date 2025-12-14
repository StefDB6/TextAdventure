# TestRaiders_TextAdventure

Overview
This repository contains a small text-based adventure game and a minimal companion Web API used for authentication and keyshare delivery. The game is written in C# targeting .NET 8. The API (TextAdventureApi) provides registration, login and keyshare endpoints and uses JWT for authentication.

Contents
- `TestRaiders_TextAdventure/` — Console game
  - `Core/Interfaces/` — interfaces (`IGame`, `IRoomsManager`, `IInventory`, `IRoom`, `IItem`, `IMonster`, `IServiceCollection`)
  - `Core/Models/` — implementations (`Game`, `RoomsManager`, `Room`, `Item`, `Inventory`, `Monster`, `GameSetup`, `ServiceCollection`)
  - `Core/Encryption/` — helpers to generate/read encrypted room descriptions
  - `Program.cs` — game startup
- `TextAdventureApi/` — minimal web API (authentication and keyshare)
  - `Data/` — `TextAdventureDbContext` (seeds a `KeyShare` and an `admin` user)
  - `Services/` — `AuthService` (register/login, JWT generation)
  - `Program.cs` — API endpoints and auth configuration
- `Tests/` — unit tests for core behavior

Quick start
1. Open `TestRaiders_TextAdventure.sln` in Visual Studio.
2. !!! Ensure startup project is `TestRaiders_TextAdventure` AND `TextAdventureApi`!!!.
3. !!! Make sure GameDb exists in SQL Server Manager !!!
4. !!! run `Update-Database` in Package Manager Console & make sure the default project is `TextAdventureApi` !!!
5. Build and run (F5). The console will show the prompt and available commands.

Commands
- `look` — Show room description, exits and items.
- `inventory` — Show items in your inventory.
- `go n|e|s|w` — Move (north/east/south/west).
- `take <item_id>` — Pick up an item by id.
- `fight` — Fight a monster in the room.
- `help` — Show command list.
- `quit` — Exit the game.

API endpoints (TextAdventureApi)
- `POST /api/auth/register`
  - Body: `{ "username": "...", "password": "...", "role": 0|1 }`
  - Roles: `0 = Player`, `1 = Admin`
- `POST /api/auth/login`
  - Body: `{ "username": "...", "password": "..." }`
  - Returns: `{ "token": "...", "role": <Role> }` on success
- `GET /api/auth/me` (authorized)
  - Returns `{ Id, Username, Role }` for current JWT
- `GET /api/auth/users` (no special role required)
  - Returns list of usernames
- `GET /api/keys/keyshare/{roomId}` (authorized)
  - Requires valid JWT and a role check:
    - Admins are allowed
    - Players allowed if `KeyShare.MinRole == "Player"`
  - Returns `{ RoomId, KeyShare }`

How the game and API integrate
- After a successful login to the API, the client receives a JWT.
- The game can (optionally) store the JWT on the `RoomsManager.JwtToken` property so `RoomsManager` can perform a best-effort GET to `/api/auth/me` to check the user's role for "admin noclip".
- Admins may "noclip": they can bypass locked doors, deadly rooms, and living-monster blocks when moving between rooms. This is a convenience for development; the check is a best-effort HTTP call and not a signature validation inside the game. For production, validate tokens properly.

Encryption and room content
- `Core/Encryption` contains utilities:
  - `EncryptedRoomGenerator` — helper to generate `.enc` files from plaintext (not required at runtime if `.enc` exist).
  - `EncryptedRoomReader` — decrypts `.enc` files using a key derived from `keyshare:passphrase`.
- Plaintext files (e.g., `throne.txt`, `seal.txt`) should be added as content and configured to `Copy to Output Directory` = `Copy if newer` in Visual Studio so generation can succeed at runtime, or pre-generate the `.enc` files.

Security notes
- The API uses JWT signed tokens and validates them server-side.
- The game currently performs a best-effort HTTP call to `/api/auth/me` to detect admin role; 
- The seeded admin password is intentionally simple for local development — change after first use.

Testing
- Unit tests live in `Tests/`. Run tests from Test Explorer in Visual Studio or via `dotnet test`.

Files of interest
- `TestRaiders_TextAdventure/Core/Models/RoomsManager.cs` — movement, locked-room and admin noclip logic
- `TestRaiders_TextAdventure/Core/Encryption/*` — encryption helpers (generator/reader)
- `TextAdventureApi/Program.cs` — API endpoints and JWT setup
- `TextAdventureApi/Data/TextAdventureDbContext.cs` — DB and seed data (admin user + keyshare)