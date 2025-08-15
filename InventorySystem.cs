using System;
using System.Collections.Generic;

public static class InventorySystem
{
    // Marker interface for inventory items
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // Electronic item class
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => 
            $"{Id}: {Name} ({Brand}) - Qty: {Quantity}, Warranty: {WarrantyMonths} months";
    }

    // Grocery item class
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() => 
            $"{Id}: {Name} - Qty: {Quantity}, Expires: {ExpiryDate:d}";
    }

    // Custom exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // Generic inventory repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists");
            _items.Add(item.Id, item);
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found");
            _items.Remove(id);
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative");
            
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // Inventory manager class
    public class InventoryManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            // Add sample electronic items
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 15, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Headphones", 25, "Sony", 6));

            // Add sample grocery items
            _groceries.AddItem(new GroceryItem(101, "Milk", 50, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(102, "Bread", 30, DateTime.Now.AddDays(3)));
            _groceries.AddItem(new GroceryItem(103, "Eggs", 100, DateTime.Now.AddDays(14)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            var items = repo.GetAllItems();
            Console.WriteLine($"\n{typeof(T).Name}s in inventory:");
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Increased {typeof(T).Name} {id} quantity by {quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error increasing stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed {typeof(T).Name} with ID {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing item: {ex.Message}");
            }
        }

        public void Run()
        {
            Console.Clear();
            Console.WriteLine("=== Inventory Inventory Management System ===");

            // Initialize sample data
            SeedData();

            // Display all items
            PrintAllItems(_groceries);
            PrintAllItems(_electronics);

            // Test error scenarios
            Console.WriteLine("\nTesting error scenarios:");

            // Try to add duplicate item
            Console.WriteLine("\nAttempting to add duplicate electronic item...");
            try
            {
                _electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 5, "HP", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            // Try to remove non-existent item
            Console.WriteLine("\nAttempting to remove non-existent grocery item...");
            RemoveItemById(_groceries, 999);

            // Try to update with invalid quantity
            Console.WriteLine("\nAttempting to update with negative quantity...");
            try
            {
                _electronics.UpdateQuantity(1, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            // Interactive operations
            Console.WriteLine("\nInteractive Operations:");
            Console.WriteLine("1. Increase stock quantity");
            Console.WriteLine("2. Remove an item");
            Console.Write("Select an operation (1-2): ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.Write("Enter item type (1-Electronics, 2-Groceries): ");
                if (int.TryParse(Console.ReadLine(), out int typeChoice))
                {
                    if (typeChoice == 1)
                    {
                        Console.Write("Enter Electronic item ID: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            if (choice == 1)
                            {
                                Console.Write("Enter quantity to add: ");
                                if (int.TryParse(Console.ReadLine(), out int qty))
                                {
                                    IncreaseStock(_electronics, id, qty);
                                }
                            }
                            else if (choice == 2)
                            {
                                RemoveItemById(_electronics, id);
                            }
                        }
                    }
                    else if (typeChoice == 2)
                    {
                        Console.Write("Enter Grocery item ID: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            if (choice == 1)
                            {
                                Console.Write("Enter quantity to add: ");
                                if (int.TryParse(Console.ReadLine(), out int qty))
                                {
                                    IncreaseStock(_groceries, id, qty);
                                }
                            }
                            else if (choice == 2)
                            {
                                RemoveItemById(_groceries, id);
                            }
                        }
                    }
                }
            }

            // Show final inventory
            Console.WriteLine("\nFinal Inventory Status:");
            PrintAllItems(_groceries);
            PrintAllItems(_electronics);
        }
    }

    public static void Run()
    {
        var manager = new InventoryManager();
        manager.Run();
    }
}