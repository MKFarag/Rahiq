using Application.Contracts.Bundles;
using Application.Feathers.Bundles.AddBundle;
using Application.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories;
using FakeItEasy;
using System.Linq.Expressions;

namespace Application.Test.Bundles;

public class AddBundleCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IFileStorageService _fileStorage = A.Fake<IFileStorageService>();
    private readonly ICacheService _cache = A.Fake<ICacheService>();
    private readonly IGenericRepository<Bundle> _bundleRepo = A.Fake<IGenericRepository<Bundle>>();
    private readonly IGenericRepositoryWithPagination<Product> _productRepo = A.Fake<IGenericRepositoryWithPagination<Product>>();
    private readonly AddBundleCommandHandler _handler;

    public AddBundleCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Bundles).Returns(_bundleRepo);
        A.CallTo(() => _unitOfWork.Products).Returns(_productRepo);
        _handler = new AddBundleCommandHandler(_unitOfWork, _fileStorage, _cache);
    }

    [Fact]
    public async Task Handle_DuplicatedName_ReturnsFailure()
    {
        // ---- Arrange ----
        A.CallTo(() => _bundleRepo.AnyAsync(A<Expression<Func<Bundle, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new AddBundleCommand(
            new BundleRequest("Existing Name", 10, 20, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), [1, 2]),
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
        A.CallTo(() => _bundleRepo.AnyAsync(A<Expression<Func<Bundle, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _productRepo.FindAllProjectionAsync<int>(
                A<Expression<Func<Product, bool>>>.Ignored,
                A<Expression<Func<Product, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(new List<int> { 10, 20 });

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
        A.CallTo(() => _bundleRepo.AnyAsync(A<Expression<Func<Bundle, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _productRepo.FindAllProjectionAsync<int>(
                A<Expression<Func<Product, bool>>>.Ignored,
                A<Expression<Func<Product, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(new List<int> { 1, 2, 3 });

        A.CallTo(() => _bundleRepo.FindAllAsync(
                A<Expression<Func<Bundle, bool>>>.Ignored,
                A<string[]>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(new List<Bundle>
            {
                new()
                {
                    Id = 50,
                    Name = "Old Bundle",
                    BundleItems = [new() { ProductId = 1 }, new() { ProductId = 2 }]
                }
            });

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
        A.CallTo(() => _bundleRepo.AnyAsync(A<Expression<Func<Bundle, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _productRepo.FindAllProjectionAsync<int>(
                A<Expression<Func<Product, bool>>>.Ignored,
                A<Expression<Func<Product, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(new List<int> { 1, 2, 3 });

        A.CallTo(() => _bundleRepo.FindAllAsync(
                A<Expression<Func<Bundle, bool>>>.Ignored,
                A<string[]>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(new List<Bundle>());

        A.CallTo(() => _bundleRepo.AddAsync(A<Bundle>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsLazily((Bundle b, CancellationToken _) => { b.Id = 1; return b; });

        var command = new AddBundleCommand(
            new BundleRequest("New Bundle", 10, 20, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), [1, 2]),
            null);

        // ---- Act ----
        var result = await _handler.Handle(command);

        // ---- Assert ----
        Assert.True(result.IsSuccess);
    }
}
