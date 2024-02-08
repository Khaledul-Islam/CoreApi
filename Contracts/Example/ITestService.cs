using Data.Providers;
using Models.Entities.Example;

namespace Contracts.Example
{
    public interface ITestService:IRepository<Test,int>
    {
    }
}
