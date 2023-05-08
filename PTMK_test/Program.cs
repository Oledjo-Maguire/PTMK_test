using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;

namespace PTMK_test
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: PTMK_test <operation>");
                return;
            }

            string connectionString = "Server=localhost;Database=test;Uid=root;Pwd=Oledjo001;";

            switch (args[0])
            {
                case "1":
                    CreateTable(connectionString);
                    break;
                case "2":
                    if (args.Length != 4)
                    {
                        Console.WriteLine("Usage: MyApp 2 <name> <birthdate> <gender>");
                        return;
                    }
                    string name = args[1];
                    DateTime birthDate = DateTime.Parse(args[2]);
                    string gender = args[3];
                    InsertRecord(connectionString, name, birthDate, gender);
                    break;
                case "3":
                    SelectRecords(connectionString);
                    break;
                case "4":
                    InsertMillionRecords(connectionString);
                    InsertHundredMaleFRecords(connectionString);
                    break;
                case "5":
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    SelectMaleFRecords(connectionString);
                    sw.Stop();
                    Console.WriteLine($"Query executed in {sw.ElapsedMilliseconds} ms.");

                    break;
                default:
                    Console.WriteLine("Unknown operation");
                    break;
            }
        }

        static void CreateTable(string connectionString)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("CREATE TABLE IF NOT EXISTS people (id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(255), birth_date DATE, gender VARCHAR(1))", connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Table created");
            }
        }
        static void SelectRecords(string connectionString)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT name, birth_date, gender, TIMESTAMPDIFF(YEAR, birth_date, CURDATE()) AS age FROM people GROUP BY name, birth_date ORDER BY name", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        DateTime birthDate = reader.GetDateTime(1);
                        string gender = reader.GetString(2);
                        int age = reader.GetInt32(3);
                        Console.WriteLine($"Name: {name}\nBirth Date: {birthDate}\nGender: {gender}\nAge: {age}");
                    }
                }
            }
        }
        static void InsertRecord(string connectionString, string name, DateTime birthDate, string gender)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO people (name, birth_date, gender) VALUES (@name, @birthDate, @gender)", connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@birthDate", birthDate);
                command.Parameters.AddWithValue("@gender", gender);
                command.ExecuteNonQuery();
                Console.WriteLine("Record inserted");
            }
        }

        private static void InsertMillionRecords(string connectionString)
        {

            var random = new Random();
            var genders = new[] { "M", "F" };
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < 1000000; i++)
            {
                var name = alphabet[random.Next(alphabet.Length)].ToString();
                var birthDate = new DateTime(random.Next(1950, 2010), random.Next(1, 13), random.Next(1, 29));
                var gender = genders[random.Next(genders.Length)];
                InsertRecord(connectionString, name, birthDate, gender);

            }

        }

        private static void InsertHundredMaleFRecords(string connectionString)
        {

            var random = new Random();
            var name = "F" + new string('A', 49);
            var birthDate = new DateTime(random.Next(1950, 2010), random.Next(1, 13), random.Next(1, 29));
            var gender = "M";
            for (int i = 0; i < 100; i++)
            {
                InsertRecord(connectionString, name, birthDate, gender);
                birthDate = new DateTime(random.Next(1950, 2010), random.Next(1, 13), random.Next(1, 29));

            }

        }

        private static void SelectMaleFRecords(string connectionString)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var watch = Stopwatch.StartNew();
                var command = new MySqlCommand("SELECT * FROM people WHERE name LIKE 'F%' AND gender = 'M'", connection);
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("ID\tName\tBirthDate\tGender");
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var name = reader.GetString(1);
                        var birthDate = reader.GetDateTime(2);
                        var gender = reader.GetString(3);
                        Console.WriteLine($"{id}\t{name}\t{birthDate.ToShortDateString()}\t{gender}");
                    }
                }
                watch.Stop();
                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds}ms");
            }
        }
    }
}