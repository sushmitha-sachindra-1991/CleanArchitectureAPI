using AutoMapper;
using static ExtModule.API.Core.ERP.F8API;

namespace ExtModule.API.Model
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Example: Mapping between DTO and Entity classes
            CreateMap<LoginRes, Loginresponse>(); 
          
        }
    }
       
}
