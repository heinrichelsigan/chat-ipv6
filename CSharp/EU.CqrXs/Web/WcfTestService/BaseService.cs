using EU.CqrXs.Web.WcfTestService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EU.CqrXs.Web.WcfTestService
{
    public abstract class BaseService<T> : IBaseService, IBaseCRUDService<T>
    {
        public abstract T Create(T item);
        public abstract T Delete(string id);
        public abstract T Get(string id);
        public abstract T Update(string id, T item);

        public virtual string TestService(string input)
        {
            return $"input = {input}. Now = {DateTime.Now}";
        }

        public string TestServicePost(string input, string postObj)
        {
            return $"input = {input}. Now = {DateTime.Now}";
        }
    }
}