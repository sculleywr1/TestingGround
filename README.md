# ChatApp

This is a minimal Blazor Server chat application built on .NET 8.0. It uses SignalR for real-time messaging and Identity with JWT tokens for user authentication. Messages are stored in a SQLite database. 

## Features
- Register and log in users with Identity
- JWT authentication for SignalR connections
- Public chat and private messaging
- Simple Bootstrap-based UI

## Running Locally
1. Install the .NET 8 SDK.
2. Build the project:
   ```bash
   dotnet build ChatApp/ChatApp.csproj -v minimal
   ```
3. Run the server:
   ```bash
   dotnet run --project ChatApp/ChatApp.csproj --urls http://localhost:5005
   ```
4. Open `http://localhost:5005` in a browser.

> **Note**: The application uses `Index.razor` as the home page. Breakpoints set in `Index.cshtml` will not be hit because that file is not part of the app.

