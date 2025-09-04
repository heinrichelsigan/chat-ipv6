using Area23.At.Framework.Core;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Static;
using EU.CqrXs.Srv.Svc.Swashbuckle.Util;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Reflection;

namespace EU.CqrXs.Srv.Svc.Swashbuckle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestServiceController : CqrSrvControllerBase
    {

        private readonly ILogger<TestServiceController> _logger;

        public TestServiceController(ILogger<TestServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "TestService")]
        public string Get()
        {
            Area23Log.LogOriginMsg("TestServiceController", $"TestServiceController.Get() started.\n");
            
            string ret = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetName().FullName) +
                Assembly.GetExecutingAssembly().GetName().Version.ToString();

            object[] sattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (sattributes.Length > 0)
            {
                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)sattributes[0];
                if (titleAttribute.Title != "")
                    ret = titleAttribute.Title;
            }
            ret += "\n" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            object[] oattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (oattributes.Length > 0)
                ret += "\n" + ((AssemblyDescriptionAttribute)oattributes[0]).Description;

            object[] pattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (pattributes.Length > 0)
                ret += "\n" + ((AssemblyProductAttribute)pattributes[0]).Product;

            object[] crattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (crattributes.Length > 0)
                ret += "\n" + ((AssemblyCopyrightAttribute)crattributes[0]).Copyright;

            object[] cpattributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (cpattributes.Length > 0)
                ret += "\n" + ((AssemblyCompanyAttribute)cpattributes[0]).Company;


            return ret;

        }


    }
}
