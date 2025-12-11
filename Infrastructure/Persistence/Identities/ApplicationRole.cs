namespace Infrastructure.Persistence.Identities;

public class ApplicationRole : IdentityRole<string>
{
    public ApplicationRole()
    {
        Id = Guid.CreateVersion7().ToString();
    }

    public bool IsDisabled { get; set; }
    public bool IsDefault { get; set; }
}
