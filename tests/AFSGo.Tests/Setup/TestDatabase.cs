using AFSGo.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace AFSGo.Tests.Setup;

public static class TestDatabase
{
    public static ApplicationDbContext Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "database")
            .Options;

        return new ApplicationDbContext(options);
    }
}