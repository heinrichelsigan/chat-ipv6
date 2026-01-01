using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Zfx;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Text;

namespace Area23.At.Framework.Core.Crypt.Cipher.Symmetric
{

    /// <summary>
    /// Provides a simple crypt pipe for <see cref="SymmCipherEnum"/>
    /// </summary>
    public class SymmCipherPipe : CipherPipe
    {

        private ZipType zType = ZipType.None;
        private SymmCipherEnum[] inPipe;
        // internal readonly SymmCipherEnum[] outPipe;
        private EncodingType encodeType = EncodingType.Base64;
        private KeyHash kHash = KeyHash.Hex;
        // private readonly string pipeString;
        private string symmCipherKey = "", symmCipherHash = "";

        /// <summary>
        /// ZType is current <see cref="ZipType"/>
        /// </summary>
        public new ZipType ZType { get => zType; internal set => zType = value; }

        /// <summary>
        /// Current <see cref="EncodeType"/> 
        /// </summary>
        public new EncodingType EncodeType { get => encodeType; internal set => encodeType = value; }

        /// <summary>
        /// KHash is <see cref="KeyHash"/>
        /// </summary>
        public new KeyHash KHash { get => kHash; internal set => kHash = value; }


        public new SymmCipherEnum[] InPipe { get => inPipe; set => inPipe = value; }    
        public new SymmCipherEnum[] OutPipe { get => new List<SymmCipherEnum>(InPipe).Reverse<SymmCipherEnum>().ToArray(); }


        public new string PipeString
        {
            get
            {
                string pipeString = string.Empty;
                foreach (SymmCipherEnum symmCipher in inPipe)
                    pipeString += symmCipher.GetSymmCipherChar();
                return pipeString;
            }
        }

        //#if DEBUG
        //        public Dictionary<SymmCipherEnum, byte[]> stageDictionary = new Dictionary<SymmCipherEnum, byte[]>();

        //        public string HexStages
        //        {
        //            get
        //            {
        //                string hexOut = string.Empty;
        //                foreach (var stage in stageDictionary)
        //                {
        //                    hexOut += stage.Key.ToString() + "\r\n" + Hex16.ToHex16(stage.Value) + "\r\n";
        //                }

        //                return hexOut;
        //            }
        //        }
        //#endif

        #region ctor SymmCipherPipe

        public SymmCipherPipe()
        {
            inPipe = (new List<SymmCipherEnum>()).ToArray();
            encodeType = EncodingType.Base64;
            kHash = KeyHash.Hex;
            zType = ZipType.None;

            Area23Log.LogOriginMsg("SymmCipherPipe", $"Generating symmetric cipher pipe: {PipeString}, encoding = {encodeType}, zipping={zType}, hashing={kHash}");
        }

        /// <summary>
        /// SymmCipherPipe constructor with an array of <see cref="T:SymmCipherEnum[]"/> as inpipe
        /// </summary>
        /// <param name="symmCipherEnums">array of <see cref="T:SymmCipherEnum[]"/> as inpipe</param>
        public SymmCipherPipe(SymmCipherEnum[] symmCipherEnums, uint maxpipe = 8, 
            EncodingType encType = EncodingType.Base64, ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex)
        {
            // What ever is entered here as parameter, maxpipe has to be not greater 8, because of no such agency
            maxpipe = (maxpipe > Constants.MAX_PIPE_LEN) ? Constants.MAX_PIPE_LEN : maxpipe; // if somebody wants more, he/she/it gets less

            int isize = Math.Min(((int)symmCipherEnums.Length), ((int)maxpipe));
            inPipe = new SymmCipherEnum[isize];
            Array.Copy(symmCipherEnums, inPipe, isize);
           
            zType = zpType;
            encodeType = encType;
            kHash = kh;

            Area23Log.LogOriginMsg("SymmCipherPipe", $"Generating symmetric cipher pipe: {PipeString}, encoding = {encType}, zipping={zpType}, hashing={kh}");

            //if (inPipe.Length > maxpipe)
            //{
            //    List<string> pipElems = new List<string>(inPipe.Length);
            //    foreach (var cipherEnum in inPipe)
            //        pipElems.Add(cipherEnum.ToString());                
            //    throw new ArgumentException($"Pipe \"{string.Join(";", pipElems.ToArray())}\" length exceeds {maxpipe}!");
            //}

            //foreach (SymmCipherEnum symmCipher in inPipe)
            //    pipeString += symmCipher.GetSymmCipherChar();
        }

        /// <summary>
        /// SymmCipherPipe constructor with an array of <see cref="T:string[]"/> as inpipe
        /// </summary>
        /// <param name="symmCipherAlgos">array of <see cref="T:string[]"/> as inpipe</param>
        public SymmCipherPipe(string[] symmCipherAlgos, uint maxpipe = 8, 
            EncodingType encType = EncodingType.Base64, ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex)
        {
            // What ever is entered here as parameter, maxpipe has to be not greater 8, because of no such agency
            maxpipe = (maxpipe > Constants.MAX_PIPE_LEN) ? Constants.MAX_PIPE_LEN : maxpipe; // if somebody wants more, he/she/it gets less
            int maxCnt = 0; 
            List<SymmCipherEnum> symmCipherEnums = new List<SymmCipherEnum>();
            foreach (string algo in symmCipherAlgos)
            {
                if (!string.IsNullOrEmpty(algo))
                {
                    SymmCipherEnum cipherAlgo = SymmCipherEnum.Aes;
                    if (!Enum.TryParse<SymmCipherEnum>(algo, out cipherAlgo))
                        cipherAlgo = SymmCipherEnum.Aes;
                    
                    symmCipherEnums.Add(cipherAlgo);
                    if (++maxCnt > maxpipe)
                        break;
                }
            }

            inPipe = symmCipherEnums.ToArray();

            encodeType = encType;
            kHash = kh;
            zType = zpType;

            // Area23Log.LogOriginMsg("SymmCipherPipe", $"Generating symmetric cipher pipe: {PipeString}, encoding = {encType}, zipping={zpType}, hashing={kh}");
        }

        /// <summary>
        /// SymmCipherPipe ctor with array of user key bytes
        /// </summary>
        /// <param name="keyBytes">user key bytes</param>
        /// <param name="maxpipe">maximum lentgh <see cref="Constants.MAX_PIPE_LEN"/></param>
        public SymmCipherPipe(byte[] keyBytes, uint maxpipe = 8, 
            EncodingType encType = EncodingType.Base64, ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex)
        {
            // What ever is entered here as parameter, maxpipe has to be not greater 8, because of no such agency
            maxpipe = (maxpipe > Constants.MAX_PIPE_LEN) ? Constants.MAX_PIPE_LEN : maxpipe; // if somebody wants more, he/she/it gets less

            ushort scnt = 0;
            List<SymmCipherEnum> pipeList = new List<SymmCipherEnum>();
            Dictionary<byte, SymmCipherEnum> symDict = SymmCipherEnumExtensions.GetByteSymmCipherDict();

            string hexString = string.Empty;
            HashSet<byte> hashBytes = new HashSet<byte>();
            for (int bc = 0; (bc < keyBytes.Length && hashBytes.Count < maxpipe); bc++)
            { 
                byte msb = (byte)(keyBytes[bc] / 0x10);
                byte lsb = (byte)(keyBytes[bc] % 0x10);
                if (!hashBytes.Contains(msb))
                {
                    hashBytes.Add(msb);
                    pipeList.Add(symDict[msb]);
                }
                if (!hashBytes.Contains(lsb))
                {
                    hashBytes.Add(lsb);
                    pipeList.Add(symDict[lsb]);
                }
            }


            inPipe = pipeList.ToArray();

            encodeType = encType;
            kHash = kh;
            zType = zpType;

            // Area23Log.LogOriginMsg("SymmCipherPipe", $"Generating symmetric cipher pipe: {PipeString}, encoding = {encType}, zipping={zpType}, hashing={kh}");
        }

        /// <summary>
        /// Constructs a <see cref="SymmCipherPipe"/> from key and hash
        /// by getting <see cref="T:byte[]">byte[] keybytes</see> with <see cref="CryptHelper.GetUserKeyBytes(string, string, int)"/>
        /// </summary>
        /// <param name="key">secret key to generate pipe</param>
        /// <param name="hash">hash value of secret key</param>
        public SymmCipherPipe(string key, string hash, 
                            EncodingType encType = EncodingType.Base64, 
                            ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex)
            : this(CryptHelper.GetKeyBytesSimple(key, hash, 16), Constants.MAX_PIPE_LEN, encType, zpType, kh)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            symmCipherKey = key;
            symmCipherHash = (string.IsNullOrEmpty(hash)) ? kHash.Hash(key) : hash;
        }

        /// <summary>
        /// SymmCipherPipe ctor with only key
        /// </summary>
        /// <param name="key"></param>
        public SymmCipherPipe(string key)
            : this(key, EnDeCodeHelper.KeyToHex(key))
        {
            symmCipherKey = key;
        }

        #endregion ctor SymmCipherPipe

        #region json

        /// <summary>
        /// ToJson 
        /// </summary>
        /// <returns>serialized string</returns>
        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

        /// <summary>
        /// FromJson
        /// </summary>
        /// <param name="json">serialized json</param>
        /// <returns><see cref="CipherPipe"/></returns>
        public new SymmCipherPipe FromJson(string json)
        {
            SymmCipherPipe pipe = JsonConvert.DeserializeObject<SymmCipherPipe>(json);
            if (pipe == null)
            {
                this.inPipe = pipe.InPipe;
                this.encodeType = pipe.EncodeType;
                this.kHash = pipe.KHash;
                this.zType = pipe.ZType;
                this.symmCipherKey = pipe.symmCipherKey;
                this.symmCipherHash = pipe.symmCipherHash;
            }
            return pipe;
        }

        #endregion json

        #region static members EncryptBytesFast DecryptBytesFast

        /// <summary>
        /// Generic encrypt bytes to bytes
        /// </summary>
        /// <param name="inBytes">Array of byte</param>
        /// <param name="cipherAlgo"><see cref="SymmCipherEnum"/> both symmetric and asymetric cipher algorithms</param>
        /// <param name="secretKey">secret key to decrypt</param>
        /// <param name="hashIv">key's hash</param>
        /// <returns>encrypted byte Array</returns>
        public static byte[] EncryptBytesFast(byte[] inBytes, SymmCipherEnum cipherAlgo,
            string secretKey, string hashIv)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("seretkey");
            if (string.IsNullOrEmpty(hashIv))
                throw new ArgumentNullException("hashIv");

            byte[] encryptBytes = inBytes;
            
            CryptParamsPrefered cpParams = new CryptParamsPrefered(cipherAlgo, secretKey, hashIv);
            Symmetric.CryptBounceCastle cryptBounceCastle = new Symmetric.CryptBounceCastle(cpParams, true);
            encryptBytes = cryptBounceCastle.Encrypt(inBytes);

            return encryptBytes;
        }

        /// <summary>
        /// Generic decrypt bytes to bytes
        /// </summary>
        /// <param name="cipherBytes">Encrypted array of byte</param>
        /// <param name="cipherAlgo"><see cref="SymmCipherEnum"/>both symmetric and asymetric cipher algorithms</param>
        /// <param name="secretKey">secret key to decrypt</param>
        /// <param name="hashIv">key's hash</param>
        /// <returns>decrypted byte Array</returns>
        public static byte[] DecryptBytesFast(byte[] cipherBytes, SymmCipherEnum cipherAlgo,
            string secretKey, string hashIv)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("seretkey");
            if (string.IsNullOrEmpty(hashIv))
                throw new ArgumentNullException("hashIv");

            byte[] decryptBytes = cipherBytes;

            CryptParamsPrefered cpParams = new CryptParamsPrefered(cipherAlgo, secretKey, hashIv);
            Symmetric.CryptBounceCastle cryptBounceCastle = new Symmetric.CryptBounceCastle(cpParams, true);
            decryptBytes = cryptBounceCastle.Decrypt(cipherBytes);

            return EnDeCodeHelper.GetBytesTrimNulls(decryptBytes);
        }

		#endregion static members EncryptBytesFast DecryptBytesFast

		#region multiple rounds en-de-cryption

		/// <summary>
		/// MerryGoRoundEncrpyt starts merry to go arround from left to right in clock hour cycle
		/// </summary>
		/// <param name="inBytes">plain <see cref="T:byte[]"/> to encrypt</param>
		/// <param name="secretKey">user secret key to use for all symmetric cipher algorithms in the pipe</param>
		/// <param name="hashIv">hash key iv relational to secret key</param>
		/// <param name="zipBefore"><see cref="ZipType"/> and <see cref="ZipTypeExtensions.Zip(ZipType, byte[])"/></param>
		/// <returns>encrypted byte[]</returns>
		public override byte[] MerryGoRoundEncrpyt(byte[] inBytes, string secretKey = "", string hashIv = "", ZipType zipBefore = ZipType.None)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(symmCipherKey))
                throw new ArgumentNullException("secretKey");
            if (string.IsNullOrEmpty(hashIv) && string.IsNullOrEmpty(symmCipherHash))
                hashIv = (!string.IsNullOrEmpty(hashIv)) ? hashIv : (kHash != null) ? kHash.Hash(secretKey) : EnDeCodeHelper.KeyToHex(secretKey);
            symmCipherKey = secretKey;
            symmCipherHash = hashIv;

            byte[] encryptedBytes = new byte[inBytes.Length];
            Array.Copy(inBytes, 0, encryptedBytes, 0, inBytes.Length);
//#if DEBUG
//            stageDictionary = new Dictionary<SymmCipherEnum, byte[]>();
//            // stageDictionary.Add(SymmCipherEnum.ZenMatrix, inBytes);
//#endif
            if (zipBefore != ZipType.None)
            {
                encryptedBytes = zipBefore.Zip(inBytes);
                inBytes = encryptedBytes;
            }

            foreach (SymmCipherEnum symmCipher in InPipe)
            {
                encryptedBytes = EncryptBytesFast(inBytes, symmCipher, secretKey, hashIv);
                inBytes = encryptedBytes;
//#if DEBUG
//                stageDictionary.Add(symmCipher, encryptedBytes);
//#endif
            }

            return encryptedBytes;
        }

        /// <summary>
        /// DecrpytRoundGoMerry against clock turn -
        /// starts merry to turn arround from right to left against clock hour cycle 
        /// </summary>
        /// <param name="cipherBytes">encrypted byte array</param>
        /// <param name="secretKey">user secret key, normally email address</param>
        /// <param name="hashIv">hash relational to secret kay</param>
        /// <param name="unzipAfter"><see cref="ZipType"/> and <see cref="ZipTypeExtensions.Unzip(ZipType, byte[])"/></param>
        /// <returns><see cref="T:byte[]"/> plain bytes</returns>
        public override byte[] DecrpytRoundGoMerry(byte[] cipherBytes, string secretKey = "", string hashIv = "", ZipType unzipAfter = ZipType.None)
        {
            if (string.IsNullOrEmpty(symmCipherKey) && string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("secretKey");

            symmCipherKey = string.IsNullOrEmpty(secretKey) ? symmCipherKey : secretKey;

            if (string.IsNullOrEmpty(hashIv) && string.IsNullOrEmpty(symmCipherHash))
                hashIv = (!string.IsNullOrEmpty(hashIv)) ? hashIv : (kHash != null) ? kHash.Hash(secretKey) : EnDeCodeHelper.KeyToHex(symmCipherKey);

            symmCipherHash = string.IsNullOrEmpty(hashIv) ? symmCipherHash : hashIv;

            long outByteLen = (OutPipe == null || OutPipe.Length == 0) ? cipherBytes.Length : ((cipherBytes.Length * 3) + 1);
            byte[] decryptedBytes = new byte[outByteLen];
//#if DEBUG
//            stageDictionary = new Dictionary<SymmCipherEnum, byte[]>();
//            // stageDictionary.Add(SymmCipherEnum.ZenMatrix, cipherBytes);
//#endif 
            if (OutPipe == null || OutPipe.Length == 0)
                Array.Copy(cipherBytes, 0, decryptedBytes, 0, cipherBytes.Length);
            else
                foreach (SymmCipherEnum symmCipher in OutPipe)
                {
                    decryptedBytes = DecryptBytesFast(cipherBytes, symmCipher, secretKey, hashIv);
                    cipherBytes = decryptedBytes;
//#if DEBUG
//                    stageDictionary.Add(symmCipher, cipherBytes);
//#endif
                }

            if (unzipAfter != ZipType.None)
                decryptedBytes = unzipAfter.Unzip(cipherBytes);

            return decryptedBytes;
        }


        public override byte[] EncrpytGoRounds(byte[] inBytes, string secretKey, ZipType zipBefore =ZipType.None, KeyHash keyHash = KeyHash.Hex)
            => MerryGoRoundEncrpyt(inBytes, secretKey, keyHash.Hash(secretKey), zipBefore);

        public override byte[] DecrpytRoundsGo(byte[] cipherBytes, string secretKey, ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
            => DecrpytRoundGoMerry(cipherBytes, secretKey, keyHash.Hash(secretKey), unzipAfter);

        public byte[] Encrpyt(byte[] plainBytes, string cryptKey, EncodingType encoding = EncodingType.Base64,
            ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            // construct symmetric cipher pipeline with cryptKey and pass pipeString as out param                          

            // perform multi crypt pipe stages
            byte[] encryptedBytes = this.EncrpytGoRounds(plainBytes, cryptKey, zipBefore, keyHash);
            // Encode pipes by encodingType, e.g. base64, uu, hex16, ...
            string encoded = encoding.GetEnCoder().Encode(encryptedBytes);
            byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(encoded);

            return encodedBytes;
        }

        public byte[] Decrpyt(byte[] encodedBytes, string cryptKey, EncodingType decoding = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            string decodedString = System.Text.Encoding.UTF8.GetString(encodedBytes);

            byte[] cipherBytes = decoding.GetEnCoder().Decode(decodedString);

            // staged decryption of bytes
            byte[] unroundedMerryBytes = DecrpytRoundsGo(cipherBytes, cryptKey, unzipAfter, keyHash);
            
            return unroundedMerryBytes;
            // return unroundedMerryBytes.TrimEnd((byte)0).ToArray();
        }


        public override string EncrpytEncode(byte[] inBytes, string secretKey, 
            EncodingType encType = EncodingType.Base64,
            ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(symmCipherKey))
                throw new ArgumentNullException("seretkey");

            symmCipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : symmCipherKey;
            symmCipherHash = keyHash.Hash(secretKey);
            encodeType = encType;
            zType = zipBefore;
            kHash = keyHash;
            byte[] outBytes = MerryGoRoundEncrpyt(inBytes, secretKey, symmCipherHash, zipBefore);
            string cryptedEncoded = encType.EnCode(outBytes);
            return cryptedEncoded;
        }

        public override byte[] DecodeDecrpyt(string encoded, string secretKey, 
            EncodingType encType = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(symmCipherKey))
                throw new ArgumentNullException("seretkey");

            symmCipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : symmCipherKey;
            symmCipherHash = keyHash.Hash(secretKey);
            encodeType = encType;
            zType = unzipAfter;
            kHash = keyHash;
            byte[] cipherBytes = encodeType.DeCode(encoded);
            byte[] outBytes = DecrpytRoundGoMerry(cipherBytes, secretKey, keyHash.Hash(secretKey), unzipAfter);

            return outBytes;
        }


        public override byte[] EncryptEncodeBytes(byte[] inBytes, string secretKey, string hashIV, 
            EncodingType encType = EncodingType.Base64,
            ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(symmCipherKey))
                throw new ArgumentNullException("seretkey");

            symmCipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : symmCipherKey;
            hashIV = (string.IsNullOrEmpty(hashIV)) ? keyHash.Hash(symmCipherKey) : hashIV;
            symmCipherHash = hashIV;
            encodeType = encType;
            zType = zipBefore;
            kHash = keyHash;

            byte[] outBytes = MerryGoRoundEncrpyt(inBytes, symmCipherKey, symmCipherHash, zipBefore);
            byte[] encryptedBytes = (new List<byte>()).ToArray();
            if (encType != EncodingType.None)
            {
                string cryptedEncoded = encType.EnCode(outBytes);
                encryptedBytes = System.Text.Encoding.UTF8.GetBytes(cryptedEncoded);
            }
            else
                encryptedBytes = outBytes;


            return encryptedBytes;
        }

        public override byte[] DecodeDecrpytBytes(byte[] encodedBytes, string secretKey, string hashIV, 
            EncodingType encType = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(symmCipherKey))
                throw new ArgumentNullException("seretkey");

            symmCipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : symmCipherKey;
            hashIV = (string.IsNullOrEmpty(hashIV)) ? keyHash.Hash(symmCipherKey) : hashIV;
            symmCipherHash = hashIV;
            encodeType = encType;
            zType = unzipAfter;
            kHash = keyHash;

            byte[] cipherBytes = (new List<byte>()).ToArray();
            if (encType != EncodingType.None)
            {
                string encoded = System.Text.Encoding.UTF8.GetString(encodedBytes);
                cipherBytes = encodeType.DeCode(encoded);
            }
            else
                cipherBytes = encodedBytes;

            byte[] outBytes = DecrpytRoundGoMerry(cipherBytes, secretKey, hashIV, unzipAfter);

            return outBytes;
        }

        #region static en-de-crypt members

        /// <summary>
        /// EncrpytToStringd
        /// </summary>
        /// <param name="inString">string to encrypt multiple times</param>
        /// <param name="cryptKey">Unique deterministic key for either generating the mix of symmetric cipher algorithms in the crypt pipeline 
        /// and unique crypt key for each symmetric cipher algorithm in each stage of the pipe</param>
        /// <param name="pipeString">out parameter for setting hash to compare entities encryption</param>
        /// <param name="encoding"><see cref="EncodingType"/> type for encoding encrypted bytes back in plain text></param>
        /// <param name="zipBefore">Zip bytes with <see cref="ZipType"/> before passing them in encrypted stage pipeline. <see cref="ZipTypeExtensions.Zip(ZipType, byte[])"/></param>
        /// <param name="keyHash"><see cref="KeyHash"/> hashing key algorithm</param>
        /// <returns>encrypted string</returns>        
        /// <returns></returns>
        public static new string EncrpytToString(string inString, string cryptKey, out string pipeString,
            EncodingType encoding = EncodingType.Base64, ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            // construct symmetric cipher pipeline with cryptKey and pass pipeString as out param            
            SymmCipherPipe symmPipe = new SymmCipherPipe(cryptKey);
            pipeString = symmPipe.PipeString;

            // Transform string to bytes
            byte[] inBytes = EnDeCodeHelper.GetBytesFromString(inString);
            // perform multi crypt pipe stages
            byte[] encryptedBytes = symmPipe.EncrpytGoRounds(inBytes, cryptKey, zipBefore, keyHash);
            // Encode pipes by encodingType, e.g. base64, uu, hex16, ...
            string encrypted = encoding.GetEnCoder().Encode(encryptedBytes);

            return encrypted;
        }

		public static new byte[] EncrpytStringToBytes(string inString, string cryptKey, out string pipeString,
			EncodingType encoding = EncodingType.Base64, ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
			// construct symmetric cipher pipeline with cryptKey and pass pipeString as out param            
			SymmCipherPipe symmPipe = new SymmCipherPipe(cryptKey);
			pipeString = symmPipe.PipeString;

			// Transform string to bytes
			byte[] inBytes = EnDeCodeHelper.GetBytesFromString(inString);
			// perform multi crypt pipe stages
			byte[] encryptedBytes = symmPipe.EncrpytGoRounds(inBytes, cryptKey, zipBefore, keyHash);			

			return encryptedBytes;
		}

		public static new string EncrpytBytesToString(byte[] plainBytes, string cryptKey, out string pipeString,
            EncodingType encoding = EncodingType.Base64, ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            // construct symmetric cipher pipeline with cryptKey and pass pipeString as out param            
            SymmCipherPipe symmPipe = new SymmCipherPipe(cryptKey);
            pipeString = symmPipe.PipeString;

            // perform multi crypt pipe stages
            byte[] encryptedBytes = symmPipe.EncrpytGoRounds(plainBytes, cryptKey, zipBefore, keyHash);
            // Encode pipes by encodingType, e.g. base64, uu, hex16, ...
            string encrypted = encoding.GetEnCoder().Encode(encryptedBytes);

            return encrypted;
        }


        /// <summary>
        /// DecrpytToString
        /// </summary>
        /// <param name="cryptedEncodedMsg">encrypted message</param>
        /// <param name="cryptKey">Unique deterministic key for either generating the mix of symmetric cipher algorithms in the crypt pipeline 
        /// and unique crypt key for each symmetric cipher algorithm in each stage of the pipe</param>
        /// <param name="pipeString">out parameter for setting hash to compare entities encryption</param>
        /// <param name="decoding"><see cref="EncodingType"/> type for encoding encrypted bytes back in plain text></param>
        /// <param name="unzipAfter"><see cref="ZipType"/> and <see cref="ZipTypeExtensions.Unzip(ZipType, byte[])"/></param>
        /// <param name="keyHash"><see cref="KeyHash"/> hashing key algorithm</param>
        /// <returns>Decrypted stirng</returns>
        public static new string DecrpytToString(string cryptedEncodedMsg, string cryptKey, out string pipeString,
            EncodingType decoding = EncodingType.Base64, ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            // create symmetric cipher pipe for decryption with crypt key and pass pipeString as out param
            SymmCipherPipe symmPipe = new SymmCipherPipe(cryptKey);
            pipeString = symmPipe.PipeString;

            // get bytes from encrypted encoded string dependent on the encoding type (uu, base64, base32,..)
            byte[] cipherBytes = decoding.GetEnCoder().Decode(cryptedEncodedMsg);
            // staged decryption of bytes
            byte[] unroundedMerryBytes = symmPipe.DecrpytRoundsGo(cipherBytes, cryptKey, unzipAfter, keyHash);

            // Get string from decrypted bytes
            string decrypted = EnDeCodeHelper.GetString(unroundedMerryBytes); 
            // find first \0 = NULL char in string and truncate all after first \0 apperance in string
            while (decrypted[decrypted.Length - 1] == '\0')
                decrypted = decrypted.Substring(0, decrypted.Length - 1);

            return decrypted;
        }

        public static new byte[] DecrpytStringToBytes(string cryptedEncodedMsg, string cryptKey, out string pipeString,
            EncodingType decoding = EncodingType.Base64, ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            // create symmetric cipher pipe for decryption with crypt key and pass pipeString as out param
            SymmCipherPipe symmPipe = new SymmCipherPipe(cryptKey);
            pipeString = symmPipe.PipeString;

            // get bytes from encrypted encoded string dependent on the encoding type (uu, base64, base32,..)
            byte[] cipherBytes = decoding.GetEnCoder().Decode(cryptedEncodedMsg);
            // staged decryption of bytes
            byte[] unroundedMerryBytes = symmPipe.DecrpytRoundsGo(cipherBytes, cryptKey, unzipAfter, keyHash);

            return unroundedMerryBytes;
        }

		public static new string DecrpytBytesToString(byte[] cipherBytes, string cryptKey, out string pipeString,
			EncodingType decoding = EncodingType.Base64, ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
			// create symmetric cipher pipe for decryption with crypt key and pass pipeString as out param
			SymmCipherPipe symmPipe = new SymmCipherPipe(cryptKey);
			pipeString = symmPipe.PipeString;

			// staged decryption of bytes
			byte[] unroundedMerryBytes = symmPipe.DecrpytRoundsGo(cipherBytes, cryptKey, unzipAfter, keyHash);

            // Get string from decrypted bytes
            string decrypted = EnDeCodeHelper.GetString(unroundedMerryBytes);
			// find first \0 = NULL char in string and truncate all after first \0 apperance in string
			while (decrypted[decrypted.Length - 1] == '\0')
				decrypted = decrypted.Substring(0, decrypted.Length - 1);
            
            return decrypted;
		}

		#endregion static en-de-crypt members

		#endregion multiple rounds en-de-cryption

	}

}
