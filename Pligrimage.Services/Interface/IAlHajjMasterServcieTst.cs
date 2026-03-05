using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrmsHttpClient.HrmsClientApi;
using ITS.Core.Abstractions.Services;
using ITS.Core.Abstractions.Trackable;
using ITS.Core.Services;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;

namespace Pligrimage.Services.Interface 
{
   public  interface IAlHajjMasterServcieTst : IService<AlhajjMaster>
    {
        Task<List<AlhajjMaster>> GetAllData();

    }


}
