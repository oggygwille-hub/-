using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

class Homework
{
    public string Subject;
    public string Description;
    public DateTime Deadline; // Changed to DateTime for logic checks
    public bool IsCompleted;
}

class Program
{
    static List<Homework> tasks = new List<Homework>();
    static string filePath = "homework.txt";

    static void Main(string[] args)
    {
        LoadFromFile();
        CheckDeadlines(); // New: Notification on startup
        ShowMenu();
        SaveToFile();
    }

    // --- NEW: NOTIFICATION LOGIC ---
    static void CheckDeadlines()
    {
        Console.WriteLine("\n🔔 [SYSTEM CHECK: DEADLINES]");
        bool upcoming = false;
        DateTime today = DateTime.Today;

        foreach (var hw in tasks)
        {
            if (!hw.IsCompleted)
            {
                // Calculate difference in days
                double daysLeft = (hw.Deadline - today).TotalDays;

                if (daysLeft < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[!] OVERDUE: {hw.Subject} was due on {hw.Deadline.ToShortDateString()}!");
                    upcoming = true;
                }
                else if (daysLeft <= 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[!] URGENT: {hw.Subject} is due within 24 hours!");
                    upcoming = true;
                }
            }
        }

        if (!upcoming) Console.WriteLine("Everything looks on track!");
        Console.ResetColor();
        Console.WriteLine("------------------------------");
    }

    static void ShowMenu()
    {
        string choice;
        do
        {
            Console.WriteLine("\n==== HOMEWORK TRACKER PRO ====");
            Console.WriteLine("1. Add Homework");
            Console.WriteLine("2. View Homework (List)");
            Console.WriteLine("3. Delete Homework");
            Console.WriteLine("4. Mark as Completed");
            Console.WriteLine("5. Search Homework");
            Console.WriteLine("0. Exit");
            Console.Write("Enter choice: ");

            choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AddHomework(); break;
                case "2": ViewHomework(); break;
                case "3": DeleteHomework(); break;
                case "4": MarkCompleted(); break;
                case "5": SearchHomework(); break;
                case "0": Console.WriteLine("Saving data and exiting..."); break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        } while (choice != "0");
    }

    static void AddHomework()
    {
        Homework hw = new Homework();

        Console.Write("Enter Subject: ");
        hw.Subject = Console.ReadLine();

        Console.Write("Enter Description: ");
        hw.Description = Console.ReadLine();

        // Validation for Date
        while (true)
        {
            Console.Write("Enter Deadline (YYYY-MM-DD): ");
            if (DateTime.TryParse(Console.ReadLine(), out hw.Deadline)) break;
            Console.WriteLine("Invalid format. Please use YYYY-MM-DD.");
        }

        hw.IsCompleted = false;
        tasks.Add(hw);
        Console.WriteLine("✅ Homework added successfully!");
    }

    static void ViewHomework()
    {
        Console.WriteLine("\n==== CURRENT HOMEWORK LIST ====");

        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            var hw = tasks[i];

            // Set Color based on status
            if (hw.IsCompleted) Console.ForegroundColor = ConsoleColor.Green;
            else if (hw.Deadline < DateTime.Today) Console.ForegroundColor = ConsoleColor.Red;

            string status = hw.IsCompleted ? "[✔]" : "[ ]";
            Console.WriteLine($"{i + 1}. {status} {hw.Subject,-10} | {hw.Description} (Due: {hw.Deadline.ToShortDateString()})");

            Console.ResetColor();
        }
    }

    static void DeleteHomework()
    {
        ViewHomework();
        if (tasks.Count == 0) return;

        Console.Write("\nEnter ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= tasks.Count)
        {
            tasks.RemoveAt(index - 1);
            Console.WriteLine("🗑️ Deleted.");
        }
        else Console.WriteLine("Invalid selection.");
    }

    static void MarkCompleted()
    {
        ViewHomework();
        if (tasks.Count == 0) return;

        Console.Write("\nEnter ID to mark as done: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= tasks.Count)
        {
            tasks[index - 1].IsCompleted = true;
            Console.WriteLine("🎯 Great job completing your work!");
        }
    }

    static void SearchHomework()
    {
        Console.Write("Search keyword: ");
        string key = Console.ReadLine().ToLower();

        var results = tasks.FindAll(h => h.Subject.ToLower().Contains(key) || h.Description.ToLower().Contains(key));

        if (results.Count > 0)
        {
            foreach (var hw in results)
                Console.WriteLine($"{(hw.IsCompleted ? "[✔]" : "[ ]")} {hw.Subject} - {hw.Deadline.ToShortDateString()}");
        }
        else Console.WriteLine("No results found.");
    }

    static void SaveToFile()
    {
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            foreach (var hw in tasks)
            {
                // Save date in a standard format
                sw.WriteLine($"{hw.Subject}|{hw.Description}|{hw.Deadline:yyyy-MM-dd}|{hw.IsCompleted}");
            }
        }
    }

    static void LoadFromFile()
    {
        if (!File.Exists(filePath)) return;

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] d = line.Split('|');
            if (d.Length < 4) continue;

            tasks.Add(new Homework
            {
                Subject = d[0],
                Description = d[1],
                Deadline = DateTime.Parse(d[2]),
                IsCompleted = bool.Parse(d[3])
            });
        }
    }
}
