using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU.CqrXs.WinForm.SecureChat.Entities
{

    /// <summary>
    /// CqrJsException is inherited from <see cref="CqrException"/>
    /// </summary>
    public class CqrJdException : CqrException
    {      
        private HashSet<CqrException> errorLinkedExceptions = new HashSet<CqrException>();
       
        public HashSet<CqrException> Errors { get => GetErrors(null); }
 


        public CqrJdException(string message) : base(message)
        {
            AppDomain.CurrentDomain.SetData(Constants.LAST_EXCEPTION, this);
            Area23Log.LogOriginMsgEx("CqrJdException", $"CqrJdException(string message = {message}) ctor", this);
        }

        public CqrJdException(string message, Exception innerException) : base(message, innerException)
        {
            AppDomain.CurrentDomain.SetData(Constants.LAST_EXCEPTION, this);
            Area23Log.LogOriginMsgEx("CqrJdException", "CqrJdException(...) ctor", innerException ?? this);
        }

        

        public HashSet<CqrException> GetErrors(CqrJdException? recursiceException) 
        {
            if (recursiceException == null)
            {
                errorLinkedExceptions = new HashSet<CqrException>();

                if (LastException == null)
                    return errorLinkedExceptions;
                else 
                    recursiceException = (CqrJdException)LastException;                
            }

            if (recursiceException != null)            
            {
                errorLinkedExceptions.Add(recursiceException);
                if (recursiceException.Previous != null)
                    return GetErrors((CqrJdException)((CqrJdException)recursiceException).Previous);
            }

            return errorLinkedExceptions;
        }

    }

}
