using Microsoft.AspNetCore.Identity;

namespace Data.Models;

public class SandboxUser : IdentityUser
{
    public string Name { get; set; }

    public int Version { get; set; } = 0;

    public SandboxUser(string email, string name)
    {
        Email = email;
        UserName = email;
        Name = name;
    }
}