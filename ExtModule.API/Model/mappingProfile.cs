using AutoMapper;
using ExtModule.API.Model.DataRequest;
using ExtModule.API.Core;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<DbCallStoredProcedureInput, DbCallStoredProcedureInputDTO>();
        CreateMap<DbCallStoredProcedureInput, DbCallStoredProcedureInputDTO>();
        CreateMap<DbCallMasterInputDTO, DbCallMasterInput>();
        CreateMap<DbCallViewInput, DbCallViewInputDTO>();
    }
}
