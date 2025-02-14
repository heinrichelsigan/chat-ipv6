using EU.CqrXs.CqrSrv.Svc.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace EU.CqrXs.CqrSrv.Svc
{
    [ServiceContract]
    public interface IAuthorService : IBaseService, IBaseCRUDService<Author>
    {
    }
}