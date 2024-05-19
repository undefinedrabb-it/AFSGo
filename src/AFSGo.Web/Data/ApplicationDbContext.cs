using AFSGo.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AFSGo.Web.Data;

public class ApplicationDbContext : IdentityDbContext<UserViewModel, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TranslationViewModel> T { get; set; } = default!;

    public DbSet<TranslationViewModel> TranslationViewModel { get; set; } = default!;
}