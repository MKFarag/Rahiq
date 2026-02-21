using Application.Contracts.Bundles;
using Application.Feathers.Bundles.AddBundle;
using Application.Interfaces;
using Domain.Entities;
using Domain.Errors;
using FakeItEasy;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Test.Bundles;

public class AddBundleCommandHandlerInMemoryTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AddBundleCommandHandler _handler;

    public AddBundleCommandHandlerInMemoryTest()
    {
        // Create InMemory database — this is a real DB but lives in RAM
        // Each test gets a fresh DB because of Guid.NewGuid() (unique name)
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        // Create REAL repositories — these actually query the InMemory DB
        var bundleRepo = new GenericRepository<Bundle>(_context);
        var productRepo = new GenericRepositoryWithPagination<Product>(_context);

        // We still need a fake IUnitOfWork to wire things together,
        // but .Bundles and .Products point to REAL repos with REAL DB
        var unitOfWork = A.Fake<IUnitOfWork>();
        A.CallTo(() => unitOfWork.Bundles).Returns(bundleRepo);
        A.CallTo(() => unitOfWork.Products).Returns(productRepo);
        A.CallTo(() => unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .ReturnsLazily(async () => await _context.SaveChangesAsync());

        // FileStorage and Cache are not DB related — still faked
        var fileStorage = A.Fake<IFileStorageService>();
        var cache = A.Fake<ICacheService>();

        _handler = new AddBundleCommandHandler(unitOfWork, fileStorage, cache);
    }

    // Clean up the InMemory DB after each test
    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_DuplicatedName_ReturnsFailure()
    {
        // ---- Arrange ----
        // Insert a bundle with name "Existing Bundle" into the InMemory DB
        _context.Bundles.Add(new Bundle
        {
            Name = "Existing Bundle",
            QuantityAvailable = 5,
            DiscountPercentage = 10,
            EndAt = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            BundleItems = []
        });
        await _context.SaveChangesAsync();

        // Send a command with the SAME name
        var command = new AddBundleCommand(
            new BundleRequest("Existing Bundle", 10, 20, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), [1, 2]),
            null);

        // ---- Act ----
        var result = await _handler.Handle(command);

        // ---- Assert ----
        Assert.True(result.IsFailure);
        Assert.Equal(BundleErrors.DuplicatedName, result.Error);
    }

    [Fact]
    public async Task Handle_InvalidProductIds_ReturnsFailure()
    {
        // ---- Arrange ----
        // Add products [10, 20] to DB — these are the only available products
        _context.Products.AddRange(
            new Product { Id = 10, Name = "Product 10", IsAvailable = true, Price = 100 },
            new Product { Id = 20, Name = "Product 20", IsAvailable = true, Price = 200 }
        );
        await _context.SaveChangesAsync();

        // Send command with ProductsId [1, 2] — these DON'T exist in DB
        var command = new AddBundleCommand(
            new BundleRequest("New Bundle", 10, 20, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), [1, 2]),
            null);

        // ---- Act ----
        var result = await _handler.Handle(command);

        // ---- Assert ----
        Assert.True(result.IsFailure);
        Assert.Equal(ProductErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_DuplicatedProducts_ReturnsFailure()
    {
        // ---- Arrange ----
        // Add available products [1, 2, 3]
        _context.Products.AddRange(
            new Product { Id = 1, Name = "Product 1", IsAvailable = true, Price = 100 },
            new Product { Id = 2, Name = "Product 2", IsAvailable = true, Price = 200 },
            new Product { Id = 3, Name = "Product 3", IsAvailable = true, Price = 300 }
        );

        // Add an existing bundle with products [1, 2]
        _context.Bundles.Add(new Bundle
        {
            Name = "Old Bundle",
            QuantityAvailable = 5,
            DiscountPercentage = 10,
            EndAt = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            BundleItems = [new() { ProductId = 1 }, new() { ProductId = 2 }]
        });
        await _context.SaveChangesAsync();

        // Send command with SAME products [1, 2]
        var command = new AddBundleCommand(
            new BundleRequest("New Bundle", 10, 20, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), [1, 2]),
            null);

        // ---- Act ----
        var result = await _handler.Handle(command);

        // ---- Assert ----
        Assert.True(result.IsFailure);
        Assert.Equal(BundleErrors.DuplicatedProducts, result.Error);
    }

    [Fact]
    public async Task Handle_ValidData_ReturnsSuccess()
    {
        // ---- Arrange ----
        // Add available products [1, 2, 3] — no existing bundles
        _context.Products.AddRange(
            new Product { Id = 1, Name = "Product 1", IsAvailable = true, Price = 100 },
            new Product { Id = 2, Name = "Product 2", IsAvailable = true, Price = 200 },
            new Product { Id = 3, Name = "Product 3", IsAvailable = true, Price = 300 }
        );
        await _context.SaveChangesAsync();

        var command = new AddBundleCommand(
            new BundleRequest("New Bundle", 10, 20, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), [1, 2]),
            null);

        // ---- Act ----
        var result = await _handler.Handle(command);

        // ---- Assert ----
        Assert.True(result.IsSuccess);

        // Verify the bundle was actually saved in the DB
        var savedBundle = await _context.Bundles.FirstOrDefaultAsync(b => b.Name == "New Bundle");
        Assert.NotNull(savedBundle);
    }
}
