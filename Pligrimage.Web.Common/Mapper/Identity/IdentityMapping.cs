using AutoMapper;
using HrmsHttpClient.HrmsClientApi;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using Pligrimage.Web.Common.ViewModel;
using System.Linq;

namespace Pligrimage.Web.Common.Mapper
{
    public class IdentityMapping : Profile
    {
        public IdentityMapping()
        {

            CreateMap<User, UserViewModel>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.HrmsPfNo, opt => opt.MapFrom(src => src.HrmsPfNo))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active))
            .ForMember(dest => dest.ModifiedOn, opt => opt.MapFrom(src => src.ModifiedOn))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active))
            .ForMember(dest => dest.LastLoginDate, opt => opt.MapFrom(src => src.LastLoginDate))
            .ForMember(dest => dest.SelectedRoles, opt => opt.MapFrom(src => src.UserRoles.Select(r => r.RoleId)))
            .ForMember(dest => dest.SelectedServices, opt => opt.MapFrom(src => src.UserServices.Select(r => r.ServiceId)))
            .ForMember(dest => dest.RoleCount, opt => opt.MapFrom(src => src.UserRoles.Count()))
            .ForMember(dest => dest.SerivceCount, opt => opt.MapFrom(src => src.UserServices.Count()));
            ;
            //.ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank))
            // .ForMember(dest => dest.LastLoginDate, opt => opt.MapFrom(src => src.LastLoginDate))
            // .//ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn));

            //.ForMember(dest => dest.IsSysAdmin, opt => opt.MapFrom(src => src.IsSysAdmin));
            //.ForMember(dest => dest.Serivce, opt => opt.MapFrom(src => src.unit))

            
            //.ForMember(dest => dest.UserRoleViewModel, opt => opt.MapFrom(src => src.UserRoles));



            //   Create New User
            CreateMap<EmployeeDetailsDto, UserCreateViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.SERVICE_NUMBER))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.NAME_ARABIC))
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.RANK_ARABIC))
                 .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.SERVICE.ToString()))
                 .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.SERVICE));



            //UserEdit
            CreateMap<UserViewModel, UserEditViewModel>()
              .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
              .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
              .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
              .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank))
              .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Active))
              .ForMember(dest => dest.Serivce, opt => opt.MapFrom(src => src.Serivce))
              .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.UnitName))
              .ForMember(dest => dest.SelectedRoles, opt => opt.MapFrom(src => src.UserRoleViewModel.Select(r => r.RoleId)));

 
            //// roles
            CreateMap<Role, RoleViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.UserRoleViewModel, opt => opt.MapFrom(src => src.UserRoles))              
                .ForMember(dest => dest.RolePermissionsViewModel, opt => opt.MapFrom(src => src.RolePermissions))         
               ;




            CreateMap<RoleViewModel, RoleCreateEditViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))                 
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.SelectedPermissions, opt => opt.MapFrom(src => src.RolePermissionsViewModel.Select(r => r.PermissionId)));


            CreateMap<Role, RoleCreateEditViewModel>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(dest => dest.SelectedPermissions, opt => opt.MapFrom(src => src.RolePermissions.Select(r => r.PermissionId)))
               ;



            CreateMap<Permission, PermissionsViewModel>();

        




        }
    }
}
