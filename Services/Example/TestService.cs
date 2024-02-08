using Contracts.Example;
using Data.Context;
using Data.Providers;
using Models.Entities.Example;

namespace Services.Example
{
    public class TestService(ApplicationDbContext context) :Repository<Test,int>(context),ITestService
    {
    }
}
