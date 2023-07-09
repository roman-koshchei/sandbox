using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class Db : IdentityDbContext<SandboxUser>
{
    public Db(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}