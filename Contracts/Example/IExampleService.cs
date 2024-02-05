using Data.Providers;

namespace Contracts.Example
{
    public interface IExampleService:IRepository<Models.Entities.ExampleEntity.Example,int>
    {
    }
}
