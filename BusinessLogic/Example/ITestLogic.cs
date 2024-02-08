using Models.Dtos.Example;

namespace BusinessLogic.Example
{
    public interface ITestLogic
    {
        public Task<IEnumerable<TestDto>> GetAll(CancellationToken cancellationToken);
        public Task<TestDto> GetById(int id, CancellationToken cancellationToken);
    }
}
