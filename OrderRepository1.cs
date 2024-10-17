using System.Data.SqlClient;

public class OrderRepository
{
    private string connectionString;

    public OrderRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public List<Order> GetOrders(DateTime? startDate, DateTime? endDate, string status)
    {
        List<Order> orders = new List<Order>();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Orders WHERE (@StartDate IS NULL OR OrderDate >= @StartDate) AND (@EndDate IS NULL OR OrderDate <= @EndDate) AND (@Status IS NULL OR Status = @Status)", conn);
            cmd.Parameters.AddWithValue("@StartDate", (object)startDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDate", (object)endDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", (object)status ?? DBNull.Value);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = (int)reader["OrderId"],
                        UserId = (int)reader["UserId"],
                        OrderDate = (DateTime)reader["OrderDate"],
                        Status = reader["Status"].ToString(),
                        TotalAmount = (decimal)reader["TotalAmount"]
                    });
                }
            }
        }

        return orders;
    }

    public void UpdateOrderStatus(int orderId, string newStatus, int userId)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                SqlCommand updateOrderCmd = new SqlCommand("UPDATE Orders SET Status = @Status WHERE OrderId = @OrderId", conn, transaction);
                updateOrderCmd.Parameters.AddWithValue("@Status", newStatus);
                updateOrderCmd.Parameters.AddWithValue("@OrderId", orderId);
                updateOrderCmd.ExecuteNonQuery();

                SqlCommand insertHistoryCmd = new SqlCommand("INSERT INTO OrderHistory (OrderId, Status, ChangeDate, ChangedBy) VALUES (@OrderId, @Status, @ChangeDate, @ChangedBy)", conn, transaction);
                insertHistoryCmd.Parameters.AddWithValue("@OrderId", orderId);
                insertHistoryCmd.Parameters.AddWithValue("@Status", newStatus);
                insertHistoryCmd.Parameters.AddWithValue("@ChangeDate", DateTime.Now);
                insertHistoryCmd.Parameters.AddWithValue("@ChangedBy", userId);
                insertHistoryCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public void CreateOrder(Order order)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Orders (UserId, OrderDate, Status, TotalAmount) VALUES (@UserId, @OrderDate, @Status, @TotalAmount)", conn);
            cmd.Parameters.AddWithValue("@UserId", order.UserId);
            cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            cmd.Parameters.AddWithValue("@Status", order.Status);
            cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
            cmd.ExecuteNonQuery();
        }
    }

    public void DeleteOrder(int orderId)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM Orders WHERE OrderId = @OrderId", conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.ExecuteNonQuery();
        }
    }

    public void CreateUser(User user)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Users (Username, Role, PasswordHash) VALUES (@Username, @Role, @PasswordHash)", conn);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.ExecuteNonQuery();
        }
    }

    public User GetUserByUsername(string username)
    {
        User user = null;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE Username = @Username", conn);
            cmd.Parameters.AddWithValue("@Username", username);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = new User
                    {
                        UserId = (int)reader["UserId"],
                        Username = reader["Username"].ToString(),
                        Role = reader["Role"].ToString(),
                        PasswordHash = reader["PasswordHash"].ToString()
                    };
                }
            }
        }
        return user;
    }
}

