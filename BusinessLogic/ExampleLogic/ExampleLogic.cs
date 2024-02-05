using AutoMapper;
using Contracts.Example;
using Models.Dtos.Example;

namespace BusinessLogic.ExampleLogic
{
    public class ExampleLogic(IExampleService exampleService, IMapper mapper) : IExampleLogic
    {
        public async Task<IEnumerable<ExampleDto>> GetAll(CancellationToken cancellationToken)
        {
            var entities = await exampleService.GetAllAsync(cancellationToken);
            return mapper.Map<IEnumerable<ExampleDto>>(entities);
        }

        public async Task<ExampleDto> GetById(int id, CancellationToken cancellationToken)
        {
            var entity = await exampleService.GetByIdAsync(id, cancellationToken);

            return mapper.Map<ExampleDto>(entity);
        }
    }
}
