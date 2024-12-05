using System;
using System.Collections.Generic;
using static System.Console;
using System.IO;
using System.Linq;

namespace DictionaryApp
{
    class Program
    {
        static Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

        static void AddWord()
        {
            Write("Введіть слово українською: ");
            string word = ReadLine()?.Trim();

            if (string.IsNullOrEmpty(word))
            {
                return;
            }

            Write("Введіть переклад (якщо перекладів декілька, розділіть комами): ");
            string translations = ReadLine()?.Trim();

            if (string.IsNullOrEmpty(translations))
            {
                return;
            }

            dictionary[word] = translations.Split(',').Select(t => t.Trim()).ToList();

            WriteLine("Слово з перекладом додано");
        }

        static void ChangeWordOrTranslation()
        {
            Write("Введіть слово українською для заміни: ");
            string word = ReadLine()?.Trim();
            if (!dictionary.ContainsKey(word))
            {
                WriteLine("Слово не знайдено");
                return;
            }

            WriteLine("1. Замінити слово");
            WriteLine("2. Замінити переклад");
            WriteLine("3. Повернутися до головного меню");
            string choice = ReadLine();

            switch (choice)
            {
                case "1":
                    ChangeWord(word);
                    break;
                case "2":
                    ChangeTranslation(word);
                    break;
                case "3":
                    return;
                default:
                    WriteLine("Невірний вибір");
                    break;
            }
        }

        static void ChangeWord(string oldWord)
        {
            Write("Введіть нове слово: ");
            string newWord = ReadLine()?.Trim();
            if (string.IsNullOrEmpty(newWord)) return;

            dictionary[newWord] = dictionary[oldWord];
            dictionary.Remove(oldWord);
            WriteLine("Слово замінено");
        }

        static void ChangeTranslation(string word)
        {
            Write("Введіть старий переклад: ");
            string oldTranslation = ReadLine()?.Trim();
            if (!dictionary[word].Contains(oldTranslation))
            {
                WriteLine("Переклад не знайдено");
                return;
            }

            Write("Введіть новий переклад: ");
            string newTranslation = ReadLine()?.Trim();
            dictionary[word].Remove(oldTranslation);
            dictionary[word].Add(newTranslation);
            WriteLine("Переклад замінено");
        }

        static void DeleteWordOrTranslation()
        {
            Write("Введіть слово українською для видалення: ");
            string word = ReadLine()?.Trim();

            if (!dictionary.ContainsKey(word))
            {
                WriteLine("Слово не знайдено");
                return;
            }

            WriteLine("1. Видалити все слово");
            WriteLine("2. Видалити один переклад");
            WriteLine("3. Повернутися до головного меню");
            string choice = ReadLine();

            switch (choice)
            {
                case "1":
                    DeleteWord(word);
                    break;
                case "2":
                    DeleteTranslation(word);
                    break;
                case "3":
                    return;
                default:
                    WriteLine("Невірний вибір");
                    break;
            }
        }

        static void DeleteWord(string word)
        {
            dictionary.Remove(word);
            WriteLine("Слово видалено");
        }

        static void DeleteTranslation(string word)
        {
            Write("Введіть переклад для видалення: ");
            string translation = ReadLine()?.Trim();

            if (!dictionary[word].Contains(translation))
            {
                WriteLine("Переклад не знайдено");
                return;
            }

            if (dictionary[word].Count == 1)
            {
                WriteLine("Це єдиний переклад. Неможливо видалити");
                return;
            }

            dictionary[word].Remove(translation);
            WriteLine("Переклад видалено");
        }

        static void FindTranslation()
        {
            Write("Введіть слово українською: ");
            string word = ReadLine()?.Trim();

            if (dictionary.TryGetValue(word, out var translations))
            {
                WriteLine($"Переклади: {string.Join(", ", translations)}");
            }
            else
            {
                WriteLine("Слово не знайдено");
            }
        }

        static void ExportWord()
        {
            Write("Введіть слово українською для експорту: ");
            string word = ReadLine()?.Trim();

            if (!dictionary.ContainsKey(word))
            {
                WriteLine("Слово не знайдено.");
                return;
            }

            Write("Введіть ім'я файлу для експорту (наприклад word.txt): ");
            string fileName = ReadLine()?.Trim();
            if (string.IsNullOrEmpty(fileName)) return;

            File.WriteAllText(fileName, $"{word}: {string.Join(", ", dictionary[word])}");
            WriteLine("Слово експортовано у файл");
        }

        static void SaveDictionaryToFile()
        {
            Write("Введіть ім'я файлу для збереження словника (наприклад, dictionary.txt): ");
            string fileName = ReadLine()?.Trim();
            if (string.IsNullOrEmpty(fileName)) return;

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach (var entry in dictionary)
                {
                    writer.WriteLine($"{entry.Key}: {string.Join(", ", entry.Value)}");
                }
            }

            WriteLine("Словник збережено у файл.");
        }

        static void MainMenu()
        {
            WriteLine("Головне меню:");
            WriteLine("1. Додати слово");
            WriteLine("2. Змінити слово або переклад");
            WriteLine("3. Видалити слово або переклад");
            WriteLine("4. Знайти переклад");
            WriteLine("5. Експортувати слово");
            WriteLine("6. Зберегти словник у файл");
            WriteLine("7. Вихід");
            Write("Ваш вибір: ");
            string choice = ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    AddWord();
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
                    SaveDictionaryToFile();
                    break;
                case "7":
                    Environment.Exit(0);
                    break;
                default:
                    WriteLine("Невірний вибір. Спробуйте ще раз.");
                    break;
            }
            WriteLine("Натисніть будь-яку клавішу для повернення до головного меню");
            ReadKey();
        }

        static void Main(string[] args)
        {
            while (true)
            {
                MainMenu();
            }
        }
    }
}
