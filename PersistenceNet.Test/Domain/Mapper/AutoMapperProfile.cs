using AutoMapper;
using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Test.Domain.ViewModels;

namespace PersistenceNet.Test.Domain.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Classification, ClassificationViewModel>();
            CreateMap<ClassificationViewModel, Classification>();
        }
    }
}