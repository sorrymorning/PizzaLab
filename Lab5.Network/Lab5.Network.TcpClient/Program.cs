using System.Numerics;
using Lab5.Network.Common;
using Lab5.Network.Common.UserApi;

internal class Program
{
    private static object _locker = new object();

    public static async Task Main(string[] args)
    {
        var serverAdress = new Uri("tcp://127.0.0.1:5555");
        var client = new NetTcpClient(serverAdress);
        Console.WriteLine($"Connect to server at {serverAdress}");
        await client.ConnectAsync();

        var userApi = new UserApiClient(client);
        await ManageUsers(userApi);
        client.Dispose();
    }

    private static async Task ManageUsers(IUserApi userApi)
    {
        PrintMenu();

        while(true) {
            var key = Console.ReadKey(true);

            PrintMenu();

            if (key.Key == ConsoleKey.D1) 
            {
                var users = await userApi.GetAllAsync();
                Console.WriteLine($"| Id    |      Имя пиццы             | Статус |  Покупатель   |");
                foreach (var user in users)
                {
                    Console.WriteLine($"| {user.Id,5} | {user.Name,20} | {user.Status,6} |{user.Customer,15}");
                }
            }

            if (key.Key == ConsoleKey.D2) 
            {
                Console.Write("Enter user id: ");
                var userIdString = Console.ReadLine();
                int.TryParse(userIdString, out var userId);
                var user = await userApi.GetAsync(userId);
                //Console.WriteLine($"Id={user?.Id}, Name={user?.Name}, Active={user?.Active}");
                Console.WriteLine($"| {user?.Id,5} | {user?.Name,20} | {user?.Status,6} |{user?.Customer,15}");
            }

            if (key.Key == ConsoleKey.D3) 
            {
                
                var addUserName = ChoosePizza() ?? "empty";
                Console.Write("Напишите ваше имя: ");
                var addName = Console.ReadLine() ?? "empty";
                var addUser = new User(Id: 0,
                    Name: addUserName,
                    Active: true,
                    Customer: addName,
                    Status: "Готовится"

                );
                var addResult = await userApi.AddAsync(addUser);

                Console.WriteLine(addResult ? "Ok" : "Error");
                
            }
            if (key.Key == ConsoleKey.D4) // Обновление только статуса пользователя
            {
                Console.Write("Введите ID пользователя для обновления статуса: ");
                var updateIdString = Console.ReadLine();
                int.TryParse(updateIdString, out var updateId);

                // Получаем текущие данные пользователя
                var existingUser = await userApi.GetAsync(updateId);
                if (existingUser == null)
                {
                    Console.WriteLine("Пользователь с таким ID не найден.");
                    continue;
                }


                // Создаем новый объект пользователя с измененным только статусом
                var updatedUser = new User(
                    Id: existingUser.Id,
                    Name: existingUser.Name,
                    Active: existingUser.Active,
                    Customer: existingUser.Customer,
                    Status: "Готов"
                );

                var updateResult = await userApi.UpdateAsync(updateId, updatedUser);

                Console.WriteLine(updateResult ? "Статус пользователя обновлен" : "Ошибка при обновлении статуса пользователя");
            }
            if (key.Key == ConsoleKey.D5) // Удаление пользователя
            {
                Console.Write("Введите ID пользователя для удаления: ");
                var deleteIdString = Console.ReadLine();
                int.TryParse(deleteIdString, out var deleteId);

                var deleteResult = await userApi.DeleteAsync(deleteId);

                Console.WriteLine(deleteResult ? "Пользователь удален" : "Ошибка при удалении пользователя");
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
        Console.ReadKey();
        //while (Console.Read)
    }

    private static void PrintMenu()
    {
        lock (_locker)
        {
            Console.WriteLine("1 - Вывести все заказы");
            Console.WriteLine("2 - Показать заказ по id");
            Console.WriteLine("3 - Заказать пиццу");
            Console.WriteLine("4 - Установить готовый заказ");
            Console.WriteLine("5 - Удалить заказ");
            Console.WriteLine("-------");
        }
    }
    static string ChoosePizza()
    {
        lock (_locker)
        {
            // Набор из 5 пицц
            string[] pizzas = { "Маргарита", "Пепперони", "Гавайская", "Четыре сыра", "Вегетарианская" };

            // Вывод списка пицц
            Console.WriteLine("Выберите пиццу из следующего списка:");
            for (int i = 0; i < pizzas.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {pizzas[i]}");
            }

            // Запрос выбора пользователя
            Console.Write("Введите номер пиццы (1-5): ");
            int choice;

            // Проверка корректности ввода
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > pizzas.Length)
            {
                Console.Write("Некорректный ввод. Пожалуйста, введите номер пиццы (1-5): ");
            }

            // Возврат названия выбранной пиццы
            return pizzas[choice - 1];
        }
    }
    

}
