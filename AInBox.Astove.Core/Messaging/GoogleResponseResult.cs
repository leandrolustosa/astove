namespace AInBox.Astove.Core.Messaging
{
    public class GoogleResponseResult
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public MessageResult[] results { get; set; }
    }

    public class MessageResult
    {
        public string message_id { get; set; }
    }
}