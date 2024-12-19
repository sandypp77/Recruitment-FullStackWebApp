using AutoMapper;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Models;
using Recruitment_FullStackWebApp.Common.Dtos;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Mapping from User to UserDTO and vice versa
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Job, JobDto>().ReverseMap();
        CreateMap<Job, JobCommand>().ReverseMap();
        CreateMap<JobType, JobTypeDto>().ReverseMap();
        CreateMap<JobType, JobTypeCommand>().ReverseMap();
        CreateMap<Applicant, ApplicantProfileDto>().ReverseMap();
        CreateMap<Applicant, ApplicantProfileCommand>().ReverseMap();
        CreateMap<ApplicantProfileDto, ApplicantProfileCommand>().ReverseMap();
        CreateMap<ApplicantProfileCommand, ApplicantProfileDto>().ReverseMap();
        CreateMap<JobApplication, JobApplicationDto>().ReverseMap();
        CreateMap<JobApplicationDto, JobApplication>().ReverseMap();
        CreateMap<JobApplication, JobApplicationCommand>().ReverseMap();
        CreateMap<JobApplicationDto, JobApplicationCommand>().ReverseMap();
    }
}
