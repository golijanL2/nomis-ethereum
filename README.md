# Nomis

Nomis is a crypto protocol based on a mathematical prediction and AI model enabling protocol users with a favorable on-chain credit score to borrow crypto with a fair collateral and APR on a case-by-case basis. On the other hand, Nomis is an open-source protocol that helps web3 developers both to build new on-chain solutions and use cases, and to balance already existing high-TVL protocols.

# Development

## Add new PosgreSQL database migrations

0. Install `dotnet-ef` tool:
```
dotnet tool install --global dotnet-ef
```
1. Verify installation:
```
dotnet ef
```
2. Open the window: `Package Manager Console` (`View`->`Other Windows`->`Package Manager Console`);
3. Select as the default project the project in which the data access database context is located, through which we want to add a new migration;
4. Execute command with appropriate context.
- For example, adding new `Initial` migration for `ApplicationDbContext` database context:
```
add-migration Initial -o Persistence/Migrations/ -context ApplicationDbContext
```

or

```
dotnet ef migrations add "initial" -o Persistence/Migrations/ --context ApplicationDbContext

dotnet ef migrations add "initial" -o Persistence/Migrations/ --context ScoringDbContext --project ./src/Storage/Nomis.DataAccess.PostgreSql.Scoring/Nomis.DataAccess.PostgreSql.Scoring.csproj --startup-project ./src/Common/Nomis.Web.Server.Common/Nomis.Web.Server.Common.csproj -v
```

![Add new migration](/images/add-new-migration.png)

> When developing pre-release versions, when adding new changes, it is allowed not to create new migrations, but to delete the entire `Persistence/Migrations/` directory and re-create the `Initial` migration, since the database model can often change during development.

![Persistence catalog location](/images/persistence-location.png)