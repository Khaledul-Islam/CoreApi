using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Dtos.Example;
using Models.Entities.Example;

namespace Config.Mapper.Example
{
    public class TestProfiles : Profile
    {
        public TestProfiles()
        {

            //Example Mapping
            CreateMap<TestDto, Test>()
                .ForMember(dest => dest.CreatedDate, src => src.Ignore());
            CreateMap<Test, TestDto>();
        }
    }
}
