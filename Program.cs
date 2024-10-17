class Program
{
    static string connectionString = "Server=localhost;Database=OrderManagmantDB0.1;Trusted_Connection=True;";
    static OrderRepository orderRepository = new OrderRepository(connectionString);
    static int loggedInUserId = 0; // Імітуємо залогованого користувача
    static User loggedInUser = null;

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("1. Увiйти в систему");
            Console.WriteLine("2. Переглянути замовлення");
            Console.WriteLine("3. Фiльтрувати замовлення за датою та статусом");
            Console.WriteLine("4. Змiнити статус замовлення");
            Console.WriteLine("5. Додати нове замовлення");
            Console.WriteLine("6. Видалити замовлення");
            Console.WriteLine("7. Вийти");
            Console.Write("Виберiть опцiю: ");

            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    Login();
                    break;
                case 2:
                    ViewOrders();
                    break;
                case 3:
                    FilterOrders();
                    break;
                case 4:
                    UpdateOrderStatus(); // Переконайтеся, що цей метод існує
                    break;
                case 5:
                    CreateNewOrder(); // Переконайтеся, що цей метод існує
                    break;
                case 6:
                    DeleteOrder(); // Переконайтеся, що цей метод існує
                    break;
                case 7:
                    Logout();
                    return;
                default:
                    Console.WriteLine("Невiрна опцiя, спробуйте ще раз.");
                    break;
            }
        }
    }

    static void Login()
    {
        Console.Write("Введiть iм'я користувача: ");
        string username = Console.ReadLine();
        Console.Write("Введiть пароль: ");
        string password = Console.ReadLine();

        var user = orderRepository.GetUserByUsername(username);

        if (user != null && VerifyPassword(password, user.PasswordHash))
        {
            loggedInUser = user;
            loggedInUserId = user.UserId;
            Console.WriteLine($"Вiтаємо, {user.Username}!");
        }
        else
        {
            Console.WriteLine("Невiрний логiн або пароль.");
        }
    }

    static bool VerifyPassword(string enteredPassword, string storedHash)
    {
        // Тут можна додати перевірку пароля, якщо використовується хешування
        return enteredPassword == storedHash;
    }

    static void Logout()
    {
        Console.WriteLine("Ви вийшли з системи.");
        loggedInUserId = 0;
        loggedInUser = null;
    }

    static void ViewOrders()
    {
        if (loggedInUser == null)
        {
            Console.WriteLine("Будь ласка, увiйдiть в систему для перегляду замовлень.");
            return;
        }

        List<Order> orders = orderRepository.GetOrders(null, null, null);
        foreach (var order in orders)
        {
            Console.WriteLine($"Замовлення #{order.OrderId}, Статус: {order.Status}, Дата: {order.OrderDate}, Сума: {order.TotalAmount}");
        }
    }

    static void FilterOrders()
    {
        Console.Write("Введiть початкову дату (yyyy-MM-dd) або залиште порожньою: ");
        string startDateInput = Console.ReadLine();
        DateTime? startDate = string.IsNullOrWhiteSpace(startDateInput) ? null : DateTime.Parse(startDateInput);

        Console.Write("Введiть кiнцеву дату (yyyy-MM-dd) або залиште порожньою: ");
        string endDateInput = Console.ReadLine();
        DateTime? endDate = string.IsNullOrWhiteSpace(endDateInput) ? null : DateTime.Parse(endDateInput);

        Console.Write("Введiть статус (очiкує на обробку, прийняте, оплачене, вiдправлене) або залиште порожнiм: ");
        string status = Console.ReadLine();

        List<Order> orders = orderRepository.GetOrders(startDate, endDate, status);
        foreach (var order in orders)
        {
            Console.WriteLine($"Замовлення #{order.OrderId}, Статус: {order.Status}, Дата: {order.OrderDate}, Сума: {order.TotalAmount}");
        }
    }

    static void UpdateOrderStatus()
    {
        if (loggedInUser == null)
        {
            Console.WriteLine("Будь ласка, увiйдiть в систему для змiни статусу.");
            return;
        }

        Console.Write("Введiть ID замовлення: ");
        int orderId = int.Parse(Console.ReadLine());

        Console.Write("Введiть новий статус (очiкує на обробку, прийняте, оплачене, відправлене): ");
        string newStatus = Console.ReadLine();

        orderRepository.UpdateOrderStatus(orderId, newStatus, loggedInUserId);
        Console.WriteLine("Статус оновлено.");
    }

    static void CreateNewOrder()
    {
        if (loggedInUser == null)
        {
            Console.WriteLine("Будь ласка, увiйдiть в систему для створення нового замовлення.");
            return;
        }

        Console.Write("Введiть дату замовлення (yyyy-MM-dd): ");
        DateTime orderDate = DateTime.Parse(Console.ReadLine());

        Console.Write("Введiть суму замовлення: ");
        decimal totalAmount = decimal.Parse(Console.ReadLine());

        Console.Write("Введiть статус замовлення (очiкує на обробку, прийняте, оплачене, вiдправлене): ");
        string status = Console.ReadLine();

        Order newOrder = new Order
        {
            UserId = loggedInUserId,
            OrderDate = orderDate,
            Status = status,
            TotalAmount = totalAmount
        };

        orderRepository.CreateOrder(newOrder);
        Console.WriteLine("Замовлення створено.");
    }

    static void DeleteOrder()
    {
        if (loggedInUser == null)
        {
            Console.WriteLine("Будь ласка, увiйдiть в систему для видалення замовлення.");
            return;
        }

        Console.Write("Введiть ID замовлення для видалення: ");
        int orderId = int.Parse(Console.ReadLine());

        orderRepository.DeleteOrder(orderId);
        Console.WriteLine("Замовлення видалено.");
    }
}
