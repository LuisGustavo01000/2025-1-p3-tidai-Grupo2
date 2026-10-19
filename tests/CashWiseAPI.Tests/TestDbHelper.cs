using CashWiseAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace CashWiseAPI.Tests;

internal static class TestDbHelper
{
    public static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
