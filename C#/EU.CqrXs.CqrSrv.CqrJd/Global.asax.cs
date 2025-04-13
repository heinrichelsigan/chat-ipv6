using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Util;
using EU.CqrXs.CqrSrv.CqrJd.Util;
using System;
using System.Collections.Generic;

namespace EU.CqrXs.CqrSrv.CqrJd
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Init(object sender, EventArgs e)
        {
            Area23Log.LogStatic($"Application_Init: EU.CqrXs.CqrSrv.CqrJd.HttpApplication");
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            HashSet<CContact> contacts = JsonContacts.GetContacts();
            List<string> chatRooms = JsonChatRoom.ChatRoomNumbersFromFs();
            Area23Log.LogStatic($"Application_Start: Loaded {contacts?.Count} contacts and {chatRooms?.Count} chat rooms.");                      
        }

        protected void Application_Disposed(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Area23Log.LogStatic($"Application_End: EU.CqrXs.CqrSrv.CqrJd.HttpApplication");
        }



        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Area23Log.LogStatic($"Application_BeginRequest: sender  {sender?.GetType()} {sender?.ToString()} EventArgs {e?.GetType()} {e?.ToString()}");
            Area23Log.LogStatic($"Session_Start: started new session from {Request.UserHostAddress} Referer = {Request?.UrlReferrer}");

            return;
        }


        //protected void Application_EndRequest(object sender, EventArgs e)
        //{      
        //}
        

        protected void Application_Error(object sender, EventArgs e)
        {
            Area23Log.LogStatic($"Application_Error: sender  {sender?.GetType()} {sender?.ToString()} EventArgs {e?.GetType()} {e?.ToString()}");
            // Response.Redirect(Request.ApplicationPath + "/Error.aspx");
        }


        protected void Session_Start(object sender, EventArgs e)
        {
            Area23Log.LogStatic($"Session_Start: sender  {sender?.GetType()} {sender?.ToString()} EventArgs {e?.GetType()} {e?.ToString()}");
            Area23Log.LogStatic($"Session_Start: started new session from {Request.UserHostAddress} Referer = {Request?.UrlReferrer}");
        }


        protected void Session_End(object sender, EventArgs e)
        {
            Area23Log.LogStatic($"Session_End: sender  {sender?.GetType()} {sender?.ToString()} EventArgs {e?.GetType()} {e?.ToString()}");
        }

    }

}