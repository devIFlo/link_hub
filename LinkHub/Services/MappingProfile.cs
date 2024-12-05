using AutoMapper;
using LinkHub.Models;
using LinkHub.ViewModels;

namespace LinkHub.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LinkViewModel, Link>()
                .ForMember(dest => dest.FileName, opt => opt.Ignore());

            CreateMap<Link, LinkViewModel>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            CreateMap<CategoryViewModel, Category>()
                .ForMember(dest => dest.PageId, opt => opt.MapFrom(src => src.SelectedPageId))
                .ForMember(dest => dest.Page, opt => opt.Ignore());
        }
    }
}