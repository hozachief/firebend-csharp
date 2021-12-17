using AutoMapper;
using Data;
using Services.Models;

namespace Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, Product>();
        }   
    }
}