using Models.Dtos.Example;

namespace BusinessLogic.ExampleLogic
{
    public interface IExampleLogic
    {
        public Task<IEnumerable<ExampleDto>> GetAll(CancellationToken cancellationToken);
        public Task<ExampleDto> GetById(int id, CancellationToken cancellationToken);
    }
}
