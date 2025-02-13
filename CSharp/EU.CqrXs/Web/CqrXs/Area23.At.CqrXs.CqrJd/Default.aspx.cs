using Area23.At.Framework.Library;
using Area23.At.Framework.Library.Util;
using Area23.At.CqrXs.CqrJd.Util;
using Newtonsoft.Json;
using QRCoder;
using static QRCoder.PayloadGenerator;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.DynamicData;
using System.Windows.Shapes;
using System.Runtime.Serialization.Formatters;
using System.Security.Policy;
using Area23.At.Framework.Library.Crypt.CqrJd;
using Area23.At.Framework.Library.Net.CqrJd;
using Newtonsoft.Json.Serialization;
using static QRCoder.PayloadGenerator.SwissQrCode;
using System.Configuration;
using System.Text;

namespace Area23.At.CqrXs.CqrJd
{
    public partial class Default : CqrJdBasePage
    {
        string hashKey = string.Empty;
        string tmpStrg = string.Empty;
        string allStrng = string.Empty;
        string decrypted = string.Empty;
        object _lock = new object();
        HashSet<CqrContact> _contacts;
        CqrContact myContact = null;
        IPAddress clientIp;

        protected void Page_Load(object sender, EventArgs e)
        {
            string hexall = string.Empty;
            string myServerKey = string.Empty;
            allStrng = string.Empty;
            myContact = null;
            if (Application[Constants.JSON_CONTACTS] != null)
                _contacts = (HashSet<CqrContact>)(Application[Constants.JSON_CONTACTS]);
            else
                _contacts = LoadJsonContacts();

            if (ConfigurationManager.AppSettings["ServerIPv4"] != null)
            {
                LiteralServerIPv4.Text = (string)ConfigurationManager.AppSettings["ServerIPv4"];
                allStrng += "ServerIPv4: " + (string)ConfigurationManager.AppSettings["ServerIPv4"] + Environment.NewLine;
            }
            if (ConfigurationManager.AppSettings["ServerIPv6"] != null)
            {
                this.LiteralServerIPv6.Text = (string)ConfigurationManager.AppSettings["ServerIPv6"];
                allStrng += "ServerIPv6: " + (string)ConfigurationManager.AppSettings["ServerIPv6"] + Environment.NewLine;
            }

            myServerKey = Request.UserHostAddress;
            if (ConfigurationManager.AppSettings["ExternalClientIP"] != null)
                myServerKey = (string)ConfigurationManager.AppSettings["ExternalClientIP"];
            allStrng += "UserHostAddress: " + myServerKey + Environment.NewLine;
            clientIp = IPAddress.Parse(myServerKey);
            
            this.LiteralClientIp.Text = clientIp.ToString();

            myServerKey += Constants.APP_NAME;

            if (Request.Headers["User-Agent"] != null)
                tmpStrg = (string)Request.Headers["User-Agent"];
            else if (Request.Headers["User-Agent:"] != null)
                tmpStrg = (string)Request.Headers["User-Agent:"];
            allStrng += "User-Agent: " + tmpStrg + Environment.NewLine;


            if (Application["lastdecrypted"] != null)
                allStrng += "LastDecrypted: " + (string)Application["lastdecrypted"] + Environment.NewLine;
            if (Application["lastmsg"] != null)
                allStrng += "LastMsg: " + (string)Application["lastmsg"] + Environment.NewLine;


            if (!Page.IsPostBack)
            {
                if (Request.Params["Authorization"] != null)
                    allStrng += "Authorization: " + Request.Params["Authorization"].ToString() + Environment.NewLine;

                if ((Request.Files != null && Request.Files.Count > 0))
                {

                }
                TextBoxLastMsg.Text = string.Empty;

                byte[] bytes = Request.InputStream.ToByteArray();
                string rq = Encoding.UTF8.GetString(bytes);
                if (rq.Contains("TextBoxEncrypted="))
                {
                    rq = rq.Substring(rq.IndexOf("TextBoxEncrypted=") + "TextBoxEncrypted=".Length);
                    if (rq.Contains("ButtonSubmit=Submit"))
                        rq = rq.Substring(0, rq.IndexOf("ButtonSubmit=Submit"));
                    if (rq.Contains("TextBoxLastMsg="))
                        rq = rq.Substring(0, rq.IndexOf("TextBoxLastMsg="));
                    if (rq.Contains("TextBoxDecrypted="))
                        rq = rq.Substring(0, rq.IndexOf("TextBoxDecrypted="));                    
                }
                
                
                Cqr1stServerMsg srv1stMsg = new Cqr1stServerMsg(myServerKey);
                decrypted = string.Empty;
                allStrng += "Msg: " + rq.ToString() + Environment.NewLine;
                Application["lastmsg"] = rq;
                this.TextBoxEncrypted.Text = rq;

                try
                {
                    if (!string.IsNullOrEmpty(rq) && rq.Length >= 8)
                    {
                        myContact = srv1stMsg.NCqr1stSrvMsg(rq);
                        decrypted = myContact.ToJson();                        
                    }
                }
                catch (Exception ex)
                {
                    // hexall = serverMessage.symmPipe.HexStages;
                    this.preOut.InnerText = ex.Message + ex.ToString();
                    Area23Log.LogStatic(ex);
                }


                if (myContact != null && !string.IsNullOrEmpty(decrypted))
                {
                    myContact.Cuid = new Guid();
                    _contacts.Add(myContact);
                    SaveJsonContacts(_contacts);
                    allStrng += "Decrypted: " + decrypted.ToString() + Environment.NewLine;
                    Application["lastdecrypted"] = decrypted;
                }
                
                this.TextBoxDecrypted.Text = decrypted;

                if ((string)Application["lastall"] != null)
                    this.preLast.InnerText = (string)Application["lastall"];

                this.preOut.InnerText = allStrng;                
                Application["lastall"] = allStrng;
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            this.Title = "CqrJd Testform " + DateTime.Now.Ticks;
        }


        protected HashSet<CqrContact> LoadJsonContacts()
        {
            return JsonContacts.LoadJsonContacts();
        }

        protected void SaveJsonContacts(HashSet<CqrContact> contacts)
        {
            JsonContacts.SaveJsonContacts(contacts);
        }

    }
}