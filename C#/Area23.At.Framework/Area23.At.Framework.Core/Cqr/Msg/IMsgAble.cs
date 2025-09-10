namespace Area23.At.Framework.Core.Cqr.Msg
{
    public interface IMsgAble
    {
        // SerType MsgType { get; }        

        MsgMetaSettings MetaSettings { get => MsgMetaSettings.MsgSetInstance; }

        string Message { get; }

        string Hash { get; }
        string Md5Hash { get; }

        string ToJson();
        T FromJson<T>(string jsonText);
        string ToXml();
        T FromXml<T>(string xmlText);
      
    }
}
