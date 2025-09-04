using Area23.At.Framework.Library.Cqr.Msg;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;


namespace EU.CqrXs.Srv.Util
{

    /// <summary>
    /// CqrJdBasePage is base page of Default
    /// </summary>
    public abstract class CqrJdBasePage : System.Web.UI.Page
    {
        protected System.Collections.Generic.Queue<string> mqueue = new Queue<string>();
        protected Uri area23AtUrl = new Uri("https://area23.at/");
        protected Uri cqrXsEuUrl = new Uri("https://srv.cqrxs.eu/v1.1/");
        protected Uri githubUrl = new Uri("https://github.com/heinrichelsigan/chat-ipv6/");
        protected internal global::System.Web.UI.WebControls.Image ImageQr;
        protected internal System.Globalization.CultureInfo locale;
        protected global::System.Web.UI.HtmlControls.HtmlAnchor HrefShort;

        protected internal ushort initState = 0x0;
        protected internal string hashKey = string.Empty;
        protected internal string tmpStrg = string.Empty;
        protected internal string allStrng = string.Empty;
        protected internal string decrypted = string.Empty;
        protected internal object _lock = new object();
        protected internal string myServerKey = string.Empty;
        protected internal HashSet<CContact> _contacts;
        protected internal CContact myContact = null;
        // protected internal IPAddress clientIp;


        public System.Globalization.CultureInfo Locale
        {
            get
            {
                if (locale == null)
                {
                    try
                    {
                        string defaultLang = Request.Headers["Accept-Language"].ToString();
                        string firstLang = defaultLang.Split(',').FirstOrDefault();
                        defaultLang = string.IsNullOrEmpty(firstLang) ? "en" : firstLang;
                        locale = new System.Globalization.CultureInfo(defaultLang);
                    }
                    catch (Exception)
                    {
                        locale = new System.Globalization.CultureInfo("en");
                    }
                }
                return locale;
            }
        }

        public string SepChar { get => System.IO.Path.DirectorySeparatorChar.ToString(); }

        protected CqrJdBasePage() : base() 
        {
            initState = 0x1;
        }

        protected virtual void Page_Init(object sender, EventArgs e)
        {

            myServerKey = GetServerKey();
            allStrng = string.Empty;
            myContact = null;
            if (Application[Constants.JSON_CONTACTS] != null)
                _contacts = (HashSet<CContact>)(Application[Constants.JSON_CONTACTS]);
            else
                _contacts = LoadJsonContacts();

            initState = 0x2;
        }


        /// <summary>
        /// AuthHtPasswd - authenticates a user against .htpasswd in apache2
        /// </summary>
        /// <param name="user"><see cref="string"/> username</param>
        /// <param name="passwd"><see cref="string"/>password</param>
        /// <returns>true on successful authentificaton, otherwise false</returns>
        protected virtual bool AuthHtPasswd(string user, string passwd)
        {

            bool authTypeBasic = false, authBasicProviderFile = false;
            string directoryPath = "", htAccessFile = "", authFile = "", requireUser = "";
            
            string phyAppPath = Request.PhysicalPath;
            DirectoryInfo dirInfo;
            int lastPathSeperator = -1;
            
            if ((phyAppPath.Contains(Path.DirectorySeparatorChar)) &&
                ((lastPathSeperator = phyAppPath.LastIndexOf(Path.DirectorySeparatorChar)) > 0))
                    directoryPath = phyAppPath.Substring(0, lastPathSeperator);
            
            if (string.IsNullOrEmpty(directoryPath))
            {
                dirInfo = Directory.GetParent(phyAppPath);
                directoryPath = dirInfo.FullName;
            }

            if (!Directory.Exists(directoryPath)) 
            {
                Area23Log.LogOriginMsg("CqrJdBasePage", "return false! \tdirectory " + directoryPath + " does not exist!\n");
                return false;
            }

            htAccessFile = Path.Combine(directoryPath, ".htaccess");
            if (!File.Exists(htAccessFile))
            {
                Area23Log.LogOriginMsg("CqrJdBasePage", "return true; \t.htaccess file " + htAccessFile + " does not exist!\n");
                return true;
            }
            

            List<string> lines = File.ReadLines(htAccessFile).ToList();
            foreach (string line in lines)
            {
                if (line.StartsWith("AuthType Basic", StringComparison.CurrentCultureIgnoreCase))
                    authTypeBasic = true;
                if (line.StartsWith("AuthBasicProvider file", StringComparison.CurrentCultureIgnoreCase))
                    authBasicProviderFile = true;
                if (line.StartsWith("AuthUserFile ", StringComparison.CurrentCultureIgnoreCase))
                    authFile = line.Replace("AuthUserFile ", "").Replace("\"", "");
                if (line.StartsWith("Require user ", StringComparison.CurrentCultureIgnoreCase))
                    requireUser = line.Replace("Require user ", "");
            }

            if (!authTypeBasic && !authBasicProviderFile)
            {
                Area23Log.LogOriginMsg("CqrJdBasePage", "return false! \tauthTypeBasic = " + authTypeBasic + "; authBasicProviderFile = " + authBasicProviderFile + ";\n");
                return false;
            }

            if (!string.IsNullOrEmpty(requireUser) && !user.Equals(requireUser, StringComparison.CurrentCultureIgnoreCase))
            {
                Area23Log.LogOriginMsg("CqrJdBasePage", "return false! \trequireUser = " + requireUser + " NOT EQUALS user = " + user + "!\n");
                return false;
            }


            if (!string.IsNullOrEmpty(authFile) && File.Exists(authFile))
            {
                string consoleOut = "", consoleError = "";
                string passedthrough = ProcessCmd.ExecuteWithOutAndErr(
                    "htpasswd", 
                    String.Format(" -b -v {0} {1} {2} ", authFile, user, passwd), 
                    out consoleOut, 
                    out consoleError, 
                    false);

                Area23Log.LogOriginMsg("CqrJdBasePage", "passedthrough = \t$(htpasswd" + String.Format(" -b -v {0} {1} {2})", authFile, user, passwd));
                Area23Log.LogOriginMsg("CqrJdBasePage", "passedthrough = \t" + passedthrough);
                string userMatch = string.Format("Password for user {0} correct.", user);
                if (passedthrough.EndsWith(userMatch, StringComparison.CurrentCultureIgnoreCase) ||
                    passedthrough.Contains(userMatch))
                {
                    Area23Log.LogOriginMsg("CqrJdBasePage", "return true; \t[" + passedthrough + "] matches {" + userMatch + "}.\n");
                    return true;
                }
                else
                {
                    Area23Log.LogOriginMsg("CqrJdBasePage", "return false! \t[" + passedthrough + "] not matching {" + userMatch + "}.\n");
                    return false;
                }
                    
            }

            Area23Log.LogOriginMsg("CqrJdBasePage", "return true; \tfall through.\n");
            return true;
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if (initState < 0x2 || _contacts == null) 
            { 
                Page_Init(sender, e); 
            }
            
            
            // clientIp = GetClientExternalIp();
            myServerKey = GetServerKey();
            allStrng = "UserHostAddress: " + myServerKey.Replace(Constants.APP_NAME, "") + Environment.NewLine;
            // myServerKey += Constants.APP_NAME;

            if (Request.Headers["User-Agent"] != null)
                tmpStrg = (string)Request.Headers["User-Agent"];
            else if (Request.Headers["User-Agent:"] != null)
                tmpStrg = (string)Request.Headers["User-Agent:"];
            allStrng += "User-Agent: " + tmpStrg + Environment.NewLine;


            if (Application["lastdecrypted"] != null)
                allStrng += "LastDecrypted: " + (string)Application["lastdecrypted"] + Environment.NewLine;
            if (Application["lastmsg"] != null)
                allStrng += "LastMsg: " + (string)Application["lastmsg"] + Environment.NewLine;

            try
            {
                if (Request.Params["Authorization"] != null)
                    allStrng += "Authorization: " + Request.Params["Authorization"].ToString() + Environment.NewLine;

                if ((Request.Files != null && Request.Files.Count > 0))
                {

                }
            } 
            catch (Exception ex)
            {
                Area23Log.LogOriginMsgEx("CqrJdBasePage", "Page_Load", ex);
            }

            initState = 0x4;

        }



        protected string GetServerKey()
        {
            // _serverKey = Constants.AUTHOR_EMAIL;            

            if (ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP] != null)
            {
                myServerKey = (string)ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP];
            }
            else
                myServerKey = HttpContext.Current.Request.UserHostAddress;
            myServerKey += Constants.APP_NAME;

            return myServerKey;
        }

        public virtual IPAddress GetClientExternalIp()
        {
            string externalClientIp = (ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP] != null) ?
                (string)ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP] : null;

            string externalClientIpv4 = (ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP_V4] != null) ?
               (string)ConfigurationManager.AppSettings[Constants.EXTERNAL_CLIENT_IP_V4] : null;

            if (string.IsNullOrEmpty(externalClientIp))
                externalClientIp = Request.UserHostAddress;

            if (!IPAddress.TryParse(externalClientIp, out IPAddress clientIpAddr))
            {
                string reducedExternal = "";
                foreach (char ch in externalClientIp) {
                    if ((ch >= '0' && ch <= '9') ||
                        (ch >= 'a' && ch <= 'f') ||
                        (ch >= 'A' && ch <= 'F') ||
                        (ch == '.' && ch == ':'))
                    {
                        reducedExternal += ch;
                    }
                }
                externalClientIp = reducedExternal;
                if (externalClientIp.Contains(":"))
                {
                    List<byte> bytes = new List<byte>();
                    string[] segments = externalClientIp.Split(':');
                    foreach (string segement in segments)
                    {
                        foreach (char c in segement)
                        {
                            byte b = Convert.ToByte(c);
                            bytes.Add(b);
                        }
                    }
                    clientIpAddr = new IPAddress(bytes.ToArray());
                }
                else 
                    clientIpAddr = IPAddress.Parse(externalClientIpv4);                  
            }

            return clientIpAddr;
        }


        public virtual void InitURLBase()
        {
            area23AtUrl = new Uri("https://area23.at/net/");
            cqrXsEuUrl = new Uri("https://srv.cqrxs.eu/v1.1//");
            githubUrl = new Uri("https://github.com/heinrichelsigan/chat-ipv6/");
        }





        public virtual void Log(string msg)
        {
            Area23Log.LogOriginMsg("CqrJdBasePage", msg);
        }




        protected virtual CContact FindContactByNameEmail(HashSet<CContact> cHashSet, CContact searchContact)
        {
            CContact cqrFoundContact = JsonContacts.FindContactByNameEmail(cHashSet, searchContact);
            return cqrFoundContact;
        }



        protected virtual HashSet<CContact> LoadJsonContacts()
        {
            return JsonContacts.LoadJsonContacts();
        }

        protected virtual void SaveJsonContacts(HashSet<CContact> contacts)
        {
            JsonContacts.SaveJsonContacts(contacts);
        }




        /// <summary>
        /// Get Image mime type for image bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetMimeTypeForImageBytes(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
            {
                return ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == img.RawFormat.Guid).MimeType;
            }
        }


        /// <summary>
        /// Saves a byte[] to a fileName
        /// </summary>
        /// <param name="bytes">byte[] of raw data</param>
        /// <param name="outMsg"></param>
        /// <param name="fileName">filename to save</param>
        /// <param name="directoryName">directory where to save file</param>
        /// <returns>fileName under which it was saved really</returns>
        protected virtual string ByteArrayToFile(byte[] bytes, out string outMsg, string fileName = null, string directoryName = null)
        {
            outMsg = String.Empty;
            string strPath = LibPaths.SystemDirOutPath;
            if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName))
            {
                strPath = directoryName;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Constants.DateFile + Guid.NewGuid().ToString();
            }
            string ext = "hex";
            fileName += (fileName.LastIndexOf(".") > -1) ? "" : "." + ext;

            string newFileName = fileName.BeautifyUploadFileNames();

            try
            {
                while (System.IO.File.Exists(strPath + fileName))
                {
                    newFileName = fileName.Contains(Constants.DateFile) ?
                        Constants.DateFile + Guid.NewGuid().ToString() + "_" + fileName :
                        Constants.DateFile + fileName;
                    outMsg = String.Format("{0} already exists on server, saving it to {1}", fileName, newFileName);
                    fileName = newFileName;
                }
                using (var fs = new FileStream(strPath + fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Area23Log.LogStatic(ex);
            }

            if (System.IO.File.Exists(strPath + fileName))
            {
                string mimeType = MimeType.GetMimeType(bytes, strPath + fileName);
                if (fileName.EndsWith("tmp"))
                {
                    string extR = MimeType.GetFileExtForMimeTypeApache(mimeType);
                    if (extR.ToLowerInvariant().Equals("hex") || extR.ToLowerInvariant().Equals("oct"))
                        newFileName = fileName;
                    else
                    {
                        newFileName = fileName.Replace("tmp", extR);
                        System.IO.File.Move(strPath + fileName, LibPaths.SystemDirOutPath + newFileName);
                    }

                    outMsg = newFileName;
                    return newFileName;
                }
                else
                {
                    outMsg = fileName;
                    return fileName;
                }
            }

            outMsg = null;
            return null;
        }

        protected virtual string StringToFile(string encodedText, out string outMsg, string fileName = null, string directoryName = null)
        {
            string strPath = LibPaths.SystemDirOutPath;
            if (!String.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName))
            {
                strPath = directoryName;
            }
            outMsg = String.Empty;
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Constants.DateFile + Guid.NewGuid().ToString();
            }
            string ext = "hex";

            fileName += (fileName.LastIndexOf(".") > -1) ? "" : "." + ext;

            string newFileName = fileName.BeautifyUploadFileNames();

            // strPath = LibPaths.SystemDirOutPath + fileName;
            try
            {
                while (System.IO.File.Exists(strPath + fileName))
                {
                    newFileName = fileName.Contains(Constants.DateFile) ?
                        Constants.DateFile + Guid.NewGuid().ToString() + "_" + fileName :
                        Constants.DateFile + fileName;
                    // strPath = LibPaths.SystemDirOutPath + newFileName;
                    outMsg = String.Format("{0} already exists on server, saving it to {1}", fileName, newFileName);
                    fileName = newFileName;
                }
                File.WriteAllText(strPath + fileName, encodedText, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Area23Log.LogStatic(ex);
            }

            if (System.IO.File.Exists(strPath + fileName))
            {
                outMsg = fileName;
                return fileName;
            }
            outMsg = null;
            return null;
        }

        protected virtual byte[] GetFileByteArray(string filename)
        {
            FileStream oFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);

            // Create a byte array of file size.
            byte[] FileByteArrayData = new byte[oFileStream.Length];

            //Read file in bytes from stream into the byte array
            oFileStream.Read(FileByteArrayData, 0, System.Convert.ToInt32(oFileStream.Length));

            //Close the File Stream
            oFileStream.Close();

            return FileByteArrayData; //return the byte data
        }



    }

}