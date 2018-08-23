using AInBox.Astove.Core.Model;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace AInBox.Astove.Core.Options
{
    public class KeyValueString : BaseMongoModel, IKeyValue
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
