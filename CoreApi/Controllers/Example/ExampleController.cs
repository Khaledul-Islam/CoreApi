using BusinessLogic.ExampleLogic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos.Example;
using Models.Enums;
using Utilities.Response;

namespace CoreApi.Controllers.Example
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExampleController(IExampleLogic exampleLogic) : BaseController
    {
        [HttpGet("{id:int:min(1)}")]
        public async Task<ApiResponse<ExampleDto>> GetFileById(int id, CancellationToken cancellationToken)
        {
            var model = await exampleLogic.GetById(id, cancellationToken);
            if (model == null!)
            {
                return new ApiResponse<ExampleDto>(false, ApiResultBodyCode.NotFound, model!);
            }
            return new ApiResponse<ExampleDto>(true, ApiResultBodyCode.Success, model);
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<ExampleDto>>> GetAll(CancellationToken cancellationToken)
        {
            var models = await exampleLogic.GetAll(cancellationToken);
            var listModel = models.ToList();// its just a test if u need list pass through in BLL a list
            if (!listModel.Any())
            {
                return new ApiResponse<IEnumerable<ExampleDto>>(false, ApiResultBodyCode.NotFound, listModel);
            }
            return new ApiResponse<IEnumerable<ExampleDto>>(true, ApiResultBodyCode.Success, listModel);
        }
    }
}
