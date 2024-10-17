public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
}

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string PasswordHash { get; set; }
}
