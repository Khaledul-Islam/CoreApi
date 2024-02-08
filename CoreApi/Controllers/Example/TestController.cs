using BusinessLogic.Example;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos.Example;
using Models.Enums;
using Utilities.Response;

namespace CoreApi.Controllers.Example
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TestController(ITestLogic exampleLogic) : BaseController
    {
        [HttpGet("{id:int:min(1)}")]
        public async Task<ApiResponse<TestDto>> GetFileById(int id, CancellationToken cancellationToken)
        {
            var model = await exampleLogic.GetById(id, cancellationToken);
            if (model == null!)
            {
                return new ApiResponse<TestDto>(false, ApiResultBodyCode.NotFound, model!);
            }
            return new ApiResponse<TestDto>(true, ApiResultBodyCode.Success, model);
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<TestDto>>> GetAll(CancellationToken cancellationToken)
        {
            var models = await exampleLogic.GetAll(cancellationToken);
            var listModel = models.ToList();// its just a test if u need list pass through in BLL a list
            if (!listModel.Any())
            {
                return new ApiResponse<IEnumerable<TestDto>>(false, ApiResultBodyCode.NotFound, listModel);
            }
            return new ApiResponse<IEnumerable<TestDto>>(true, ApiResultBodyCode.Success, listModel);
        }
    }
}
