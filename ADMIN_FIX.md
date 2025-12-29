# ?? QUICK FIX: Admin Page Shows Nothing

## Problem
When you open `/Admin`, the page is blank or shows "No products".

## Solution

Run these commands in PowerShell:

```powershell
# 1. Drop the database
dotnet ef database drop --project StoreSport --force

# 2. Recreate the database
dotnet ef database update --project StoreSport

# 3. Run the application
dotnet run --project StoreSport
```

Then open: `https://localhost:7135/Admin`

You should see **9 products** in the table!

## What was fixed:
- Changed `AdminController.Index()` to use `.ToList()` to materialize data
- Changed view model from `IQueryable<Product>` to `IEnumerable<Product>`
- This ensures data is loaded from database correctly
