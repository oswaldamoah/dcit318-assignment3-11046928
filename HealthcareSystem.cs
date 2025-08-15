using System;
using System.Collections.Generic;
using System.Linq;

public static class HealthcareSystem
{
    // Patient class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() => $"{Id}: {Name}, {Age}, {Gender}";
    }

    // Prescription class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() => $"{MedicationName} (issued: {DateIssued:d})";
    }

    // Generic Repository
    public class Repository<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);
        public List<T> GetAll() => _items;
        
        public T? GetById(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);
        
        public bool Remove(Func<T, bool> predicate)
        {
            var item = _items.FirstOrDefault(predicate);
            return item != null && _items.Remove(item);
        }
    }

    // Main Health System Application
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            // Add sample patients
            _patientRepo.Add(new Patient(1, "John Doe", 35, "Male"));
            _patientRepo.Add(new Patient(2, "Jane Smith", 28, "Female"));
            _patientRepo.Add(new Patient(3, "Alice Johnson", 42, "Female"));

            // Add sample prescriptions
            _prescriptionRepo.Add(new Prescription(1, 1, "Ibuprofen", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Amoxicillin", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(4, 2, "Vitamin D", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(5, 3, "Antihistamine", DateTime.Now));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo.GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("\nRegistered Patients:");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var patient = _patientRepo.GetById(p => p.Id == patientId);
            if (patient == null)
            {
                Console.WriteLine($"Patient with ID {patientId} not found.");
                return;
            }

            Console.WriteLine($"\nPrescriptions for {patient.Name}:");

            if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
            {
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine($"- {prescription}");
                }
            }
            else
            {
                Console.WriteLine("No prescriptions found for this patient.");
            }
        }

        public void Run()
        {
            Console.Clear();
            Console.WriteLine("=== Healthcare Management System ===");

            // Initialize data
            SeedData();
            BuildPrescriptionMap();

            // Display all patients
            PrintAllPatients();

            // Get patient ID from user
            Console.Write("\nEnter Patient ID to view prescriptions (or 0 to exit): ");
            while (true)
            {
                if (!int.TryParse(Console.ReadLine(), out int patientId))
                {
                    Console.Write("Invalid input. Please enter a number: ");
                    continue;
                }

                if (patientId == 0) break;

                PrintPrescriptionsForPatient(patientId);
                Console.Write("\nEnter another Patient ID (or 0 to exit): ");
            }
        }
    }

    public static void Run()
    {
        var app = new HealthSystemApp();
        app.Run();
    }
}