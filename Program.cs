using System;

while (true)
{
    Console.Clear();
    Console.WriteLine("=== DCIT 318 - Programming II Assignment 3 ===");
    Console.WriteLine("1. Finance Management System (Q1)");
    Console.WriteLine("2. Healthcare System (Q2)");
    Console.WriteLine("3. Warehouse Inventory System (Q3)");
    Console.WriteLine("4. School Grading System (Q4)");
    Console.WriteLine("5. Inventory Record System (Q5)");
    Console.WriteLine("6. Exit");
    Console.Write("Select an option (1-6): ");

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string choice = Console.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    switch (choice)
    {
        case "1":
            FinanceSystem.Run();
            break;
        case "2":
            HealthcareSystem.Run();
            break;
        case "3":
            WarehouseSystem.Run();
            break;
        case "4":
            GradingSystem.Run();
            break;
        case "5":
            InventorySystem.Run();
            break;
        case "6":
            Console.WriteLine("Exiting program...");
            return;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }

    Console.WriteLine("\nPress any key to return to main menu...");
    Console.ReadKey();
}
