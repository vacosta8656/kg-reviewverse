# KgReviewverse

This project contains a .NET backend and a crawler for Korean dramas.

## Useful Commands

### 1. Build the project

```bash
dotnet build
```

### 2. Run the backend

```bash
dotnet run --project KgReviewverse.Backend
```

### 3. Run the crawler

```bash
dotnet run --project KgReviewverse.Crawler
```

### 4. Apply changes to the database model (Entity Framework Core)

**Create a new migration:**
```bash
dotnet ef migrations add MigrationName --project KgReviewverse.Backend
```

**Update the database:**
```bash
dotnet ef database update --project KgReviewverse.Backend
```

### 5. Check active Docker containers

```bash
docker ps
```