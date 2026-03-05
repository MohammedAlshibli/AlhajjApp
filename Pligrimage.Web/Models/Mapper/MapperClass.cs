using AutoMapper;
using Pligrimage.Entities;
using Pligrimage.Web.Common.ViewModel;
using Pligrimage.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models.Mapper
{
    public class MapperClass: Profile
    {
        public MapperClass()
        {
            CreateMap<AlhajjMaster, MedicalViewModel>();
            CreateMap<JundEmployeeDto, AlhajjMaster>()
       .ForMember(dest => dest.ServcieNumber, opt => opt.MapFrom(src => src.ServiceNumber))
       .ForMember(dest => dest.HrmsUnitDesc, opt => opt.MapFrom(src => src.CurrentUnitAr))
       .ForMember(dest => dest.GSM, opt => opt.MapFrom(src => src.Mobile))
       .ForMember(dest => dest.NIC, opt => opt.MapFrom(src => src.CivilNumber))
       .ForMember(dest => dest.RankDesc, opt => opt.MapFrom(src => src.RankArabic))
        .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstNameAr} {src.SecondNameAr} {src.ThirdNameAr} {src.LastNameAr}"))
        .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Address))
         .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
         .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.BloodGroup))
         .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth))
          .ForMember(dest => dest.PassportExpire, opt => opt.MapFrom(src => src.PasspostExpiryDate))
                    .ForMember(dest => dest.NICExpire, opt => opt.MapFrom(src => src.PasspostExpiryDate))

          .ForMember(dest => dest.Passport, opt => opt.MapFrom(src => src.PasspostNo))
                                           .ForMember(dest => dest.WilayaDesc, opt => opt.MapFrom(src => src.ServiceBranchNameAr));










        }
    }
}
