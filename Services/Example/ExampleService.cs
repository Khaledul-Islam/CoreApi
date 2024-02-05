using Contracts.Example;
using Data.Context;
using Data.Providers;

namespace Services.Example
{
    public class ExampleService(ApplicationDbContext context) :Repository<Models.Entities.ExampleEntity.Example,int>(context),IExampleService
    {
    }
}
