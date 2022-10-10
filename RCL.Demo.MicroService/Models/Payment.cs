#nullable disable


namespace RCL.Demo.Data
{
    public class Payment
    {
        public string transactionId { get; set; }
        public DateTime dateCreated { get; set; }
        public int amount { get; set; }
        public int fee { get; set; }
        public int net { get; set; }
        public string description { get; set; }
        public string userId { get; set; }
        public string payerName { get; set; }
        public string payerEmail { get; set; }
        public string status { get; set; }
    }
}
