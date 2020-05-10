using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CusJoWebAPI.Models;

namespace CusJoWebAPI
{
   
         public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<UserDto,ApplicationUser>();
        }
    }
}

