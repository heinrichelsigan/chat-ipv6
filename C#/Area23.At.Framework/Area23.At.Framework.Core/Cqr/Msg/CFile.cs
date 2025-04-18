using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Newtonsoft.Json;
using System.Text;

namespace Area23.At.Framework.Core.Cqr.Msg
{
    public class CFile : CContent, IMsgAble
    {

        #region properties 

        public string FileName { get; set; }

        public string Base64Type { get; set; }

        public byte[] Data { get; set; }

        public string Sha256Hash { get; protected internal set; }

        public long FileByteLen { get; protected internal set; }

        public EncodingType EnCodingType { get; internal set; }


        #endregion properties 

        #region ctors

        public CFile() : base()
        {
            FileName = string.Empty;
            Base64Type = string.Empty;
            Md5Hash = string.Empty;
            Sha256Hash = string.Empty;
            Data = new byte[0];
            MsgType = CType.None;
        }


        public CFile(string fileName, byte[] data, string hash = "")
        {
            FileName = fileName;            
            Data = data;
            Base64Type = MimeType.GetMimeType(Data, FileName);
            _hash = hash;
            MsgType = CType.Json;
            Md5Hash = MD5Sum.Hash(Data, "");
            Sha256Hash = "";
            FileByteLen = Data.LongLength;
            EnCodingType = EncodingType.Base64;
        }

        public CFile(string fileName, string mimeType, byte[] data, string hash) : this(fileName, data, hash) 
        {            
            Base64Type = mimeType;         
        }

        public CFile(string fileName, string base64, string hash = "")
        {
            FileName = fileName;
            Data = Convert.FromBase64String(base64);
            Base64Type = MimeType.GetMimeType(Data, FileName);
            _hash = hash;
            Md5Hash = MD5Sum.Hash(Data, "");
            FileByteLen = Data.LongLength;
            Sha256Hash = "";
            MsgType = CType.Json;
            EnCodingType = EncodingType.Base64;
        }

        public CFile(string fileName, string mimeType, string base64, string hash) : this(fileName, base64, hash) 
        {            
            Base64Type = mimeType;            
        }


        public CFile(string fileName, string mimeType, byte[] data, string hash, string sMd5 = "", string sSha256 = "")
             : this(fileName, mimeType, data, hash)
        {
            Md5Hash = sMd5;
            Sha256Hash = sSha256;
        }

        public CFile(string fileName, string mimeType, byte[] data, string hash, string sMd5 = "", string sSha256 = "", CType msgType = CType.Json) :
                this(fileName, mimeType, data, hash, sMd5, sSha256)
        {
            MsgType = msgType;
        }

        public CFile(string fileName, string mimeType, byte[] data, string hash, string sMd5 = "", string sSha256 = "", CType msgType = CType.Json, EncodingType enCodeType = EncodingType.Base64) :
                this(fileName, mimeType, data, hash, sMd5, sSha256, msgType)
        {
            this.EnCodingType = enCodeType;
        }


        public CFile(FileInfo fi, string hash = "") : this()
        {
            FileName = fi.Name;            
            Data = System.IO.File.ReadAllBytes(fi.FullName);
            Base64Type = MimeType.GetMimeType(Data, FileName);
            Md5Hash = MD5Sum.Hash(Data, "");
            FileByteLen = Data.LongLength;
            Sha256Hash = "";
            MsgType = CType.Json;
            EnCodingType = EncodingType.Base64;
            this._hash = hash;
        }


        public CFile(string filePath, string hash = "", CType msgArt = CType.Json)
        {
            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException($"Didn't find a file at filePath = {filePath}");

            FileName = Path.GetFileName(filePath);
            Data = System.IO.File.ReadAllBytes(filePath);
            Base64Type = MimeType.GetMimeType(Data, FileName);
            Md5Hash = MD5Sum.Hash(Data, "");
            Sha256Hash = "";
            MsgType = msgArt;
            EnCodingType = EncodingType.Base64;
            this._hash = hash;

        }
        
        
        /// <summary>
        /// Constructor CqrFile from an json, xml or raw serialized plaintext
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="msgArt"></param>
        public CFile(string plainText, CType msgArt = CType.Json)
        {
            CFile cf = GetCFile(plainText, msgArt);
            FileName = cf.FileName;
            Base64Type = cf.Base64Type;
            Data = cf.Data;
            Md5Hash = cf.Md5Hash;
            Sha256Hash = cf.Sha256Hash;
            FileByteLen = cf.FileByteLen;
            _hash = cf._hash;
            _message = cf._message;
            SerializedMsg = cf.SerializedMsg;
            EnCodingType = cf.EnCodingType;
            MsgType = cf.MsgType;            
            CBytes = cf.CBytes;                        
            Base64Type = cf.Base64Type;
        }

        #endregion ctors



        #region EnDeCrypt+DeSerialize


        public override byte[] EncryptToJsonToBytes(string serverKey)
        {
            string serialized = EncryptToJson(serverKey);
            return Encoding.UTF8.GetBytes(serialized);
        }

        public override string EncryptToJson(string serverKey)
        {
            if (Encrypt(serverKey))
            {
                string serializedJson = ToJson();
                return serializedJson;
            }
            throw new CqrException($"EncryptToJson(string severKey failed");
        }


        public override bool Encrypt(string serverKey)
        {
            try
            {
                string hash = EnDeCodeHelper.KeyToHex(serverKey);
                SymmCipherPipe symmPipe = new SymmCipherPipe(serverKey, hash);
                _hash = symmPipe.PipeString;
                Md5Hash = MD5Sum.HashString(String.Concat(serverKey, _hash, symmPipe.PipeString, _message), "");
                Sha256Hash = Sha256Sum.Hash(this.Data, "");
                FileByteLen = this.Data.LongLength;
                

                byte[] msgBytes = Data;
                byte[] cqrbytes = LibPaths.CqrEncrypt ? symmPipe.MerryGoRoundEncrpyt(msgBytes, serverKey, hash) : msgBytes;
                
                CBytes = cqrbytes;
                Data = new byte[0];
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                return false;
            }
            return true;
        }


        public override CFile? DecryptFromJsonFromBytes(string serverKey, byte[] serializedBytes)
        {
            string serialized = Encoding.UTF8.GetString(serializedBytes);
            CFile? cFile = DecryptFromJson(serverKey, serialized);
            return cFile;
        }


        public override CFile? DecryptFromJson(string serverKey, string serialized = "") 
        {
            if (string.IsNullOrEmpty(serialized))
                serialized = this.SerializedMsg;

            if (serialized.Contains("FileName") && serialized.Contains("Base64Type"))
            {
                CFile? cf = FromJson<CFile>(serialized);
                if (cf != null && Decrypt(serverKey))
                {
                    cf._message = _message;
                    cf.CBytes = CBytes;
                    cf.Data = Data;
                    cf.Md5Hash = Md5Hash;
                    cf.Sha256Hash = Sha256Hash;
                    cf.FileByteLen = FileByteLen;
                    cf.FileName = FileName;
                    cf.EnCodingType = EnCodingType;
                    cf.MsgType = MsgType;
                    cf.SerializedMsg = SerializedMsg;
                    cf._hash = _hash;
                    cf.Base64Type = Base64Type;
                    return cf;
                }
            }
                       

            throw new CqrException($"DecryptFromJson<T>(string severKey, string serialized) failed");
        }


        public override bool Decrypt(string serverKey)
        {
            try
            {
                string hash = EnDeCodeHelper.KeyToHex(serverKey);
                SymmCipherPipe symmPipe = new SymmCipherPipe(serverKey, hash);

                byte[] cipherBytes = CBytes;
                byte[] unroundedMerryBytes = LibPaths.CqrEncrypt ? symmPipe.DecrpytRoundGoMerry(cipherBytes, serverKey, hash) : cipherBytes;


                if (!_hash.Equals(symmPipe.PipeString))
                    throw new CqrException($"Hash: {_hash} doesn't match symmPipe.PipeString: {symmPipe.PipeString}");

                string md5Hash = MD5Sum.HashString(String.Concat(serverKey, _hash, symmPipe.PipeString, _message), "");
                long decByteLen = unroundedMerryBytes.LongLength;
                if (this.FileByteLen != decByteLen)
                {
                    Area23Log.LogStatic($"byte len of decrypted = {decByteLen} != FileByteLen {FileByteLen}.");
                }
                if (!md5Hash.Equals(Md5Hash))
                {
                    ;
                    throw new CqrException($"md5Hash: {md5Hash} doesn't match property Md5Hash: {Md5Hash}");                    
                }
                string sha256Hash = Sha256Sum.Hash(unroundedMerryBytes, "");
                if (!sha256Hash.Equals(this.Sha256Hash))
                {
                    Area23Log.LogStatic($"Sha256 from decrypted = {sha256Hash}, while this.Sha256Hash = {this.Sha256Hash}.");
                    // throw new CqrException($"Sha256: {sha256Hash} doesn't match property Sha256Hash: {Sha256Hash}");
                }

                Data = unroundedMerryBytes;
                CBytes = new byte[0];
            }
            catch (Exception exCrypt)
            {
                CqrException.SetLastException(exCrypt);
                return false;
            }
            return true;
        }



        #endregion EnDeCrypt+DeSerialize


        #region members


        public virtual string ToBase64() => Convert.ToBase64String(Data);

        /// <summary>
        /// Serialize <see cref="CFile"/> to Json Stting
        /// </summary>
        /// <returns>json serialized string</returns>
        public override string ToJson()
        {
            this.SerializedMsg = "";
            string jsonText = JsonConvert.SerializeObject(this);
            this.SerializedMsg = jsonText;
            return jsonText;
        }

        /// <summary>
        /// Generic method to convert back from json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public override T FromJson<T>(string jsonText)
        {
            T t = JsonConvert.DeserializeObject<T>(jsonText);
            if (t != null)
            {
                if (t is CContent mc)
                {
                    this._hash = mc.Hash;
                    this._message = mc.Message;
                    this.SerializedMsg = mc.SerializedMsg;
                    this.MsgType = mc.MsgType;
                    this.Md5Hash = mc.Md5Hash;
                    this.CBytes = mc.CBytes;
                }
                if (t is CFile cf)
                {
                    FileName = cf.FileName;
                    Base64Type = cf.Base64Type;
                    Data = cf.Data;
                    MsgType = CType.Json;
                    Md5Hash = cf.Md5Hash;
                    Sha256Hash = cf.Sha256Hash;
                    _message = cf.Message;
                    SerializedMsg = cf.SerializedMsg;
                    _hash = cf._hash ?? string.Empty;
                }

            }
            return t;
        }

        public virtual string ToXml()
        {
            SerializedMsg = "";
            SerializedMsg = Utils.SerializeToXml<CContent>(this);
            return SerializedMsg;
        }


        public override T FromXml<T>(string xmlText)
        {
            T cqrT = default(T);
            cqrT = base.FromXml<T>(xmlText);
            if (cqrT is CFile cf)
            {
                this.Base64Type = cf.Base64Type;
                this.Data = cf.Data;
                this.Base64Type = cf.Base64Type;
                this.EnCodingType = cf.EnCodingType;
                this.Data = cf.Data;
                MsgType = CType.Xml;
                Md5Hash = cf.Md5Hash;
                Sha256Hash = cf.Sha256Hash;
                this._hash = cf._hash ?? string.Empty;
                this._message = cf._message;
                this.MsgType = cf.MsgType;
                this.SerializedMsg = cf.SerializedMsg;

            }

            return cqrT;

        }


        public CFile GetCFile(string encodedSerilizedOrRawText, CType msgArt = CType.Json)
        {
            if (msgArt == CType.None || msgArt == CType.Raw)
            {
                throw new NotImplementedException("CqrFile GetCqrFile(string encodedSerilizedOrRawText, MsgEnum msgArt = MsgEnum.Json)" +
                    " is not implemented for MsgEnum.None and MsgEnum.RawWithHashAtEnd");
            }
            if (msgArt == CType.Mime)
            {
                Data = new byte[0];

                bool mimeGot = GetByBase64Attachment(encodedSerilizedOrRawText,
                    out string cqrFileName, out string base64Type,
                    out string md5Hash, out string sha256Hash,
                    out string mimeBase64, out string hash);
                if (mimeGot && !string.IsNullOrEmpty(mimeBase64))
                {
                    FileName = cqrFileName;
                    Base64Type = base64Type;
                    Md5Hash = md5Hash;
                    Sha256Hash = sha256Hash;
                    Data = Convert.FromBase64String(mimeBase64);
                    _hash = hash;
                }
            }
            else if (msgArt == CType.Json)
            {
                this.FromJson<CFile>(encodedSerilizedOrRawText);
            }
            else if (msgArt == CType.Xml)
            {
                this.FromXml<CFile>(encodedSerilizedOrRawText);
            }
            return this;
        }


        /// <summary>
        /// Get Html Page embedding CqrFile
        /// </summary>
        /// <returns>html as string rendered</returns>
        public string GetWebPage()
        {
            string base64Mime = Convert.ToBase64String(Data);
            string html = $"<html>\n\t<head>\n\t\t<title>{FileName} {Base64Type}</title>\n\t</head>";
            html += $"\n\t<body>\n\t\t";
            if (MimeType.IsMimeTypeImage(Base64Type))
                html += $"\n\t\t<img src=\"data:{Base64Type};base64,{base64Mime}\" alt=\"{Base64Type} {FileName}\" />";
            if (MimeType.IsMimeTypeDocument(Base64Type))
            {
                html += $"\n\t\t<object data=\"data:{Base64Type};base64,{base64Mime}\" type=\"{Base64Type}\" width=\"640px\" height=\"480px\" >";
                html += $"\n\t\t\t<p>Unable to display {Base64Type} <b>{FileName}</b></p>";
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
            string fileCLen = FileName + " [" + Data.Length + "]";
            return fileCLen;
        }

        #endregion members

        #region static members



        public static void SaveCqrFile(CFile file, string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory {directoryPath} could not be found.");

            string saveFileName = Path.Combine(directoryPath, file.FileName);
            System.IO.File.WriteAllBytes(saveFileName, file.Data);

            return;
        }


        public static CFile LoadCqrFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException($"File {filePath} could not be found.");

            string fileName = Path.GetFileName(filePath);
            byte[] data = System.IO.File.ReadAllBytes(fileName);
            string mimeType = MimeType.GetMimeType(data, fileName);
            CFile cfile = new CFile(fileName, mimeType, data, "");
            return cfile;
        }



        internal static bool GetByBase64Attachment(string plainAttachment,
            out string cqrFileName, out string base64Type,
            out string md5Hash, out string sha256Hash,
            out string mimeBase64, out string _hash)
        {
            string restString = plainAttachment;

            base64Type = restString.GetSubStringByPattern("Content-Type: ", true, "", ";", false);
            cqrFileName = restString.GetSubStringByPattern("; name=\"", true, "", "\";", false);
            string contentLengthString = restString.GetSubStringByPattern("Content-Length: ", true, "", ";\n", false);
            string contentLenString = string.Empty;
            foreach (char ch in contentLengthString.ToCharArray())
                if (Char.IsDigit(ch) || Char.IsNumber(ch) || ch == '.')
                    contentLenString += ch.ToString();
            int contentLen = Int32.Parse(contentLenString);

            _hash = restString.GetSubStringByPattern("Content-Verification: ", true, "", ";", false);
            md5Hash = restString.GetSubStringByPattern("md5=\"", true, "", "\";", false);
            sha256Hash = restString.GetSubStringByPattern("sha256=\"", true, "", "\";", false);

            restString = restString.Substring(restString.IndexOf("Content-Verification: ") + "Content-Verification: ".Length);

            mimeBase64 = restString.Substring(restString.IndexOf(";\n") + ";\n".Length);
            restString = restString.Substring(restString.IndexOf(";\n") + ";\n".Length);

            bool isMimeAttachment = false;
            if (!string.IsNullOrEmpty(mimeBase64) && mimeBase64.Length >= 4)
            {
                try
                {
                    if (restString.EndsWith($"\n{_hash}\0"))
                        mimeBase64 = restString.Substring(0, restString.LastIndexOf($"\n{_hash}\0"));
                    if (restString.EndsWith($"\n{_hash}"))
                        mimeBase64 = restString.Substring(0, restString.LastIndexOf($"\n{_hash}"));
                    if (restString.LastIndexOf("\n") >= (restString.Length - 11))
                        mimeBase64 = restString.Substring(0, restString.LastIndexOf($"\n"));

                    if (mimeBase64.Length < (contentLen + 10) && mimeBase64.Length > (contentLen - 10))
                        isMimeAttachment = true;
                }
                catch (Exception exMime)
                {
                    SLog.Log(exMime);
                }
            }
            return isMimeAttachment;

        }
        #endregion static members
    }
}
