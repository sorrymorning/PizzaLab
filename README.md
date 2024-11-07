# Лабораторная работа на тему TCP/UDP

## Описание проекта

Данный проект представляет собой реализацию клиент-серверного приложения с использованием протоколов TCP и UDP. Основные функции включают управление заказами пиццы, такие как добавление, обновление статуса и удаление заказов.

## Изменения в коде TCP

В коде TCP были добавлены следующие функции:

1. **Добавление заказа**
2. **Обновление статуса заказа**
3. **Удаление заказа**

### Добавление заказа

Для добавления нового заказа используется следующий код:

```csharp
if (key.Key == ConsoleKey.D3) 
{
    var addUser Name = ChoosePizza() ?? "empty";
    Console.Write("Напишите ваше имя: ");
    var addName = Console.ReadLine() ?? "empty";
    var addUser  = new User(Id: 0,
        Name: addUser Name,
        Active: true,
        Customer: addName,
        Status: "Готовится"
    );
    var addResult = await userApi.AddAsync(addUser );

    Console.WriteLine(addResult ? "Ok" : "Error");
}
```
### Обновление статуса заказа
```C#
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
```
### Удаление заказа
```C#
if (key.Key == ConsoleKey.D5) // Удаление пользователя
{
    Console.Write("Введите ID пользователя для удаления: ");
    var deleteIdString = Console.ReadLine();
    int.TryParse(deleteIdString, out var deleteId);

    var deleteResult = await userApi.DeleteAsync(deleteId);

    Console.WriteLine(deleteResult ? "Пользователь удален" : "Ошибка при удалении пользователя");
}
```

Чтобы данный код работал нужно добавить в ApiTcpServer.cs следующий код:
```C#
case CommandCode.DeleteUser:
    var deleteId = Convert.ToInt32(command.Arguments["Id"]?.ToString());
    var deleteResult = await userApi.DeleteAsync(deleteId);
    return new Command()
    {
        Code = (byte)CommandCode.DeleteUser,
        Arguments = new Dictionary<string, object?>()
        {
            ["Data"] = deleteResult
        }
    };
case CommandCode.UpdateUser:
    var updateId = Convert.ToInt32(command.Arguments["Id"]?.ToString());
    var updateUserData = command.Arguments["Data"]?.ToString() ?? "{}";
    var updateUser = JsonSerializer.Deserialize<User>(updateUserData);
    var updateResult = await userApi.UpdateAsync(updateId, updateUser!);
    return new Command()
    {
        Code = (byte)CommandCode.UpdateUser,
        Arguments = new Dictionary<string, object?>()
        {
            ["Data"] = updateResult
        }
    };

```


### Работа программы можно посмотреть в папке Images.

## UDP

### Добавлен следующий код
```C#
public async Task<bool> OrderPizza(string pizzaType)
{
    var command = new Command()
    {
        Code = (byte)CommandCode.OrderPizza,
        Arguments = new Dictionary<string, object?>()
        {
            ["Data"] = pizzaType
        }
    };

    await netUdpClient.SendAsync(command);
    return true;
}
```
```C#
if (key.Key == ConsoleKey.D2) 
{
    Console.Write("Enter pizza type: ");
    var pizzaType = Console.ReadLine() ?? string.Empty;
    await messageApi.OrderPizza(pizzaType);
    Console.WriteLine($"Pizza order sent: {pizzaType}");
}
```
### Работа программы можно посмотреть в папке Images.
