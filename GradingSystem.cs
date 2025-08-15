using System;
using System.Collections.Generic;
using System.IO;

public static class GradingSystem
{
    // Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            return Score switch
            {
                >= 80 and <= 100 => "A",
                >= 70 and < 80 => "B",
                >= 60 and < 70 => "C",
                >= 50 and < 60 => "D",
                _ => "F"
            };
        }
    }

    // Custom exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // Student result processor
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();
            int lineNumber = 0;

            using (var reader = new StreamReader(inputFilePath))
            {
                while (!reader.EndOfStream)
                {
                    lineNumber++;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    string line = reader.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var parts = line.Split(',');
                        if (parts.Length != 3)
                        {
                            throw new MissingFieldException(
                                $"Line {lineNumber}: Expected 3 fields but found {parts.Length}");
                        }

                        if (!int.TryParse(parts[0].Trim(), out int id))
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Invalid ID format '{parts[0]}'");
                        }

                        string fullName = parts[1].Trim();
                        if (string.IsNullOrWhiteSpace(fullName))
                        {
                            throw new MissingFieldException(
                                $"Line {lineNumber}: Missing student name");
                        }

                        if (!int.TryParse(parts[2].Trim(), out int score))
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Invalid score format '{parts[2]}'");
                        }

                        students.Add(new Student(id, fullName, score));
                    }
                    catch (Exception ex) when (
                        ex is MissingFieldException || 
                        ex is InvalidScoreFormatException)
                    {
                        Console.WriteLine($"Skipping line {lineNumber}: {ex.Message}");
                    }
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine(
                        $"{student.FullName} (ID: {student.Id}): " +
                        $"Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    public static void Run()
    {
        Console.Clear();
        Console.WriteLine("=== School Grading System ===");

        try
        {
            // Get input file path from user
            Console.Write("Enter path to input file (or press Enter for default): ");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string inputPath = Console.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                inputPath = "students.txt";
                // Create sample file if it doesn't exist
                if (!File.Exists(inputPath))
                {
                    File.WriteAllText(inputPath, 
                        "101, John Doe, 85\n" +
                        "102, Jane Smith, 72\n" +
                        "103, Alice Johnson, 91\n" +
                        "104, Bob Brown, 58\n" +
                        "105, Charlie Davis, 42\n" +
                        "106, Invalid Student, abc\n" +  // Invalid score
                        "107, Missing Field\n" +         // Missing score
                        "108, Emily Wilson, 95");
                }
            }

            // Get output file path from user
            Console.Write("Enter path for output report (or press Enter for default): ");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string outputPath = Console.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = "grades_report.txt";
            }

            var processor = new StudentResultProcessor();
            
            // Read and process students
            Console.WriteLine($"\nReading student data from {inputPath}...");
            var students = processor.ReadStudentsFromFile(inputPath);
            Console.WriteLine($"Successfully processed {students.Count} students.");

            // Generate report
            Console.WriteLine($"\nGenerating report to {outputPath}...");
            processor.WriteReportToFile(students, outputPath);
            Console.WriteLine("Report generated successfully!");

            // Display sample of report
            Console.WriteLine("\nSample of generated report:");
            for (int i = 0; i < Math.Min(3, students.Count); i++)
            {
                var s = students[i];
                Console.WriteLine($"  {s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
            }
            if (students.Count > 3)
            {
                Console.WriteLine($"  ... and {students.Count - 3} more");
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: Input file not found - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}