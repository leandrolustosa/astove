using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace AInBox.Astove.Core.Options
{
    public class KeyValue : IKeyValue
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public int ParentId { get; set; }
        public int Key { get; set; }
        public string Value { get; set; }
    }
}
