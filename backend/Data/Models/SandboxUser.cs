
using Microsoft.AspNetCore.Identity;

namespace Data.Models;

public class SandboxUser : IdentityUser
{
    string Name { get; }

    public SandboxUser(string email, string name) { 
        Email = email;
        UserName = email;
        Name = name;
    }
}
