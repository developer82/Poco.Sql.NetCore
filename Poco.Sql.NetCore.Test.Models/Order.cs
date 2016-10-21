namespace Poco.Sql.NetCore.Test.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public User User { get; set; }
    }
}
