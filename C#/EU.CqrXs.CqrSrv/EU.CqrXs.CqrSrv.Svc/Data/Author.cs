﻿using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace EU.CqrXs.CqrSrv.Svc.Data
{
    [DataContract]
    [SwaggerWcfDefinition("author")]
    public class Author
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
