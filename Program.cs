using System;
using System.Collections.Generic;
using static System.Console;
using System.IO;
using System.Linq;

namespace DictionaryApp
{
    class Program
    {
        static List<Dictionary> dictionaries = new List<Dictionary>();

        static void Main(string[] args)
        {
            LoadDictionaries();
            while (true)
            {
                ShowMainMenu();
            }
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("Головне меню:");
            Console.WriteLine("1. Створити новий словник");
            Console.WriteLine("2. Вибрати існуючий словник");
            Console.WriteLine("3. Вихід");
            Console.Write("Ваш вибір: ");

            string choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1":
                    CreateDictionary();
                    break;
                case "2":
                    ChooseDictionary();
                    break;
                case "3":
                    SaveDictionaries();
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    break;
            }
        }

        static void CreateDictionary()
        {
            Console.Write("Введіть тип словника (наприклад, англо-український): ");
            string type = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(type))
            {
                Console.WriteLine("Тип словника не може бути порожнім.");
                return;
            }

            dictionaries.Add(new Dictionary(type));
            Console.WriteLine($"Словник '{type}' створено.");
        }

        static void ChooseDictionary()
        {
            if (dictionaries.Count == 0)
            {
                Console.WriteLine("Немає створених словників.");
                return;
            }

            Console.WriteLine("Доступні словники:");
            for (int i = 0; i < dictionaries.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {dictionaries[i].Type}");
            }

            Console.Write("Виберіть номер словника: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= dictionaries.Count)
            {
                dictionaries[index - 1].ShowMenu();
            }
            else
            {
                Console.WriteLine("Невірний вибір");
            }
        }

        static void LoadDictionaries()
        {
            if (File.Exists("dictionaries.txt"))
            {
                var lines = File.ReadAllLines("dictionaries.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length > 1)
                    {
                        var dictionary = new Dictionary(parts[0]);
                        for (int i = 1; i < parts.Length; i += 2)
                        {
                            if (i + 1 < parts.Length)
                            {
                                dictionary.AddWord(parts[i], parts[i + 1].Split(',').ToList());
                            }
                        }
                        dictionaries.Add(dictionary);
                    }
                }
            }
        }

        static void SaveDictionaries()
        {
            using (var writer = new StreamWriter("dictionaries.txt"))
            {
                foreach (var dictionary in dictionaries)
                {
                    writer.WriteLine(dictionary.Serialize());
                }
            }
        }
    }

    class Dictionary
    {
        public string Type { get; }
        private Dictionary<string, List<string>> words = new Dictionary<string, List<string>>();

        public Dictionary(string type)
        {
            Type = type;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine($"Меню словника '{Type}':");
                Console.WriteLine("1. Додати слово");
                Console.WriteLine("2. Змінити слово або переклад");
                Console.WriteLine("3. Видалити слово або переклад");
                Console.WriteLine("4. Знайти переклад");
                Console.WriteLine("5. Експортувати слово");
                Console.WriteLine("6. Повернутися до головного меню");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        AddWordInteractive();
                        break;
                    case "2":
                        ChangeWordOrTranslation();
                        break;
                    case "3":
                        DeleteWordOrTranslation();
                        break;
                    case "4":
                        FindTranslation();
                        break;
                    case "5":
                        ExportWord();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        break;
                }
            }
        }

        public void AddWord(string word, List<string> translations)
        {
            words[word] = translations;
        }

        public void AddWordInteractive()
        {
            Console.Write("Введіть слово: ");
            string word = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(word)) return;

            Console.Write("Введіть переклад (якщо перекладів декілька, розділіть комами): ");
            string translations = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(translations)) return;

            words[word] = translations.Split(',').Select(t => t.Trim()).ToList();
            Console.WriteLine("Слово додано");
        }

        public void ChangeWordOrTranslation()
        {
            Console.Write("Введіть слово для зміни: ");
            string word = Console.ReadLine()?.Trim();

            if (!words.ContainsKey(word))
            {
                Console.WriteLine("Слово не знайдено");
                return;
            }

            Console.WriteLine("1. Замінити слово");
            Console.WriteLine("2. Замінити переклад");
            Console.Write("Ваш вибір: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введіть нове слово: ");
                    string newWord = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(newWord))
                    {
                        words[newWord] = words[word];
                        words.Remove(word);
                        Console.WriteLine("Слово змінено");
                    }
                    break;
                case "2":
                    Console.Write("Введіть старий переклад: ");
                    string oldTranslation = Console.ReadLine()?.Trim();
                    if (words[word].Remove(oldTranslation))
                    {
                        Console.Write("Введіть новий переклад: ");
                        string newTranslation = Console.ReadLine()?.Trim();
                        words[word].Add(newTranslation);
                        Console.WriteLine("Переклад змінено");
                    }
                    else
                    {
                        Console.WriteLine("Переклад не знайдено");
                    }
                    break;
                default:
                    Console.WriteLine("Невірний вибір");
                    break;
            }
        }

        public void DeleteWordOrTranslation()
        {
            Console.Write("Введіть слово для видалення: ");
            string word = Console.ReadLine()?.Trim();

            if (!words.ContainsKey(word))
            {
                Console.WriteLine("Слово не знайдено");
                return;
            }

            Console.WriteLine("1. Видалити слово");
            Console.WriteLine("2. Видалити переклад");
            Console.Write("Ваш вибір: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    words.Remove(word);
                    Console.WriteLine("Слово видалено");
                    break;
                case "2":
                    Console.Write("Введіть переклад для видалення: ");
                    string translation = Console.ReadLine()?.Trim();
                    if (words[word].Count > 1 && words[word].Remove(translation))
                    {
                        Console.WriteLine("Переклад видалено");
                    }
                    else
                    {
                        Console.WriteLine("Переклад не знайдено або це останній варіант");
                    }
                    break;
                default:
                    Console.WriteLine("Невірний вибір");
                    break;
            }
        }

        public void FindTranslation()
        {
            Console.Write("Введіть слово: ");
            string word = Console.ReadLine()?.Trim();

            if (words.TryGetValue(word, out var translations))
            {
                Console.WriteLine($"Переклади: {string.Join(", ", translations)}");
            }
            else
            {
                Console.WriteLine("Слово не знайдено");
            }
        }

        public void ExportWord()
        {
            Console.Write("Введіть слово для експорту: ");
            string word = Console.ReadLine()?.Trim();

            if (!words.ContainsKey(word))
            {
                Console.WriteLine("Слово не знайдено");
                return;
            }

            Console.Write("Введіть ім'я файлу для експорту: ");
            string fileName = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(fileName))
            {
                File.WriteAllText(fileName, $"{word}: {string.Join(", ", words[word])}");
                Console.WriteLine("Слово експортовано");
            }
        }

        public string Serialize()
        {
            var serializedWords = words.Select(w => $"{w.Key}|{string.Join(",", w.Value)}");
            return $"{Type}|{string.Join("|", serializedWords)}";
        }
    }
}
