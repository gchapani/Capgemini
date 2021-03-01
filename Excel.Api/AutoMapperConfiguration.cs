using AutoMapper;
using Excel.Data.Models;
using Excel.Domain;

namespace Excel.Service.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<EXCEL, ExcelList>().ReverseMap();
            CreateMap<EXCEL, GetImportByIdResult>().ReverseMap();
        }
    }
}