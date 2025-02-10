using SwaggerWcf.Attributes;
using EU.CqrXs.Web.WcfTestService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EU.CqrXs.Web.WcfTestService
{
    [SwaggerWcf("/v1/authors")]
    [SwaggerWcfTag("Authors")]    
    [SwaggerWcfServiceInfo(
        title: "EU.CqrXs.Web.WcfTestService",
        version: "0.0.1",
        Description = "Sample Service to test SwaggerWCF",
        TermsOfService = "Terms of Service"
    )]
    [SwaggerWcfContactInfo(
        Name = "cqrxs.eu thanks to http://github.com/abelsilva",
        Url = "https://cqrxs.eu/contact",
        Email = "webmaster@cqrxs.eu"
    )]
    [SwaggerWcfLicenseInfo(
        name: "Apache License 2.0",
        Url = "https://www.apache.org/licenses/LICENSE-2.0"
    )]
    public class AuthorService : BaseService<Author>, IAuthorService
    {
        [SwaggerWcfTag("GenericAuthor")]
        public override Author Get(string id)
        {
            return new Author { Id = id };
        }

        [SwaggerWcfTag("GenericAuthor")]
        public override Author Create(Author item)
        {
            return item;
        }

        [SwaggerWcfTag("GenericAuthor")]
        public override Author Update(string id, Author item)
        {
            return new Author { Id = id };
        }

        [SwaggerWcfTag("GenericAuthor")]
        public override Author Delete(string id)
        {
            return new Author { Id = id };
        }
    }
}