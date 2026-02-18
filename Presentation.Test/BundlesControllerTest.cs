using Application.Abstraction.Messaging;
using Application.Contracts.Bundles;
using Application.Feathers.Bundles.AddBundle;
using Application.Feathers.Bundles.GetBundle;
using Domain.Abstraction;
using Domain.Errors;
using FakeItEasy;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using Presentation.DTOs.Bundles;

namespace Presentation.Test;

public class BundlesControllerTest
{
    // ===================================================
    // Create a fake ISender to use in all tests
    // ===================================================
    private readonly ISender _sender = A.Fake<ISender>();

    // ===================================================
    // TEST 1: When Bundle not found → return Problem
    // ===================================================
    [Fact]
    public async Task Get_WhenBundleNotFound_ReturnsProblem()
    {
        // Arrange
        A.CallTo(() => _sender.Send(A<GetBundleQuery>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Result.Failure<BundleDetailResponse>(BundleErrors.NotFound));

        var controller = new BundlesController(_sender);

        // Act
        var result = await controller.Get(id: 999, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    // ===================================================
    // TEST 2: When Bundle exists → return Ok (HTTP 200)
    // ===================================================
    [Fact]
    public async Task Get_WhenBundleExists_ReturnsOk()
    {
        // Arrange
        var bundleResponse = new BundleDetailResponse(
            Bundle: A.Fake<BundleResponse>(),
            Products: []
        );

        A.CallTo(() => _sender.Send(A<GetBundleQuery>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Result.Success(bundleResponse));

        var controller = new BundlesController(_sender);

        // Act
        var result = await controller.Get(id: 1, CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    // ===================================================
    // TEST 3: When Validation fails → Sender should NOT be called
    // ===================================================
    [Fact]
    public async Task Add_WhenValidationFails_DoesNotCallSender()
    {
        // Arrange
        var validator = A.Fake<IValidator<BundleWithImageRequest>>();

        var failedValidation = new ValidationResult(
            [new ValidationFailure("Name", "Name is required")]
        );

        A.CallTo(() => validator.ValidateAsync(A<BundleWithImageRequest>.Ignored, A<CancellationToken>.Ignored))
            .Returns(failedValidation);

        var controller = new BundlesController(_sender);

        var request = new BundleWithImageRequest(
            new BundleRequest("", 10, 20, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), [1, 2]),
            null
        );

        // Act
        var result = await controller.Add(request, validator, CancellationToken.None);

        // Assert — Sender should NOT have been called at all
        A.CallTo(() => _sender.Send(A<AddBundleCommand>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    // ===================================================
    // TEST 4: When Sender returns Failure → return Problem
    // ===================================================
    [Fact]
    public async Task Add_WhenSenderReturnsFailure_ReturnsProblem()
    {
        // Arrange
        var validator = A.Fake<IValidator<BundleWithImageRequest>>();
        A.CallTo(() => validator.ValidateAsync(A<BundleWithImageRequest>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => _sender.Send(A<AddBundleCommand>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Result.Failure<BundleDetailResponse>(BundleErrors.DuplicatedName));

        var controller = new BundlesController(_sender);

        var request = new BundleWithImageRequest(
            new BundleRequest("Test", 10, 20, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), [1, 2]),
            null
        );

        // Act
        var result = await controller.Add(request, validator, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
}
