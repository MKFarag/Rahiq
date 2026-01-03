namespace Presentation.DTOs.Files;

public record UploadManyFilesRequest(
    IFormFileCollection Files
);

#region Validation

public class UploadManyFilesRequestValidator : AbstractValidator<UploadManyFilesRequest>
{
    public UploadManyFilesRequestValidator()
    {
        RuleForEach(x => x.Files)
            .SetValidator(new FileSizeValidator())
            .SetValidator(new BlockedSignaturesValidator())
            .SetValidator(new FileNameValidator());
    }
}

#endregion
