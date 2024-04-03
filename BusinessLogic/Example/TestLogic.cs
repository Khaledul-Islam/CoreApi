using AutoMapper;
using Contracts.ContextAccessor;
using Contracts.Example;
using Models.Dtos.Example;

namespace BusinessLogic.Example
{
    public class TestLogic(ITestService exampleService, IMapper mapper,ICurrentPrinciple currentPrinciple) : ITestLogic
    {
        public async Task<IEnumerable<TestDto>> GetAll(CancellationToken cancellationToken)
        {
            var entities = await exampleService.GetAllAsync(cancellationToken);
            return mapper.Map<IEnumerable<TestDto>>(entities);
        }

        public async Task<TestDto> GetById(int id, CancellationToken cancellationToken)
        {
            var ss = currentPrinciple.GetCurrentUser();
            var entity = await exampleService.GetByIdAsync(id, cancellationToken);

            return mapper.Map<TestDto>(entity);
        }
    }
}
