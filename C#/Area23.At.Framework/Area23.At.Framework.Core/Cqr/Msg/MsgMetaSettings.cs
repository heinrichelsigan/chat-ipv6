using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Zfx;
using System.Text.Json.Serialization;

namespace Area23.At.Framework.Core.Cqr.Msg
{
    [Serializable]
    public class MsgMetaSettings
    {
        [JsonIgnore]
        private static readonly Lazy<MsgMetaSettings> _lazy;
        private static readonly Lock _lock;

        [JsonIgnore]
        public static MsgMetaSettings MsgSetInstance {  get => _lazy.Value; }

        public SerType MsgType { get; set; }

        public EncodingType EnCodingType { get; set; }

        public ZipType ZType { get; set; }

        public KeyHash KHash { get; set; }

        public byte[] MsgEnumBytes
        {
            get
            {
                short s = (short)((short)MsgType + (short)EnCodingType + (short)ZType + (short)KHash);
                byte[] b = new byte[2];
                b[1] = (byte)(s >> 4);
                short tempByte = (short)((short)b[1] << 4);
                b[0] = (byte)(s - tempByte);

                return b;
            }
            set
            {
                byte[] b = value ?? new byte[] { 0x0, 0x0 };
                short number = (short)b[1];
                number <<= 4;
                number += (short)b[0];
                KHash = KeyHash_Extensions.GetKeyHashFromValue((short)(number % 0x10));
                ZType = ZipTypeExtensions.GetZipTypeFromValue((short)((number % 0x100) - (short)KHash));
                EnCodingType = EncodingTypesExtensions.GetEncodingTypeFromValue((short)(((number % 0x1000) - (short)ZType) - (short)KHash));
                MsgType = SerializationTypeExtension.GetSerializationTypeFromValue((short)(number - ((short)EnCodingType + (short)ZType) + (short)KHash));
            }
        }

        static MsgMetaSettings()
        {
            _lock = new Lock();
            int lazyLoop = 0;
            lock (_lock)
            {
                _lazy = new Lazy<MsgMetaSettings>(() => new MsgMetaSettings());
            }
            while ((lazyLoop++ < 4) && (_lazy == null || !_lazy.IsValueCreated))
            { 
                Thread.Sleep(125);
            }
            if (_lazy != null && _lazy.IsValueCreated)
                Area23Log.LogOriginMsg("MsgMetaSettings", $"Singelton _lazy = new Lazy<MsgMetaSettings>(() => new MsgMetaSettings()) initialized; loop {lazyLoop}.");
            else
                Area23Log.LogOriginMsg("MsgMetaSettings", $"Singelton _lazy = new Lazy<MsgMetaSettings>(() => new MsgMetaSettings()) NOT initialized loop {lazyLoop}.");
        }

        public MsgMetaSettings()
        {
            MsgType = SerType.Json;
            EnCodingType = EncodingType.Base64;
            ZType = ZipType.None;
            KHash = KeyHash.Hex;
        }

        public MsgMetaSettings(SerType serType, EncodingType encodingType, ZipType zipType, KeyHash keyHash)
        {
            MsgType = serType;
            EnCodingType = encodingType;
            ZType = zipType;
            KHash = keyHash;
        }


        public void SetMsgMetaSettings(SerType serType, EncodingType encodingType, ZipType zipType, KeyHash keyHash)
        {
            MsgType = serType;
            EnCodingType = encodingType;
            ZType = zipType;
            KHash = keyHash;
        }



    }
}
