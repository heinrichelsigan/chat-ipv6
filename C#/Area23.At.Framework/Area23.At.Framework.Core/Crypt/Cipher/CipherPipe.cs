using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Zfx;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using System.IO.Pipelines;
using System.Text;

namespace Area23.At.Framework.Core.Crypt.Cipher
{

    /// <summary>
    /// Provides a simple crypt pipe for <see cref="CipherEnum"/>
    /// </summary>
    public class CipherPipe
    {

        #region fields and properties

        private string cipherKey = "", cipherHash = "";
        private ZipType zType = ZipType.None;
        // private readonly CipherEnum[] inPipe;
        private CipherEnum[] inPipe;
        // private readonly CipherEnum[] outPipe;
        private EncodingType encodeType = EncodingType.Base64;
        private KeyHash kHash = KeyHash.Hex;
        // private readonly string pipeString;

        /// <summary>
        /// ZType is current <see cref="ZipType"/>
        /// </summary>
        public ZipType ZType { get => zType; internal set => zType = value; }

        /// <summary>
        /// Current <see cref="EncodeType"/> 
        /// </summary>
        public EncodingType EncodeType { get => encodeType; internal set => encodeType = value; }

        /// <summary>
        /// KHash is <see cref="KeyHash"/>
        /// </summary>
        public KeyHash KHash { get => kHash; internal set => kHash = value; }

        /// <summary>
        /// InPipe is current encryption pipe
        /// </summary>
        public CipherEnum[] InPipe { get => inPipe; internal set => inPipe = value; }

        /// <summary>
        /// OutPipe will always be generated from <see cref="InPipe"/>
        /// </summary>
        public CipherEnum[] OutPipe { get => inPipe.ToList().Reverse<CipherEnum>().ToArray(); }

        /// <summary>
        /// PipeString will always be generated on the fly from <see cref="InPipe"/>
        /// </summary>
        public string PipeString
        {
            get
            {
                string pipeString = "";
                foreach (CipherEnum cipher in inPipe)
                    pipeString += cipher.GetCipherChar();
                return pipeString;
            }
        }





        //#if DEBUG
        //        public Dictionary<CipherEnum, byte[]> stageDictionary = new Dictionary<CipherEnum, byte[]>();

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

        #endregion fields and properties

        #region ctor CipherPipe

        public CipherPipe()
        {
            cipherKey = ""; //
            cipherHash = "";
            inPipe = (new List<CipherEnum>()).ToArray();
            encodeType = EncodingType.Base64;
            zType = ZipType.None;
            kHash = KeyHash.Hex;
        }

        /// <summary>
        /// CipherPipe constructor with an array of <see cref="T:CipherEnum[]"/> as inpipe
        /// </summary>
        /// <param name="cipherEnums">array of <see cref="T:CipherEnum[]"/> as inpipe</param>
        public CipherPipe(CipherEnum[] cipherEnums, uint maxpipe = 8, 
            EncodingType encType = EncodingType.Base64, ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex)
        {
            // What ever is entered here as parameter, maxpipe has to be not greater 8, because of no such agency
            maxpipe = (maxpipe > Constants.MAX_PIPE_LEN) ? Constants.MAX_PIPE_LEN : maxpipe; // if somebody wants more, he/she/it gets less

            int isize = Math.Min(((int)cipherEnums.Length), ((int)maxpipe));
            inPipe = new CipherEnum[isize];
            Array.Copy(cipherEnums, inPipe, isize);

            encodeType = encType;
            zType = zpType;
            kHash = kh;            
        }

        /// <summary>
        /// CipherPipe constructor with an array of <see cref="T:string[]"/> cipherAlgos as inpipe
        /// </summary>
        /// <param name="cipherAlgos">array of <see cref="T:string[]"/> as inpipe</param>
        /// <param name="maxpipe">maximum lentgh <see cref="Constants.MAX_PIPE_LEN"/></param>
        /// <param name="encType"><see cref="EncodeType"/></param>
        /// <param name="zpType"><see cref="Zip.ZipType"/></param>
        /// <param name="kh"><see cref="KeyHash"/></param>
        public CipherPipe(string[] cipherAlgos, uint maxpipe = 8, EncodingType encType = EncodingType.Base64, ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex)
        {
            // What ever is entered here as parameter, maxpipe has to be not greater 8, because of no such agency
            maxpipe = (maxpipe > Constants.MAX_PIPE_LEN) ? Constants.MAX_PIPE_LEN : maxpipe; // if somebody wants more, he/she/it gets less

            List<CipherEnum> cipherEnums = new List<CipherEnum>();
            int cnt = 0;
            foreach (string algo in cipherAlgos)
            {
                if (!string.IsNullOrEmpty(algo))
                {
                    CipherEnum cipherAlgo = CipherEnum.Aes;
                    if (!Enum.TryParse<CipherEnum>(algo, out cipherAlgo))
                        cipherAlgo = CipherEnum.Aes;

                    cipherEnums.Add(cipherAlgo);

                    if (++cnt > maxpipe)
                        break;
                }
            }

            inPipe = cipherEnums.ToArray();

            encodeType = encType;
            kHash = kh;
            zType = zpType;
        }

        /// <summary>
        /// CipherPipe ctor with array of user key bytes
        /// </summary>
        /// <param name="keyBytes">user key bytes</param>
        /// <param name="maxpipe">maximum lentgh <see cref="Constants.MAX_PIPE_LEN"/></param>
        /// <param name="encType"><see cref="EncodeType"/></param>
        /// <param name="zpType"><see cref="Zip.ZipType"/></param>
        /// <param name="kh"><see cref="KeyHash"/></param>
        /// <exception cref="ArgumentException"></exception>
        public CipherPipe(byte[] keyBytes, uint maxpipe = 8, 
            EncodingType encType = EncodingType.Base64, ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex)
        {
            // What ever is entered here as parameter, maxpipe has to be not greater 8, because of no such agency
            maxpipe = (maxpipe > Constants.MAX_PIPE_LEN) ? Constants.MAX_PIPE_LEN : maxpipe; // if somebody wants more, he/she/it gets less

            ushort scnt = 0;
            List<CipherEnum> pipeList = new List<CipherEnum>();     

            HashSet<byte> hashBytes = new HashSet<byte>();
            for (int i = 0; i < keyBytes.Length && pipeList.Count < maxpipe; i++)
            {
                byte cb = (byte)((int)((int)keyBytes[i] % 0x20));
                if (!hashBytes.Contains(cb))
                {
                    hashBytes.Add(cb);
                    pipeList.Add(CipherEnumExtensions.ByteCipherDict[cb]);
                }
            }
           
            inPipe = pipeList.ToArray();

            zType = zpType;            
            encodeType = encType;
            kHash = kh;

            //if (inPipe.Length > maxpipe)
            //{
            //    List<string> pipElems = new List<string>(inPipe.Length);
            //    foreach (var cipherEnum in inPipe)
            //        pipElems.Add(cipherEnum.ToString());
            //    throw new ArgumentException($"Pipe \"{string.Join(";", pipElems.ToArray())}\" length exceeds {maxpipe}!");
            //}

            // foreach (CipherEnum cipherE in inPipe)
            // pipeString += cipherE.GetCipherChar();

        }

        /// <summary>
        /// Constructs a <see cref="CipherPipe"/> from key and hash
        /// by getting <see cref="T:byte[]">byte[] keybytes</see> with <see cref="CryptHelper.GetUserKeyBytes(string, string, int)"/>
        /// </summary>
        /// <param name="key">secret key to generate pipe</param>
        /// <param name="hash">hash value of secret key</param>
        /// <param name="encType"></param>
        /// <param name="zpType"></param>
        /// <param name="kh"></param>
        public CipherPipe(string key, string hash, EncodingType encType = EncodingType.Base64, ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex)
            : this(CryptHelper.GetKeyBytesSimple(key, hash, 16), Constants.MAX_PIPE_LEN, encType, zpType, kh)
        {
            cipherKey = key;
            cipherHash = hash;
        }

        /// <summary>
        /// CipherPipe ctor with only key
        /// </summary>
        /// <param name="key"></param>
        public CipherPipe(string key)
            : this(key, EnDeCodeHelper.KeyToHex(key))
        {
            cipherKey = key;
        }

        #endregion ctor CipherPipe

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
        public CipherPipe FromJson(string json)
        {
            CipherPipe pipe = JsonConvert.DeserializeObject<CipherPipe>(json);
            if (pipe == null)
            {
                this.inPipe = pipe.InPipe;
                this.encodeType = pipe.EncodeType;
                this.kHash = pipe.KHash;
                this.zType = pipe.ZType;
                this.cipherKey = pipe.cipherKey;
                this.cipherHash = pipe.cipherHash;
            }
            return pipe;
        }

        #endregion json

        #region static members EncryptBytesFast DecryptBytesFast

        /// <summary>
        /// Generic encrypt bytes to bytes
        /// </summary>
        /// <param name="inBytes">Array of byte</param>
        /// <param name="cipherAlgo"><see cref="CipherEnum"/> both symmetric and asymetric cipher algorithms</param>
        /// <param name="secretKey">secret key to decrypt</param>
        /// <param name="hash">key's hash</param>
        /// <returns>encrypted byte Array</returns>
        public static byte[] EncryptBytesFast(byte[] inBytes, CipherEnum cipherAlgo, 
            string secretKey, string hash)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("seretkey");
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentNullException("hash");

            byte[] encryptBytes = inBytes;

            switch (cipherAlgo)
            {
                case CipherEnum.AesNet:
                    AesNet aesNet = new AesNet(secretKey, hash);
                    encryptBytes = aesNet.Encrypt(inBytes);
                    break;
                case CipherEnum.Des3Net:
                    Des3Net des3 = new Des3Net(secretKey, hash);
                    encryptBytes = des3.Encrypt(inBytes);
                    break;
                //case CipherEnum.RC564:
                //    RC564.RC564GenWithKey(secretKey, hash, true);
                //    encryptBytes = RC564.Encrypt(inBytes);
                //    break;
                case CipherEnum.Rsa:
                    AsymmetricCipherKeyPair keyPair = Asymmetric.Rsa.RsaGenWithKey(Constants.RSA_PUB, Constants.RSA_PRV);
                    encryptBytes = Asymmetric.Rsa.Encrypt(inBytes, keyPair);
                    break;
                case CipherEnum.ZenMatrix:
                    encryptBytes = (new ZenMatrix(secretKey, hash, false)).Encrypt(inBytes);
                    break;
                case CipherEnum.ZenMatrix2:
                    encryptBytes = (new ZenMatrix2(secretKey, hash, false)).Encrypt(inBytes);
                    break;
                case CipherEnum.Aes:
                case CipherEnum.AesLight:
                case CipherEnum.Aria:
                case CipherEnum.BlowFish:
                case CipherEnum.Camellia:
                case CipherEnum.Cast5:
                case CipherEnum.Cast6:
                case CipherEnum.Des:
                case CipherEnum.Des3:
                case CipherEnum.Dstu7624:
                case CipherEnum.Fish2:
                case CipherEnum.Fish3:
                case CipherEnum.ThreeFish256:
                case CipherEnum.Gost28147:
                case CipherEnum.Idea:
                case CipherEnum.Noekeon:
                case CipherEnum.RC2:
                case CipherEnum.RC532:
                case CipherEnum.RC564:
                case CipherEnum.RC6:
                case CipherEnum.Rijndael:
                case CipherEnum.Seed:
                case CipherEnum.Serpent:
                case CipherEnum.SM4:
                case CipherEnum.SkipJack:
                case CipherEnum.Tea:
                case CipherEnum.Tnepres:
                case CipherEnum.XTea:
                // case CipherEnum.ZenMatrix:
                // case CipherEnum.ZenMatrix2:
                default:
                    CryptParams cpParams = new CryptParams(cipherAlgo, secretKey, hash);
                    Symmetric.CryptBounceCastle cryptBounceCastle = new Symmetric.CryptBounceCastle(cpParams, true);
                    encryptBytes = cryptBounceCastle.Encrypt(inBytes);
                    break;
            }

            return encryptBytes;
        }

        /// <summary>
        /// Generic decrypt bytes to bytes
        /// </summary>
        /// <param name="cipherBytes">Encrypted array of byte</param>
        /// <param name="cipherAlgo"><see cref="CipherEnum"/>both symmetric and asymetric cipher algorithms</param>
        /// <param name="secretKey">secret key to decrypt</param>
        /// <param name="hash">key's hash</param>
        /// <returns>decrypted byte Array</returns>
        public static byte[] DecryptBytesFast(byte[] cipherBytes, CipherEnum cipherAlgo, 
            string secretKey, string hash)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("seretkey");
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentNullException("hash");
            // bool sameKey = true;

            byte[] decryptBytes = cipherBytes;

            switch (cipherAlgo)
            {
                case CipherEnum.AesNet:
                    AesNet aesNet = new AesNet(secretKey, hash);
                    decryptBytes = aesNet.Decrypt(cipherBytes);
                    break;
                case CipherEnum.Des3Net:
                    Des3Net des3 = new Des3Net(secretKey, hash);
                    decryptBytes = des3.Decrypt(cipherBytes);
                    break;
                //case CipherEnum.RC564:
                //    RC564.RC564GenWithKey(secretKey, hash, true);
                //    decryptBytes = RC564.Decrypt(cipherBytes);
                //    break;
                case CipherEnum.Rsa:
                    AsymmetricCipherKeyPair keyPair = Asymmetric.Rsa.RsaGenWithKey(Constants.RSA_PUB, Constants.RSA_PRV);
                    decryptBytes = Asymmetric.Rsa.DecryptWithPrivate(cipherBytes, keyPair);
                    break;
                case CipherEnum.ZenMatrix:
                    decryptBytes = (new ZenMatrix(secretKey, hash, false)).Decrypt(cipherBytes);
                    break;
                case CipherEnum.ZenMatrix2:
                    decryptBytes = (new ZenMatrix2(secretKey, hash, false)).Decrypt(cipherBytes);
                    break;
                case CipherEnum.Aes:
                case CipherEnum.AesLight:
                case CipherEnum.Aria:
                case CipherEnum.BlowFish:
                case CipherEnum.Camellia:
                case CipherEnum.Cast5:
                case CipherEnum.Cast6:
                case CipherEnum.Des:
                case CipherEnum.Des3:
                case CipherEnum.Dstu7624:
                case CipherEnum.Fish2:
                case CipherEnum.Fish3:
                case CipherEnum.ThreeFish256:
                case CipherEnum.Gost28147:
                case CipherEnum.Idea:
                case CipherEnum.Noekeon:
                case CipherEnum.RC2:
                case CipherEnum.RC532:
                case CipherEnum.RC564:
                case CipherEnum.RC6:
                case CipherEnum.Rijndael:
                case CipherEnum.Seed:
                case CipherEnum.Serpent:
                case CipherEnum.SM4:
                case CipherEnum.SkipJack:
                case CipherEnum.Tea:
                case CipherEnum.Tnepres:
                case CipherEnum.XTea:
                // case CipherEnum.ZenMatrix:
                // case CipherEnum.ZenMatrix2:
                default:
                    CryptParams cpParams = new CryptParams(cipherAlgo, secretKey, hash);
                    Symmetric.CryptBounceCastle cryptBounceCastle = new Symmetric.CryptBounceCastle(cpParams, true);
                    decryptBytes = cryptBounceCastle.Decrypt(cipherBytes);

                    break;
            }


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
        public virtual byte[] MerryGoRoundEncrpyt(byte[] inBytes, string secretKey, string hashIv, ZipType zipBefore = ZipType.None)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            string hash = (!string.IsNullOrEmpty(hashIv)) ? hashIv : (kHash != null) ? kHash.Hash(secretKey) : EnDeCodeHelper.KeyToHex(secretKey);
            cipherKey = string.IsNullOrEmpty(secretKey) ? cipherKey : secretKey;
            cipherHash = hash;

            byte[] encryptedBytes = new byte[inBytes.Length];
            Array.Copy(inBytes, 0, encryptedBytes, 0, inBytes.Length);
            //#if DEBUG
            //            stageDictionary = new Dictionary<CipherEnum, byte[]>();
            //            // stageDictionary.Add(CipherEnum.ZenMatrix, inBytes);
            //#endif
            if (zipBefore != ZipType.None)
            {
                encryptedBytes = zipBefore.Zip(inBytes);
                inBytes = encryptedBytes;
            }

            foreach (CipherEnum cipher in InPipe)
            {
                encryptedBytes = EncryptBytesFast(inBytes, cipher, cipherKey, cipherHash);
                inBytes = encryptedBytes;
                //#if DEBUG
                //                stageDictionary.Add(cipher, encryptedBytes);
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
        public virtual byte[] DecrpytRoundGoMerry(byte[] cipherBytes, string secretKey, string hashIv, ZipType unzipAfter = ZipType.None)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            string hash = (!string.IsNullOrEmpty(hashIv)) ? hashIv : (kHash != null) ? kHash.Hash(secretKey) : EnDeCodeHelper.KeyToHex(secretKey);
            cipherKey = string.IsNullOrEmpty(secretKey) ? cipherKey : secretKey;
            cipherHash = hash;

            byte[] decryptedBytes = new byte[cipherBytes.Length];
            //#if DEBUG
            //            stageDictionary = new Dictionary<CipherEnum, byte[]>();
            //            // stageDictionary.Add(CipherEnum.ZenMatrix, cipherBytes);
            //#endif 
            if (OutPipe == null || OutPipe.Length == 0)
                Array.Copy(cipherBytes, 0, decryptedBytes, 0, cipherBytes.Length);
            else
                foreach (CipherEnum cipher in OutPipe)
                {
                    decryptedBytes = DecryptBytesFast(cipherBytes, cipher, cipherKey, cipherHash);
                    cipherBytes = decryptedBytes;
                    //#if DEBUG
                    //                    stageDictionary.Add(cipher, cipherBytes);
                    //#endif
                }

            if (unzipAfter != ZipType.None)
                decryptedBytes = unzipAfter.Unzip(cipherBytes);

            return decryptedBytes;
        }

        /// <summary>
        /// EncrpytTextGoRounds encrypts text with cipher pipe pipeline
        /// </summary>
        /// <param name="inString">plain text to encrypt</param>
        /// <param name="cryptKey">prviate key for encryption</param>
        /// <param name="hashIv">private hash for encryption</param>
        /// <param name="encoding"><see cref="EncodingType"/></param>
        /// <param name="zipBefore"><see cref="ZipType"/></param>
        /// <param name="keyHash"><see cref="KeyHash"/></param>
        /// <returns>UTF9 emcoded encrypted string without binary data</returns>
        public virtual string EncrpytTextGoRounds(
            string inString,
            string cryptKey,
            string hashIv,
            EncodingType encoding = EncodingType.Base64,
            ZipType zipBefore = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            // Transform string to bytes
            byte[] inBytes = EnDeCodeHelper.GetBytesFromString(inString);

            // use EncrpytFileBytesGoRounds for operations zip before and pipe cycöe encryption
            byte[] encryptedBytes = EncrpytFileBytesGoRounds(inBytes, cryptKey, hashIv, encoding, zipBefore, keyHash);

            // Encode pipes by encodingType, e.g. base64, uu, hex16, ...
            string encrypted = encoding.GetEnCoder().Encode(encryptedBytes);

            return encrypted;
        }

        /// <summary>
        /// Encrypt nomary data byte[]
        /// </summary>
        /// <param name="inBytes">binary data</param>
        /// <param name="cryptKey">prviate key for encryption</param>
        /// <param name="encoding"><see cref="EncodingType">encoding type</see> for decodinng</param>
        /// <param name="hashIv">private key hash for encryption</param>
        /// <param name="zipBefore"><see cref="ZipType"/></param>
        /// <param name="keyHash"><see cref="KeyHash"/></param>
        /// <returns>binary data</returns>
        public virtual byte[] EncrpytFileBytesGoRounds(
            byte[] inBytes,
            string cryptKey,
            string hashIv,
            EncodingType encoding = EncodingType.Base64,
            ZipType zipBefore = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            // hashIv if empty hash secretKey with keyHash hashing variant
            hashIv = (string.IsNullOrEmpty(hashIv)) ? keyHash.Hash(cryptKey) : hashIv;
            cipherKey = cryptKey;
            cipherHash = hashIv;
            KHash = keyHash;
            ZType = zipBefore;
            EncodeType = encoding;

            // perform multi crypt pipe stages
            byte[] encryptedBytes = MerryGoRoundEncrpyt(inBytes, cryptKey, hashIv, zipBefore);

            return encryptedBytes;
        }

        /// <summary>
        /// decrypt encoded encrypted text
        /// </summary>
        /// <param name="cryptedEncodedMsg">encoded encrypted ASCII string</param>
        /// <param name="cryptKey">prviate key for encryption</param>
        /// <param name="hashIv">private hash for encryption</param>
        /// <param name="decoding"><see cref="EncodingType"/></param>
        /// <param name="unzipAfter"><see cref="ZipType"/></param>
        /// <param name="keyHash"><see cref="KeyHash"/></param>
        /// <returns>decrypted UTF8 string, containing no binary data</returns>
        public virtual string DecryptTextRoundsGo(
            string cryptedEncodedMsg,
            string cryptKey,
            string hashIv,
            EncodingType decoding = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            byte[] cipherBytes = decoding.GetEnCoder().Decode(cryptedEncodedMsg);

            // perform multi crypt pipe stages
            byte[] decryptedBytes = DecryptFileBytesRoundsGo(cipherBytes, cryptKey, hashIv, decoding, unzipAfter, keyHash);

            // Get string from decrypted bytes
            string decrypted = EnDeCodeHelper.GetString(decryptedBytes);
            // find first \0 = NULL char in string and truncate all after first \0 apperance in string
            while (decrypted[decrypted.Length - 1] == '\0')
                decrypted = decrypted.Substring(0, decrypted.Length - 1);

            return decrypted;
        }

        /// <summary>
        /// DecryptFileBytesRoundsGo
        /// </summary>
        /// <param name="cipherBytes"></param>
        /// <param name="cryptKey">prviate key for encryption</param>
        /// <param name="hashIv">private hash for encryption</param>
        /// <param name="decoding"><see cref="EncodingType">decoding type</see> for decodinng</param>
        /// <param name="unzipAfter"><see cref="ZipType"/></param>
        /// <param name="keyHash"><see cref="KeyHash"/></param>
        /// <returns>plain data byte[]</returns>
        public virtual byte[] DecryptFileBytesRoundsGo(
            byte[] cipherBytes,
            string cryptKey,
            string hashIv,
            EncodingType decoding = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            // hashIv if empty hash secretKey with keyHash hashing variant            
            hashIv = string.IsNullOrEmpty(hashIv) ? keyHash.Hash(cryptKey) : hashIv;
            cipherKey = cryptKey;
            cipherHash = hashIv;
            KHash = keyHash;
            ZType = unzipAfter;
            EncodeType = decoding;

            // perform multi crypt pipe stages
            byte[] decryptedBytes = DecrpytRoundGoMerry(cipherBytes, cryptKey, hashIv, unzipAfter);

            return decryptedBytes;
        }


        public virtual byte[] EncrpytGoRounds(byte[] inBytes, string secretKey, ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            cipherHash = keyHash.Hash(secretKey);
            ZType = zipBefore;
            KHash = keyHash;
            return MerryGoRoundEncrpyt(inBytes, secretKey, cipherHash, zipBefore);
        }


        public virtual byte[] DecrpytRoundsGo(byte[] cipherBytes, string secretKey, ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            cipherHash = keyHash.Hash(secretKey);
            ZType = unzipAfter;
            KHash = keyHash;
            return DecrpytRoundGoMerry(cipherBytes, secretKey, keyHash.Hash(secretKey), unzipAfter);
        }


        public virtual string EncrpytEncode(byte[] inBytes, string secretKey, EncodingType encType = EncodingType.Base64,
            ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            cipherHash = keyHash.Hash(secretKey);
            encodeType = encType;
            ZType = zipBefore;
            KHash = keyHash;
            byte[] outBytes = MerryGoRoundEncrpyt(inBytes, secretKey, cipherHash, zipBefore);
            string cryptedEncoded = encType.EnCode(outBytes);
            return cryptedEncoded;
        }

        public virtual byte[]  EncryptEncodeBytes(byte[] inBytes, string secretKey, string hashIV, EncodingType encType = EncodingType.Base64,
           ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            hashIV = (string.IsNullOrEmpty(hashIV)) ? keyHash.Hash(cipherKey) : hashIV;
            cipherHash = hashIV;
            encodeType = encType;
            ZType = zipBefore;
            KHash = keyHash;

            byte[] outBytes = MerryGoRoundEncrpyt(inBytes, secretKey, cipherHash, zipBefore);
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



        public virtual byte[] DecodeDecrpyt(string encoded, string secretKey, EncodingType encType = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            cipherHash = keyHash.Hash(secretKey);
            encodeType = encType;
            ZType = unzipAfter;
            KHash = keyHash;
            byte[] cipherBytes = encodeType.DeCode(encoded);
            byte[] outBytes = DecrpytRoundGoMerry(cipherBytes, secretKey, keyHash.Hash(secretKey), unzipAfter);

            return outBytes;
        }


        public virtual byte[] DecodeDecrpytBytes(byte[] encodedBytes, string secretKey, string hashIV, EncodingType encType = EncodingType.Base64,
           ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            hashIV = (string.IsNullOrEmpty(hashIV)) ? keyHash.Hash(cipherKey) : hashIV;
            cipherHash = hashIV;
            encodeType = encType;
            ZType = unzipAfter;
            KHash = keyHash;

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
        /// <param name="encoding"><see cref="EncodingType"/> type for encoding encrypted bytes back in plain text</param>
        /// <param name="zipBefore">Zip bytes with <see cref="ZipType"/> before passing them in encrypted stage pipeline. <see cref="ZipTypeExtensions.Zip(ZipType, byte[])"/></param>
        /// <param name="keyHash"><see cref="KeyHash"/> hashing key algorithm</param>
        /// <returns>encrypted string</returns>        
        public static string EncrpytToString(string inString, string cryptKey,
            EncodingType encoding = EncodingType.Base64,
            ZipType zipBefore = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            // construct symmetric cipher pipeline with cryptKey
            CipherPipe cyptPipe = new CipherPipe(cryptKey);

            // Transform string to bytes
            byte[] inBytes = EnDeCodeHelper.GetBytesFromString(inString);
            // perform multi crypt pipe stages
            byte[] encryptedBytes = cyptPipe.EncrpytGoRounds(inBytes, cryptKey, zipBefore, keyHash);
            // Encode pipes by encodingType, e.g. base64, uu, hex16, ...
            string encrypted = encoding.GetEnCoder().Encode(encryptedBytes);

            return encrypted;
        }

        public static string EncrpytBytesToString(byte[] plainBytes, string cryptKey,
            EncodingType encoding = EncodingType.Base64,
            ZipType zipBefore = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            // construct symmetric cipher pipeline with cryptKey 
            CipherPipe cyptPipe = new CipherPipe(cryptKey);

            // perform multi crypt pipe stages
            byte[] encryptedBytes = cyptPipe.EncrpytGoRounds(plainBytes, cryptKey, zipBefore, keyHash);
            // Encode pipes by encodingType, e.g. base64, uu, hex16, ...
            string encrypted = encoding.GetEnCoder().Encode(encryptedBytes);

            return encrypted;
        }

        public static byte[] EncrpytStringToBytes(string inString, string cryptKey,
            EncodingType encoding = EncodingType.Base64,
            ZipType zipBefore = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            // construct symmetric cipher pipeline with cryptKey and pass pipeString as out param            
            CipherPipe cryptPipe = new CipherPipe(cryptKey);

            // Transform string to bytes
            byte[] inBytes = EnDeCodeHelper.GetBytesFromString(inString);
            // perform multi crypt pipe stages
            byte[] encryptedBytes = cryptPipe.EncrpytGoRounds(inBytes, cryptKey, zipBefore, keyHash);

            return encryptedBytes;
        }


        /// <summary>
        /// DecrpytToString
        /// </summary>
        /// <param name="cryptedEncodedMsg">encrypted message</param>
        /// <param name="cryptKey">Unique deterministic key for either generating the mix of symmetric cipher algorithms in the crypt pipeline 
        /// and unique crypt key for each symmetric cipher algorithm in each stage of the pipe</param>
        /// <param name="decoding"><see cref="EncodingType"/> type for encoding encrypted bytes back in plain text></param>
        /// <param name="unzipAfter"><see cref="ZipType"/> and <see cref="ZipTypeExtensions.Unzip(ZipType, byte[])"/></param>
        /// <param name="keyHash"><see cref="KeyHash"/> hashing key algorithm</param>
        /// <returns>Decrypted stirng</returns>
        public static string DecrpytToString(string cryptedEncodedMsg, string cryptKey,
            EncodingType decoding = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            // create symmetric cipher pipe for decryption with crypt key and pass pipeString as out param
            CipherPipe cryptPipe = new CipherPipe(cryptKey);

            // get bytes from encrypted encoded string dependent on the encoding type(uu, base64, base32,..)
            byte[] cipherBytes = decoding.GetEnCoder().Decode(cryptedEncodedMsg);
            // staged decryption of bytes
            byte[] unroundedMerryBytes = cryptPipe.DecrpytRoundsGo(cipherBytes, cryptKey, unzipAfter, keyHash);

            // Get string from decrypted bytes
            string decrypted = EnDeCodeHelper.GetString(unroundedMerryBytes);
            // find first \0 = NULL char in string and truncate all after first \0 apperance in string
            while (decrypted[decrypted.Length - 1] == '\0')
                decrypted = decrypted.Substring(0, decrypted.Length - 1);

            return decrypted;
        }

        public static byte[] DecrpytStringToBytes(string cryptedEncodedMsg, string cryptKey,
            EncodingType decoding = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            // create symmetric cipher pipe for decryption with crypt key
            CipherPipe cryptPipe = new CipherPipe(cryptKey);

            // get bytes from encrypted encoded string dependent on the encoding type (uu, base64, base32,..)
            byte[] cipherBytes = decoding.GetEnCoder().Decode(cryptedEncodedMsg);
            // staged decryption of bytes
            byte[] unroundedMerryBytes = cryptPipe.DecrpytRoundsGo(cipherBytes, cryptKey, unzipAfter, keyHash);

            return unroundedMerryBytes;
        }

        public static string DecrpytBytesToString(byte[] cipherBytes, string cryptKey,
            EncodingType decoding = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None,
            KeyHash keyHash = KeyHash.Hex)
        {
            // create symmetric cipher pipe for decryption with crypt key and pass pipeString as out param
            CipherPipe cryptPipe = new CipherPipe(cryptKey);

            // staged decryption of bytes
            byte[] unroundedMerryBytes = cryptPipe.DecrpytRoundsGo(cipherBytes, cryptKey, unzipAfter, keyHash);

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
