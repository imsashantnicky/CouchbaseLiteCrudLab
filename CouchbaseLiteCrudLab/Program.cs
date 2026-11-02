using Couchbase.Lite;

const string DatabaseName = "employeecrudlab";

using var database = new Database(DatabaseName);
var collection = database.GetDefaultCollection();

Console.WriteLine("Couchbase Lite - Employee CRUD Lab");
Console.WriteLine($"Database opened: {DatabaseName}");
Console.WriteLine($"Collection: {collection.Name}");

bool exit = false;

while (!exit)
{
    Console.WriteLine("\\n1. Create employee");
    Console.WriteLine("2. Read employee");
    Console.WriteLine("3. Update employee");
    Console.WriteLine("4. Delete employee");
    Console.WriteLine("5. Exit");
    Console.Write("Select option: ");

    switch (Console.ReadLine())
    {
        case "1":
            CreateEmployee(collection);
            //Console.WriteLine("Create feature will be added in Iteration 2.");
            break;
        case "2":
            ReadEmployee(collection);
            //Console.WriteLine("Read feature will be added in Iteration 3.");
            break;
        case "3":
            UpdateEmployee(collection);
            //Console.WriteLine("Update feature will be added in Iteration 4.");
            break;
        case "4":
            DeleteEmployee(collection);
            //Console.WriteLine("Delete feature will be added in Iteration 5.");
            break;
        case "5":
            exit = true;
            break;
        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}

Console.WriteLine("Application closed.");




static void CreateEmployee(Collection collection)
{
    Console.Write("Employee ID: ");
    string id = ReadRequired();

    var existing = collection.GetDocument(id);
    if (existing is not null)
    {
        Console.WriteLine("Employee already exists. Use a different ID.");
        return;
    }

    Console.Write("First name: ");
    string firstName = ReadRequired();

    Console.Write("Last name: ");
    string lastName = ReadRequired();

    Console.Write("Email: ");
    string email = ReadRequired();

    Console.Write("Department: ");
    string department = ReadRequired();

    Console.Write("Salary: ");
    if (!double.TryParse(Console.ReadLine(), out double salary) || salary < 0)
    {
        Console.WriteLine("Invalid salary. Enter a number greater than or equal to zero.");
        return;
    }

    var document = new MutableDocument(id);
    document.SetString("type", "employee");
    document.SetString("firstName", firstName);
    document.SetString("lastName", lastName);
    document.SetString("email", email);
    document.SetString("department", department);
    document.SetDouble("salary", salary);
    document.SetString("createdAtUtc", DateTimeOffset.UtcNow.ToString("O"));

    collection.Save(document);
    Console.WriteLine($"Employee '{id}' created successfully.");
}

static string ReadRequired()
{
    while (true)
    {
        string? value = Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        Console.Write("This field is required. Try again: ");
    }
}


static void ReadEmployee(Collection collection)
{
    Console.Write("Employee ID: ");
    string id = ReadRequired();

    var document = collection.GetDocument(id);
    if (document is null)
    {
        Console.WriteLine("Employee not found.");
        return;
    }

    Console.WriteLine("\\n--- Employee Details ---");
    Console.WriteLine($"ID         : {document.Id}");
    Console.WriteLine($"First Name : {document.GetString("firstName")}");
    Console.WriteLine($"Last Name  : {document.GetString("lastName")}");
    Console.WriteLine($"Email      : {document.GetString("email")}");
    Console.WriteLine($"Department : {document.GetString("department")}");
    Console.WriteLine($"Salary     : {document.GetDouble("salary"):0.00}");
    Console.WriteLine($"Created    : {document.GetString("createdAtUtc")}");
}



static void UpdateEmployee(Collection collection)
{
    Console.Write("Employee ID to update: ");
    string id = ReadRequired();

    var document = collection.GetDocument(id);
    if (document is null)
    {
        Console.WriteLine("Employee not found.");
        return;
    }

    var mutable = document.ToMutable();

    Console.Write($"First name [{document.GetString("firstName")}]: ");
    string? firstName = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(firstName))
        mutable.SetString("firstName", firstName.Trim());

    Console.Write($"Last name [{document.GetString("lastName")}]: ");
    string? lastName = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(lastName))
        mutable.SetString("lastName", lastName.Trim());

    Console.Write($"Email [{document.GetString("email")}]: ");
    string? email = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(email))
        mutable.SetString("email", email.Trim());

    Console.Write($"Department [{document.GetString("department")}]: ");
    string? department = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(department))
        mutable.SetString("department", department.Trim());

    Console.Write($"Salary [{document.GetDouble("salary"):0.00}]: ");
    string? salaryInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(salaryInput))
    {
        if (!double.TryParse(salaryInput, out double salary) || salary < 0)
        {
            Console.WriteLine("Invalid salary. No changes were saved.");
            return;
        }

        mutable.SetDouble("salary", salary);
    }

    mutable.SetString("updatedAtUtc", DateTimeOffset.UtcNow.ToString("O"));
    collection.Save(mutable);
    Console.WriteLine("Employee updated successfully.");
}


static void DeleteEmployee(Collection collection)
{
    Console.Write("Employee ID to delete: ");
    string id = ReadRequired();

    var document = collection.GetDocument(id);
    if (document is null)
    {
        Console.WriteLine("Employee not found.");
        return;
    }

    Console.Write($"Confirm deletion of '{id}'? (y/n): ");
    string? confirmation = Console.ReadLine();

    if (!string.Equals(confirmation?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Deletion cancelled.");
        return;
    }

    collection.Delete(document);
    Console.WriteLine("Employee deleted successfully.");
}
