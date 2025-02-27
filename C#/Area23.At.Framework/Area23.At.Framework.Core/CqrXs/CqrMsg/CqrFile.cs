using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Formats.Tar;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Area23.At.Framework.Core.CqrXs.CqrMsg
{

    /// <summary>
    /// Represtents a MimeAttachment
    /// </summary>
    [Serializable]
    public class CqrFile : MsgContent
    {       

        #region properties 

        public string CqrFileName { get; set; }

        public string Base64Type { get; set; }
        
        public byte[] Data { get; set; }
        

        public string Md5Hash { get; set; }
        public string Sha256Hash { get; set; }

        public EncodingType EnCodingType { get; internal set; }

        
        #endregion properties 

        #region ctors

        public CqrFile() : base()
        {
            CqrFileName = string.Empty;
            Base64Type = string.Empty;
            Md5Hash = string.Empty;
            Sha256Hash = string.Empty;
            Data = new byte[0];
        }

        public CqrFile(string fileName, string mimeType, byte[] data, string hash)
        {
            CqrFileName = fileName;
            Base64Type = mimeType;
            Data = data;
            _hash = hash;
            MsgType = MsgEnum.Json;
            Md5Hash = string.Empty;
            Sha256Hash = string.Empty;
            EnCodingType = EncodingType.Base64;
        }

        public CqrFile(string fileName, string mimeType, string base64, string hash)
        {
            CqrFileName = fileName;
            Base64Type = mimeType;
            Data = Convert.FromBase64String(base64);
            _hash = hash;
            Md5Hash = string.Empty;
            Sha256Hash = string.Empty;
            MsgType = MsgEnum.Json;
            EnCodingType = EncodingType.Base64;
        }


        public CqrFile(string fileName, string mimeType, byte[] data, string hash, string sMd5 = "", string sSha256 = "")
             : this(fileName, mimeType, data, hash)
        {
            Md5Hash = sMd5;
            Sha256Hash = sSha256;
        }

        public CqrFile(string fileName, string mimeType, byte[] data, string hash, string sMd5 = "", string sSha256 = "",
            MsgEnum msgType = MsgEnum.Json) :
                this(fileName, mimeType, data, hash, sMd5, sSha256) 
        {
            this.MsgType = msgType;
        }

        public CqrFile(string fileName, string mimeType, byte[] data, string hash, string sMd5 = "", string sSha256 = "", 
            MsgEnum msgType = MsgEnum.Json, EncodingType enCodeType = EncodingType.Base64) :
                this(fileName, mimeType, data, hash, sMd5, sSha256, msgType)
        {
            this.EnCodingType = enCodeType;
        }


        /// <summary>
        /// Constructor CqrFile from an json, xml or raw serialized plaintext
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="msgArt"></param>
        public CqrFile(string plainText, MsgEnum msgArt = MsgEnum.Json)
        {
            if (msgArt == MsgEnum.None || msgArt == MsgEnum.RawWithHashAtEnd)
            {
                MimeAttachment mimeAttachment = MimeAttachment.GetBase64Attachment(plainText);
                Base64Type = mimeAttachment.Base64Type;
                CqrFileName = mimeAttachment.FileName;
                Md5Hash = mimeAttachment.Md5Hash;
                Sha256Hash = mimeAttachment.Sha256Hash;
                MsgType = mimeAttachment.MsgType;
                Data = Convert.FromBase64String(mimeAttachment.Base64Mime);
                _hash = mimeAttachment.Hash;
            }
            else if (msgArt == MsgEnum.Json)
            {
                this.FromJson<CqrFile>(plainText);
            }
            else if (msgArt == MsgEnum.Xml)
            {
                CqrFile? f = Ext.DeserializeFromXml<CqrFile>(plainText);
                if (f != null)
                {
                    this.CqrFileName = f.CqrFileName;
                    this.Base64Type = f.Base64Type;
                    this.Data = f.Data;
                    this._rawMessage = f._rawMessage;
                    this._hash = f._hash;
                    this.Md5Hash = f.Md5Hash;
                    this.Sha256Hash = f.Sha256Hash;
                    this._message = f.Message;
                    EnCodingType = f.EnCodingType;
                    MsgType = MsgEnum.Xml;
                }
            }
        }

        #endregion ctors

        #region members

        public virtual string ToBase64() => Convert.ToBase64String(Data);       

        /// <summary>
        /// Serialize <see cref="CqrFile"/> to Json Stting
        /// </summary>
        /// <returns></returns>
        public override string ToJson()
        {
            string jsonText = JsonConvert.SerializeObject(this);
            return jsonText;
        }
        

        /// <summary>
        /// Generic method to convert back from json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public override T? FromJson<T>(string jsonText) where T : default
        {
            T? t = JsonConvert.DeserializeObject<T>(jsonText);
            if (t != null)
            {                
                if (t is MsgContent mc)
                {
                    this._hash = mc.Hash;
                    this._message = mc.Message;
                    this._rawMessage = mc.RawMessage;
                    this.MsgType = mc.MsgType;
                }
                if (t is MimeAttachment ma)
                {                    
                    this.Data = Convert.FromBase64String(ma.Base64Mime);
                    this.Base64Type = ma.Base64Type;
                    this.CqrFileName = ma.FileName;
                    this.Md5Hash = ma.Md5Hash;
                    this.Sha256Hash = ma.Sha256Hash;
                    this._hash = ma._hash;
                }
                if (t is CqrFile cf)
                {
                    CqrFileName = cf.CqrFileName;
                    Base64Type = cf.Base64Type;
                    Data = cf.Data;
                    Md5Hash = cf.Md5Hash;
                    Sha256Hash = cf.Sha256Hash;
                    EnCodingType = cf.EnCodingType;
                    MsgType = MsgEnum.Json;
                    _message = cf.Message;
                    _rawMessage = cf.RawMessage;
                    _hash = cf._hash ?? string.Empty;
                }

            }
            return t;
        }



        /// <summary>
        /// Get Html Page embedding CqrFile
        /// </summary>
        /// <returns>html as string rendered</returns>
        public string GetWebPage()
        {
            string base64Mime = Convert.ToBase64String(Data);
            string html = $"<html>\n\t<head>\n\t\t<title>{CqrFileName} {Base64Type}</title>\n\t</head>";
            html += $"\n\t<body>\n\t\t";
            if (MimeType.IsMimeTypeImage(Base64Type))
                html += $"\n\t\t<img src=\"data:{Base64Type};base64,{base64Mime}\" alt=\"{Base64Type} {CqrFileName}\" />";
            if (MimeType.IsMimeTypeDocument(Base64Type))
            {
                html += $"\n\t\t<object data=\"data:{Base64Type};base64,{base64Mime}\" type=\"{Base64Type}\" width=\"640px\" height=\"480px\" >";
                html += $"\n\t\t\t<p>Unable to display {Base64Type} <b>{CqrFileName}</b></p>";
                html += $"\n\t\t</object>";
            }
            if (MimeType.IsMimeTypeAudio(Base64Type))
            {
                html += $"\n\t\t<audio controls>";
                html += $"\n\t\t\t<source src=\"data:{Base64Type};base64,{base64Mime}\" type=\"{Base64Type}\">";
                html += $"\n\t\tYour browser does not support the audio element.";
                html += $"\n\t\t</audio>";
            }
            if (MimeType.IsMimeTypeVideo(Base64Type))
            {
                html += $"\n\t\t<video width=\"320\" height=\"240\" controls>";
                html += $"\n\t\t\t<source src=\"data:{Base64Type};base64,{base64Mime}\" type=\"{Base64Type}\">";
                html += $"\n\t\tYour browser does not support the video tag.";
                html += $"\n\t\t</video>";
            }
            html += $"\n\t</body>\n\t\t";
            html += $"\n</html>\n";


            return html;
        }


        /// <summary>
        /// GetFileNameContentLength write <see cref="CqrFileName"/> and <see cref="Data.Length"/>
        /// </summary>
        /// <returns>CqrFileName + " [" + Data.Length + "]";</returns>
        public string GetFileNameContentLength()
        {
            string fileCLen = CqrFileName + " [" + Data.Length + "]";
            return fileCLen;
        }


        #endregion members

        #region static members

        public static void SaveCqrFile(CqrFile file, string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory {directoryPath} could not be found.");

            string saveFileName = Path.Combine(directoryPath, file.CqrFileName);
            File.WriteAllBytes(saveFileName, file.Data);

            return;
        }


        public static CqrFile LoadCqrFile(string cqrFileName)
        {
            if (!File.Exists(cqrFileName))
                throw new FileNotFoundException($"File {cqrFileName} could not be found.");


            string fileName = Path.GetFileName(cqrFileName);
            byte[] data = File.ReadAllBytes(cqrFileName);
            string mimeType = MimeType.GetMimeType(data, fileName);
            CqrFile cfile = new CqrFile(fileName, mimeType, data, "");
            return cfile;

        }

        #endregion static members
    }


}
