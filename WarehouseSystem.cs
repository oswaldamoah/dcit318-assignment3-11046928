using System;
using System.Collections.Generic;

public static class WarehouseSystem
{
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

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
            $"{Id}: {Name} ({Brand}), Qty: {Quantity}, Warranty: {WarrantyMonths} months";
    }

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
            $"{Id}: {Name}, Qty: {Quantity}, Expiry: {ExpiryDate:d}";
    }

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

    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");

            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            Console.Write("\nHow many electronic items to add? ");
            if (int.TryParse(Console.ReadLine(), out int eCount) && eCount > 0)
            {
                for (int i = 0; i < eCount; i++)
                {
                    Console.WriteLine($"\nElectronic Item {i + 1}:");
                    Console.Write("ID: ");
                    int id = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Name: ");
                    string name = Console.ReadLine() ?? "";
                    Console.Write("Quantity: ");
                    int qty = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Brand: ");
                    string brand = Console.ReadLine() ?? "";
                    Console.Write("Warranty (months): ");
                    int warranty = int.Parse(Console.ReadLine() ?? "0");

                    try { _electronics.AddItem(new ElectronicItem(id, name, qty, brand, warranty)); }
                    catch (DuplicateItemException ex) { Console.WriteLine($"Error: {ex.Message}"); }
                }
            }

            Console.Write("\nHow many grocery items to add? ");
            if (int.TryParse(Console.ReadLine(), out int gCount) && gCount > 0)
            {
                for (int i = 0; i < gCount; i++)
                {
                    Console.WriteLine($"\nGrocery Item {i + 1}:");
                    Console.Write("ID: ");
                    int id = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Name: ");
                    string name = Console.ReadLine() ?? "";
                    Console.Write("Quantity: ");
                    int qty = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Expiry date (yyyy-MM-dd): ");
                    DateTime expiry = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString("yyyy-MM-dd"));

                    try { _groceries.AddItem(new GroceryItem(id, name, qty, expiry)); }
                    catch (DuplicateItemException ex) { Console.WriteLine($"Error: {ex.Message}"); }
                }
            }
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            var items = repo.GetAllItems();
            if (items.Count == 0) Console.WriteLine("No items found.");
            else foreach (var item in items) Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock updated for {item.Name}. New quantity: {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing item: {ex.Message}");
            }
        }

        public void RunDemo()
        {
            Console.WriteLine("\n--- Grocery Items ---");
            PrintAllItems(_groceries);

            Console.WriteLine("\n--- Electronic Items ---");
            PrintAllItems(_electronics);

            Console.WriteLine("\n--- Testing Exceptions ---");

            try
            {
                _electronics.AddItem(new ElectronicItem(1, "Camera", 3, "Canon", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Duplicate Add Error: {ex.Message}");
            }

            RemoveItemById(_groceries, 99);

            try
            {
                _groceries.UpdateQuantity(1, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Invalid Quantity Error: {ex.Message}");
            }
        }
    }

    public static void Run()
    {
        var manager = new WareHouseManager();
        Console.WriteLine("=== Warehouse Inventory System ===");
        manager.SeedData();
        manager.RunDemo();
    }
}
