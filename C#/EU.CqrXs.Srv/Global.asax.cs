using Area23.At.Framework.Library.Cqr;
using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Util;
using EU.CqrXs.Srv.Util;
using System;
using System.Collections.Generic;
using System.Web;

namespace EU.CqrXs.Srv
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Init(object sender, EventArgs e)
        {
            Area23Log.LogOriginMsg("EU.CqrXs.Srv.Global", $"Application_Init: EU.CqrXs.Srv.HttpApplication");
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            HashSet<CContact> contacts = JsonContacts.GetContacts();
            List<string> chatRooms = new List<string>();
            try
            {
                chatRooms = JsonChatRoom.ChatRoomNumbersFromFs();
            }
            catch (Exception exChatRoomsFrom) 
            {
                Area23Log.LogOriginMsgEx("EU.CqrXs.Srv.Global", "Application_Start(sender, e):", exChatRoomsFrom);
            }
            Area23Log.LogOriginMsg("EU.CqrXs.Srv.Global", $"Application_Start: Loaded {contacts?.Count} contacts and {chatRooms?.Count} chat rooms.");                      
        }

        protected void Application_Disposed(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Area23Log.LogOriginMsg("EU.CqrXs.Srv.Global", $"Application_End: EU.CqrXs.Srv.HttpApplication");
        }



        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Area23Log.LogOriginMsg("EU.CqrXs.Srv.Global", $"Application_BeginRequest: sender  {sender?.GetType()} {sender?.ToString()} EventArgs {e?.GetType()} {e?.ToString()}");
            Area23Log.LogOriginMsg("EU.CqrXs.Srv.Global", $"Session_Start: started new session from {Request.UserHostAddress} Referer = {Request?.UrlReferrer}");

            return;
        }


        //protected void Application_EndRequest(object sender, EventArgs e)
        //{      
        //}
        

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            string path = "N/A";
            if (sender is HttpApplication)
                path = ((HttpApplication)sender).Request.Url.PathAndQuery;

            Area23Log.LogOriginMsg("EU.CqrXs.Srv.Global", $"Application_Error: sender  {sender?.GetType()} {sender?.ToString()} EventArgs {e?.GetType()} {e?.ToString()}");

            CqrException appException = new CqrException(
                string.Format("{0}: {1} thrown with path {2}", ex.GetType(), ex.Message, path), 
                ex);
            
            
            // Response.Redirect(Request.ApplicationPath + "/Error.aspx");
        }


        protected void Session_Start(object sender, EventArgs e)
        {
            Area23Log.LogOriginMsg("EU.CqrXs.Srv.Global", $"Session_Start: sender  {sender?.GetType()} {sender?.ToString()} EventArgs {e?.GetType()} {e?.ToString()}");
            Area23Log.LogOriginMsg("EU.CqrXs.Srv.Global", $"Session_Start: started new session from {Request.UserHostAddress} Referer = {Request?.UrlReferrer}");
        }


        protected void Session_End(object sender, EventArgs e)
        {
            Area23Log.LogOriginMsg("EU.CqrXs.Srv.Global", $"Session_End: sender  {sender?.GetType()} {sender?.ToString()} EventArgs {e?.GetType()} {e?.ToString()}");
        }

    }

}