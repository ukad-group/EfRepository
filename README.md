# EfRepository

Entity Framework Core implementation of the generic repository pattern.

## Features

- Generic `IRepository<T>` implementation for Entity Framework Core
- Async/await support with `CancellationToken`
- Include and ThenInclude for eager loading
- No-tracking queries for read-only scenarios
- Patch operations for partial updates
- Batch operations (AddRange, Update collections)

## Installation

```bash
dotnet add package UkadGroup.EfRepository
```

Or via NuGet Package Manager:
```
Install-Package UkadGroup.EfRepository
```

## Usage

1. Have your DbContext implement `IEfRepositoryDbContext`:

```csharp
public class AppDbContext : DbContext, IEfRepositoryDbContext
{
    public DbSet<Product> Products { get; set; }
}
```

2. Register the repository in your DI container:

```csharp
services.AddDbContext<AppDbContext>();
services.AddScoped<IEfRepositoryDbContext>(sp => sp.GetRequiredService<AppDbContext>());
services.AddEfRepository();
```

3. Inject and use the repository:

```csharp
public class ProductService
{
    private readonly IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}
```

## Requirements

- .NET Standard 2.0+
- Entity Framework Core 3.1+
- [UkadGroup.IRepository](https://www.nuget.org/packages/UkadGroup.IRepository)

## Downloads

[![NuGet](https://img.shields.io/nuget/v/UkadGroup.EfRepository.svg)](https://www.nuget.org/packages/UkadGroup.EfRepository)

## License

MIT License - see [LICENSE](LICENSE) for details.
