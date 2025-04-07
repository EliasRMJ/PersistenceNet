using AutoMapper;
using PersistenceNet.Enuns;
using PersistenceNet.Extensions;
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

            CreateMap<Client, ClientViewModel>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.Person.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Person.Name))
                .ForMember(d => d.ComplementName, opt => opt.MapFrom(src => src.Person.ComplementName))
                .ForMember(d => d.InclusionDate, opt => opt.MapFrom(src => src.Person.InclusionDate))
                .ForMember(d => d.PersonType, opt => opt.MapFrom(src => src.Person.PersonType))
                .ForMember(d => d.Active, opt => opt.MapFrom(src => src.Person.Active))
                .ForMember(d => d.Note, opt => opt.MapFrom(src => src.Note))
                .ForMember(d => d.DocumentNumber, opt => opt.MapFrom(src => src.Person.PersonType == PersonTypeEnum.Phisic ? ((PhysicsPerson)src.Person).DocumentNumber : ((LegalPerson)src.Person).DocumentNumber))
                .ForMember(d => d.MunicipalRegistration, opt => opt.MapFrom(src => src.Person.PersonType == PersonTypeEnum.Phisic ? null : ((LegalPerson)src.Person).MunicipalRegistration))
                .ForMember(d => d.DateBirth, opt => opt.MapFrom(src => src.Person.PersonType == PersonTypeEnum.Phisic ? ((PhysicsPerson)src.Person).DateBirth : null))
                .ForMember(d => d.Emails, opt => opt.MapFrom(src => src.Person.Emails));

            CreateMap<ClientViewModel, Client>()
                .ForMember(d => d.Person, opt => opt.MapFrom(map => map.PersonType == PersonTypeEnum.Legal ? new LegalPerson
                {
                    Id = map.ClientId,
                    Name = map.Name,
                    ComplementName = map.ComplementName,
                    InclusionDate = map.InclusionDate,
                    PersonType = map.PersonType,
                    Active = map.Active,
                    DocumentNumber = map.DocumentNumber,
                    MunicipalRegistration = map.MunicipalRegistration,
                    Emails = map.Emails != null ? map.Emails!.Select(email => new EmailPerson
                    {
                        Id = email.Id,
                        Mail = email.Mail,
                        PersonId = map.ClientId
                    }).ToCollection() : null,
                } : null))
                .ForMember(d => d.Person, opt => opt.MapFrom(map => map.PersonType == PersonTypeEnum.Phisic ? new PhysicsPerson
                {
                    Id = map.ClientId,
                    Name = map.Name,
                    ComplementName = map.ComplementName,
                    InclusionDate = map.InclusionDate,
                    PersonType = map.PersonType,
                    Active = map.Active,
                    DocumentNumber = map.DocumentNumber,
                    Emails = map.Emails != null ? map.Emails!.Select(email => new EmailPerson
                    {
                        Id = email.Id,
                        Mail = email.Mail,
                        PersonId = map.ClientId
                    }).ToCollection() : null,
                } : null));

            CreateMap<EmailViewModel, EmailPerson>();
            CreateMap<EmailPerson, EmailViewModel>();
        }
    }
}