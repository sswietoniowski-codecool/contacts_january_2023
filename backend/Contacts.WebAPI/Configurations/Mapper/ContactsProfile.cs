using AutoMapper;
using Contacts.WebAPI.Domain;
using Contacts.WebAPI.DTOs;

namespace Contacts.WebAPI.Configurations.Mapper;

public class ContactsProfile : Profile
{
    public ContactsProfile()
    {
        CreateMap<Contact, ContactDto>();
        CreateMap<Contact, ContactDetailsDto>();
        CreateMap<Phone, PhoneDto>();
        CreateMap<ContactForCreationDto, Contact>();
        CreateMap<ContactForUpdateDto, Contact>().ReverseMap();
    }
}