using AutoMapper;
using System;
using System.Globalization;
using VacationAPI.DTOs;
using VacationAPI.Models;

namespace VacationAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<User, UserDTO>();
            CreateMap<NationalHoliday, NationalHolidayDTO>();
            CreateMap<RegisterUserDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.ToLower()))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(src.Role.ToLower())));

            CreateMap<VacationRequest, VacationRequestDTO>()
                .ForMember(dto => dto.requestId, opt => opt.MapFrom(request => request.RequestId))
                .ForMember(dto => dto.Username, opt => opt.MapFrom(request => request.Username))
                .ForMember(dto => dto.StartDate, opt => opt.MapFrom(request => request.StartDate.Date))
                .ForMember(dto => dto.EndDate, opt => opt.MapFrom(request => request.EndDate.Date))
                .ForMember(dto => dto.Comment, opt => opt.MapFrom(request => request.Comment))
                .ForMember(dto => dto.Status, opt => opt.MapFrom(request => request.GetStatusAsString())); // use GetStatusAsString method

            CreateMap<VacationRequestDTO, VacationRequest>()
                .ForMember(request => request.RequestId, opt => opt.MapFrom(dto => dto.requestId))
                .ForMember(request => request.User, opt => opt.Ignore())
                .ForMember(request => request.StartDate, opt => opt.MapFrom(dto => dto.StartDate.Date))
                .ForMember(request => request.EndDate, opt => opt.MapFrom(dto => dto.EndDate.Date))
                .ForMember(request => request.Comment, opt => opt.MapFrom(dto => dto.Comment))
                .ForMember(request => request.Status, opt => opt.MapFrom(dto => Enum.Parse(typeof(Status), dto.Status))); // use Enum.Parse to convert string to enum
        }
    }
}