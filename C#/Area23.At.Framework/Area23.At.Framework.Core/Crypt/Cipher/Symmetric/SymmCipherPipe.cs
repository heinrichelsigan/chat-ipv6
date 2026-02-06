using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Zfx;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace Area23.At.Framework.Core.Crypt.Cipher.Symmetric
{

    /// <summary>
    /// Provides a simple crypt pipe for <see cref="SymmCipherEnum"/>
    /// </summary>
    public class SymmCipherPipe : CipherPipe
    {

        protected internal SymmCipherEnum[] inSymmPipe;

        public SymmCipherEnum[] InSymmPipe
        {
            get => inSymmPipe;
            set
            {
                inSymmPipe = value;
                InPipe = value.ToList().ConvertAll(new Converter<SymmCipherEnum, CipherEnum>(SymmCipherToCipher)).ToArray();
            }
        }

        public SymmCipherEnum[] OutSymmPipe { get => new List<SymmCipherEnum>(InSymmPipe).Reverse<SymmCipherEnum>().ToArray(); }

        public new string PipeString
        {
            get
            {
                string pipeString = string.Empty;
                foreach (SymmCipherEnum symmCipher in inSymmPipe)
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

        public SymmCipherPipe() : base()
        {
            inSymmPipe = (new List<SymmCipherEnum>()).ToArray();

            Area23Log.LogOriginMsg("SymmCipherPipe", $"Generating symmetric cipher pipe: {PipeString}, encoding = {encodeType}, zipping={zType}, hashing={kHash}");
        }

        /// <summary>
        /// SymmCipherPipe constructor with an array of <see cref="T:SymmCipherEnum[]"/> as inpipe
        /// </summary>
        /// <param name="symmCipherEnums">array of <see cref="T:SymmCipherEnum[]"/> as inpipe</param>
        public SymmCipherPipe(
            SymmCipherEnum[] symmCipherEnums,
            uint maxpipe = 8,
            EncodingType encType = EncodingType.Base64,
            ZipType zpType = ZipType.None,
            KeyHash kh = KeyHash.Hex,
            CipherMode2 cmode2 = CipherMode2.ECB) :
            base(
                symmCipherEnums.ToList().ConvertAll(new Converter<SymmCipherEnum, CipherEnum>(SymmCipherToCipher)).ToArray(),
                maxpipe,
                encType,
                zpType,
                kh,
                cmode2)
        {
            // What ever is entered here as parameter, maxpipe has to be not greater 8, because of no such agency
            maxpipe = (maxpipe > Constants.MAX_PIPE_LEN) ? Constants.MAX_PIPE_LEN : maxpipe; // if somebody wants more, he/she/it gets less

            int isize = Math.Min(((int)symmCipherEnums.Length), ((int)maxpipe));
            inSymmPipe = new SymmCipherEnum[isize];
            Array.Copy(symmCipherEnums, inSymmPipe, isize);

            Area23Log.LogOriginMsg("SymmCipherPipe", $"Generating symmetric cipher pipe: {PipeString}, encoding = {encType}, zipping={zpType}, hashing={kh}");
        }

        /// <summary>
        /// SymmCipherPipe constructor with an array of <see cref="T:string[]"/> as inpipe
        /// </summary>
        /// <param name="symmCipherAlgos">array of <see cref="T:string[]"/> as inpipe</param>
        public SymmCipherPipe(string[] symmCipherAlgos, uint maxpipe = 8,
            EncodingType encType = EncodingType.Base64,
            ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex, CipherMode2 cmode2 = CipherMode2.ECB)
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

            InSymmPipe = symmCipherEnums.ToArray();
            CMode2 = cmode2;
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
            EncodingType encType = EncodingType.Base64, ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex,
            CipherMode2 cmode2 = CipherMode2.ECB, bool verbose = false)
        {
            // What ever is entered here as parameter, maxpipe has to be not greater 8, because of no such agency
            maxpipe = (maxpipe > Constants.MAX_PIPE_LEN) ? Constants.MAX_PIPE_LEN : maxpipe; // if somebody wants more, he/she/it gets less

            ushort scnt = 0;
            List<SymmCipherEnum> pipeList = new List<SymmCipherEnum>();
            Dictionary<byte, SymmCipherEnum> symDict = SymmCipherEnumExtensions.GetByteSymmCipherDict();

            string hexString = string.Empty;
            HashSet<byte> hashBytes = new HashSet<byte>();
            int cc = 0, bc = 0;
            for (bc = 0; (bc < keyBytes.Length && pipeList.Count < maxpipe); bc++)
            {
                byte msb = (byte)(keyBytes[bc] / 0x10);
                byte lsb = (byte)(keyBytes[bc] % 0x10);
                SymmCipherEnum symmCipherEnum = symDict[msb];
                if (!hashBytes.Contains(msb))
                {
                    hashBytes.Add(msb);
                    pipeList.Add(symmCipherEnum);
                    if (verbose)
                        Console.Out.WriteLine("keybyts[" + cc + "]=" + keyBytes[cc++] + " byte msb = " + (int)msb + " SymmCipherEnum: " + symmCipherEnum);
                }
                if (!hashBytes.Contains(lsb))
                {
                    hashBytes.Add(lsb);
                    symmCipherEnum = symDict[lsb];
                    pipeList.Add(symmCipherEnum);
                    if (verbose)
                        Console.Out.WriteLine("keybyts[" + cc + "]=" + keyBytes[cc++] + " byte lsb = " + (int)lsb + " SymmCipherEnum: " + symmCipherEnum);
                }
            }


            InSymmPipe = pipeList.ToArray();
            CMode2 = cmode2;
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
                            ZipType zpType = ZipType.None, KeyHash kh = KeyHash.Hex,
                            CipherMode2 cmode2 = CipherMode2.ECB,
                            bool verbose = false)
            : this(CryptHelper.GetKeyBytesSimple(key, hash, 16), Constants.MAX_PIPE_LEN, encType, zpType, kh, cmode2, verbose)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            cipherKey = key;
            cipherHash = (string.IsNullOrEmpty(hash)) ? kHash.Hash(key) : hash;
        }

        /// <summary>
        /// SymmCipherPipe ctor with only key
        /// </summary>
        /// <param name="key"></param>
        public SymmCipherPipe(string key, bool verbose = false)
            : this(key, EnDeCodeHelper.KeyToHex(key), EncodingType.Base64, ZipType.None, KeyHash.Hex, CipherMode2.ECB, verbose)
        {
            cipherKey = key;
        }

        #endregion ctor SymmCipherPipe

        #region json

        /// <summary>
        /// ToJson 
        /// </summary>
        /// <returns>serialized string</returns>
        public new string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

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
                this.inSymmPipe = pipe.InSymmPipe;
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
        /// <param name="cipherAlgo"><see cref="SymmCipherEnum"/> both symmetric and asymetric cipher algorithms</param>
        /// <param name="secretKey">secret key to decrypt</param>
        /// <param name="hashIv">key's hash</param>
        /// <param name="cipherMode"></param>
        /// <returns>encrypted byte Array</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] EncryptBytesFast(byte[] inBytes, SymmCipherEnum cipherAlgo,
            string secretKey, string hashIv, CipherMode2 cmode2 = CipherMode2.ECB)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("seretkey");
            string hash = (!string.IsNullOrEmpty(hashIv)) ? hashIv : KeyHash.Hex.Hash(secretKey);

            byte[] encryptBytes = inBytes;

            SymmCryptParams cpParams = new SymmCryptParams(cipherAlgo, secretKey, hash) { CMode2 = cmode2 };
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
        /// <param name="cmode2"></param>
        /// <returns>decrypted byte Array</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] DecryptBytesFast(byte[] cipherBytes, SymmCipherEnum cipherAlgo,
            string secretKey, string hashIv, CipherMode2 cmode2 = CipherMode2.ECB)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("seretkey");
            string hash = (!string.IsNullOrEmpty(hashIv)) ? hashIv : KeyHash.Hex.Hash(secretKey);

            byte[] decryptBytes = cipherBytes;

            SymmCryptParams cpParams = new SymmCryptParams(cipherAlgo, secretKey, hash) { Mode = cmode2.ToString() };
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
        /// <param name="cmode2"></param>
        /// <returns>encrypted byte[]</returns>
        public override byte[] MerryGoRoundEncrpyt(byte[] inBytes, string secretKey, string hashIv,
            CipherMode2 cmode2)
        {
            if (InPipe == null || inPipe.Length == 0)   // when 0 round cipher merry go round => return immideate inBytes;
                return inBytes;

            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                secretKey = "";
            string hash = hashIv ?? "";
            if (string.IsNullOrEmpty(hash) && !string.IsNullOrEmpty(secretKey))
                hash = (KHash != null) ? KHash.Hash(secretKey) : EnDeCodeHelper.KeyToHex(secretKey);
            cipherKey = string.IsNullOrEmpty(secretKey) ? cipherKey : secretKey;
            cipherHash = hash;
            CMode2 = cmode2;
            //#if DEBUG
            //      stageDictionary = new Dictionary<SymmCipherEnum, byte[]>();
            //#endif
            byte[] encryptedBytes = new byte[inBytes.Length];
            foreach (SymmCipherEnum symmCipher in InPipe)
            {
                encryptedBytes = EncryptBytesFast(inBytes, symmCipher, secretKey, hashIv, cmode2);
                inBytes = encryptedBytes;
                //#if DEBUG
                //      stageDictionary.Add(symmCipher, encryptedBytes);
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
        /// <param name="cmode2"></param>
        /// <returns><see cref="T:byte[]"/> plain bytes</returns>
        public override byte[] DecrpytRoundGoMerry(byte[] cipherBytes, string secretKey, string hashIv,
            CipherMode2 cmode2)
        {
            if (OutPipe == null || OutPipe.Length == 0) // when 0 rounds carusell, return immideate inBytes
                return cipherBytes;

            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                secretKey = "";
            string hash = hashIv ?? "";
            if (string.IsNullOrEmpty(hashIv) && !string.IsNullOrEmpty(secretKey))
                hash = (KHash != null) ? KHash.Hash(secretKey) : EnDeCodeHelper.KeyToHex(secretKey);
            cipherKey = string.IsNullOrEmpty(secretKey) ? cipherKey : secretKey;
            cipherHash = string.IsNullOrEmpty(hash) ? cipherHash : hashIv;

            //#if DEBUG
            //            stageDictionary = new Dictionary<SymmCipherEnum, byte[]>();
            //            // stageDictionary.Add(SymmCipherEnum.ZenMatrix, cipherBytes);
            //#endif 
            long outByteLen = (OutPipe == null || OutPipe.Length == 0) ? cipherBytes.Length : ((cipherBytes.Length * 3) + 1);
            byte[] decryptedBytes = new byte[outByteLen];
            foreach (SymmCipherEnum symmCipher in OutPipe)
            {
                decryptedBytes = DecryptBytesFast(cipherBytes, symmCipher, secretKey, hashIv, cmode2);
                cipherBytes = decryptedBytes;
                //#if DEBUG
                //                    stageDictionary.Add(symmCipher, cipherBytes);
                //#endif
            }

            return decryptedBytes;
        }


        public override byte[] EncrpytGoRounds(byte[] inBytes, string secretKey,
            ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex,
            CipherMode2 cmode2 = CipherMode2.ECB)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            KHash = keyHash;
            ZType = zipBefore;
            cipherHash = KHash.Hash(secretKey);

            // zip if requested
            byte[] zippedBytes = (zipBefore != ZipType.None) ? zipBefore.Zip(inBytes) : inBytes;
            // encrypt in a marry go round way
            return MerryGoRoundEncrpyt(zippedBytes, secretKey, cipherHash, cmode2);
        }

        public override byte[] DecrpytRoundsGo(byte[] cipherBytes, string secretKey,
            ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex,
            CipherMode2 cmode2 = CipherMode2.ECB)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                throw new ArgumentNullException("seretkey");

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            cipherHash = keyHash.Hash(secretKey);
            ZType = unzipAfter;
            KHash = keyHash;
            CMode2 = cmode2;
            // perform multi crypt pipe stages
            byte[] intermediatBytes = DecrpytRoundGoMerry(cipherBytes, secretKey, cipherHash, CMode2);
            // Unzip after if necessary
            byte[] decryptedBytes = (unzipAfter != ZipType.None) ? unzipAfter.Unzip(intermediatBytes) : intermediatBytes;

            return decryptedBytes;
        }

        public byte[] Encrpyt(byte[] plainBytes, string cryptKey, EncodingType encoding = EncodingType.Base64,
            ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex,
            CipherMode2 cmode2 = CipherMode2.ECB)
        {
            // construct symmetric cipher pipeline with cryptKey and pass pipeString as out param                          
            CMode2 = cmode2;
            // perform multi crypt pipe stages
            byte[] encryptedBytes = this.EncrpytGoRounds(plainBytes, cryptKey, zipBefore, keyHash, CMode2);
            // Encode pipes by encodingType, e.g. base64, uu, hex16, ...
            string encoded = encoding.GetEnCoder().Encode(encryptedBytes);
            byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(encoded);

            return encodedBytes;
        }

        public byte[] Decrpyt(byte[] encodedBytes, string cryptKey, EncodingType decoding = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex,
            CipherMode2 cmode2 = CipherMode2.ECB)
        {
            string decodedString = System.Text.Encoding.UTF8.GetString(encodedBytes);

            byte[] cipherBytes = decoding.GetEnCoder().Decode(decodedString);

            // staged decryption of bytes
            byte[] unroundedMerryBytes = DecrpytRoundsGo(cipherBytes, cryptKey, unzipAfter, keyHash, cmode2);

            return unroundedMerryBytes;
            // return unroundedMerryBytes.TrimEnd((byte)0).ToArray();
        }


        public override byte[] EncryptEncodeBytes(byte[] inBytes, string secretKey, string hashIV,
            EncodingType encType = EncodingType.Base64,
            ZipType zipBefore = ZipType.None, KeyHash keyHash = KeyHash.Hex,
            CipherMode2 cmode2 = CipherMode2.ECB)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                secretKey = "";

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            if (string.IsNullOrEmpty(hashIV))
                cipherHash = (!string.IsNullOrEmpty(secretKey)) ? keyHash.Hash(secretKey) : "";
            else
                cipherHash = hashIV;
            encodeType = encType;
            ZType = zipBefore;
            KHash = keyHash;

            // zip if requested
            byte[] zippedBytes = (zipBefore != ZipType.None) ? zipBefore.Zip(inBytes) : inBytes;
            // now encrypt with pipe
            byte[] outBytes = MerryGoRoundEncrpyt(zippedBytes, secretKey, cipherHash, cmode2);
            // encode after encryption pipe
            if (encType == EncodingType.None)
                return outBytes;

            return System.Text.Encoding.UTF8.GetBytes(encType.EnCode(outBytes));
        }

        public override byte[] DecodeDecrpytBytes(byte[] encodedBytes, string secretKey, string hashIV,
            EncodingType encType = EncodingType.Base64,
            ZipType unzipAfter = ZipType.None, KeyHash keyHash = KeyHash.Hex,
            CipherMode2 cmode2 = CipherMode2.ECB)
        {
            if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(cipherKey))
                secretKey = "";

            cipherKey = (!string.IsNullOrEmpty(secretKey)) ? secretKey : cipherKey;
            if (string.IsNullOrEmpty(hashIV))
                cipherHash = (!string.IsNullOrEmpty(secretKey)) ? keyHash.Hash(secretKey) : "";
            else
                cipherHash = hashIV;
            cipherHash = hashIV;
            encodeType = encType;
            ZType = unzipAfter;
            KHash = keyHash;
            CMode2 = cmode2;

            // Decoded encoded bytes first, if necessary
            byte[] cipherBytes = (encType != EncodingType.None) ?
                encodeType.GetEnCoder().Decode(System.Text.Encoding.UTF8.GetString(encodedBytes)) :
                encodedBytes;
            // perform multi crypt pipe stages
            byte[] intermediatBytes = DecrpytRoundGoMerry(cipherBytes, secretKey, cipherHash, cmode2);
            // Unzip after all, if it's necessary
            byte[] decryptedBytes = (unzipAfter != ZipType.None) ? unzipAfter.Unzip(intermediatBytes) : intermediatBytes;

            return decryptedBytes;
        }

        /// <summary>
        /// Multi functional 
        /// <see cref="EncryptEncodeBytes(byte[], string, string, EncodingType, ZipType, KeyHash, CipherMode2)"/>
        /// <see cref="DecodeDecrpytBytes(byte[], string, string, EncodingType, ZipType, KeyHash, CipherMode2)"/>
        /// </summary>
        /// <param name="inBytes">incoming bytes</param>
        /// <param name="secretKey">user private key</param>
        /// <param name="hashIV">hashed secret key</param>
        /// <param name="directionDecrypt">true for decryption, false for encryption</param>
        /// <param name="encType">encoding ascii type, e.g. base64, uu, xx</param>
        /// <param name="zip">compression method to zip before or unzip after pipe processed</param>
        /// <param name="keyHash">hashing type of hashing method to hash key</param>
        /// <returns>transformed byte array</returns>
        public override byte[] CryptCodeBytes(byte[] inBytes, string secretKey, string hashIV,
            bool directionDecrypt = false, EncodingType encType = EncodingType.Base64,
            ZipType zip = ZipType.None, KeyHash keyHash = KeyHash.Hex,
            CipherMode2 cmode2 = CipherMode2.ECB)
        {
            return (!directionDecrypt) ?
                EncryptEncodeBytes(inBytes, secretKey, hashIV, encType, zip, keyHash, cmode2) :
                DecodeDecrpytBytes(inBytes, secretKey, hashIV, encType, zip, keyHash, cmode2);
        }

        #endregion multiple rounds en-de-cryption

    }

}
