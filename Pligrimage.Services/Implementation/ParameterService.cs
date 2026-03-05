using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Trackable;
using ITS.Core.Services;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;

namespace Pligrimage.Services.Implementation
{
    public class ParameterService : Service<Parameter>, IParameterService

    {

        public ParameterService(ITrackableRepository<Parameter> paraemeterRepository) : base(paraemeterRepository)
        {

        }

        public  IEnumerable<Parameter> GetClassTypeListAsync()
        {
            return  Repository.Queryable().Where(c => c.Code == "ClassType");
        }

        public IEnumerable<Parameter> GetConfirmCodeParameter()
        {
            return Repository.Queryable().Where(c => c.Code == "ConfirmCode");
        }

        public IEnumerable<Parameter> GetFitCodeTypeList()
        {
            return Repository.Queryable().Where(c => c.Code == "FitCode");
        }

        public IEnumerable<Parameter> GetRankCodeListAsync()
        {
            return Repository.Queryable().Where(c => c.Code == "RankCode");
        }

        public IEnumerable<Parameter> GetRTNCodeListAsync()
        {
            return Repository.Queryable().Where(c => c.Code == "RTNCode");
        }
    }
}
