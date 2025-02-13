using EU.CqrXs.Web.WcfTestService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace EU.CqrXs.Web.WcfTestService
{
    [ServiceContract]
    public interface IAuthorService : IBaseService, IBaseCRUDService<Author>
    {
    }
}