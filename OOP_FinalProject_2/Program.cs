using OOP_FinalProject_2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using log4net;
using log4net.Config;

namespace OOP_FinalProject_2
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            // Налаштування log4net
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                List<JewelryStore> stores = new List<JewelryStore>();

                // зчитування даних з файлу
                using (StreamReader reader = new StreamReader("stores.txt"))
                {
                    List<string> lines = new List<string>();
                    string line = null;

                    // Читання файлу рядок за рядком і збереження кожного рядка в списку lines
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Якщо зчитаний рядок містить "---", то це означає, що були зчитані всі дані про попередній магазин
                        if (line == "---")
                        {
                            // зчитування даних про магазин
                            string address = lines[0]; // адреса магазину
                            int count = int.Parse(lines[1]); // кількість прикрас в магазині

                            // Створення списку для зберігання прикрас, що продаються в магазині
                            List<Jewelry> jewelryList = new List<Jewelry>();
                            for (int i = 0; i < count; i++)
                            {
                                // Зчитування даних про кожну прикрасу і додавання її в список
                                string name = lines[i * 4 + 2]; // назва прикраси
                                string metal = lines[i * 4 + 3]; // метал прикраси
                                double weight = double.Parse(lines[i * 4 + 4]); // вага прикраси
                                double price = double.Parse(lines[i * 4 + 5]); // ціна прикраси

                                // Створення об'єкту класу Jewelry і додавання його в список прикрас магазину
                                Jewelry jewelry = new Jewelry
                                {
                                    Name = name,
                                    Metal = metal,
                                    Weight = weight,
                                    Price = price
                                };
                                jewelryList.Add(jewelry);
                            }

                            // Створення об'єкту класу JewelryStore зі зчитаними даними і додавання його в список магазинів
                            JewelryStore store = new JewelryStore
                            {
                                Address = address,
                                Count = count,
                                JewelryList = jewelryList
                            };
                            stores.Add(store);

                            // очищення буфера рядків
                            lines.Clear();
                        }
                        else
                        {
                            // Якщо зчитаний рядок не містить "---", то додавати його до списку lines
                            lines.Add(line);
                        }
                    }
                }

                // Серіалізація у XML формат
                var serializer = new XmlSerializer(typeof(List<JewelryStore>));
                using (var writer = new StreamWriter("data.xml"))
                {
                    serializer.Serialize(writer, stores);
                }

                // Десереалізація з XML формату
                using (var reader = new StreamReader("data.xml"))
                {
                    var deserializer = new XmlSerializer(typeof(List<JewelryStore>));
                    var stores1 = (List<JewelryStore>)deserializer.Deserialize(reader);

                    // Виведення даних на екран
                    foreach (var store in stores1)
                    {
                        Console.WriteLine(store.Address);
                        foreach (var item in store.JewelryList)
                        {
                            Console.WriteLine($"- {item.Name} ({item.Metal}, {item.Weight}g, ${item.Price})");
                        }
                    }


                    // список всіх назв металу (без повторів), що присутні у ювелірних виробах магазинів
                    Dictionary<string, int> metals = new Dictionary<string, int>();
                    foreach (JewelryStore store in stores)
                    {
                        foreach (Jewelry jewelry in store.JewelryList)
                        {
                            if (!metals.ContainsKey(jewelry.Metal))
                            {
                                metals.Add(jewelry.Metal, 1);
                            }
                            else
                            {
                                metals[jewelry.Metal]++;
                            }
                        }
                    }

                    // запис списку всіх назв металу (без повторів) у файл
                    using (StreamWriter writer = new StreamWriter("metals.txt"))
                    {
                        foreach (KeyValuePair<string, int> pair in metals)
                        {
                            writer.WriteLine($"{pair.Key} - {pair.Value}");
                        }
                    }

                    // список прикрас з магазинів, загальна сума виробів в яких не менше 500
                    // фільтрація за умовою
                    var filteredStores = stores.Where(store => store.JewelryList.Sum(j => j.Price) >= 500);

                    // сортування та виведення в файл
                    using (StreamWriter writer = new StreamWriter("jewelry.txt"))
                    {
                        foreach (var store in filteredStores)
                        {
                            var sortedJewelry = store.JewelryList.OrderBy(j => j.Name);

                            foreach (var jewelry in sortedJewelry)
                            {
                                writer.WriteLine($"Магазин: {store.Address}, Прикраса: {jewelry.Name}, Метал: {jewelry.Metal}, Вага: {jewelry.Weight} г, Ціна: {jewelry.Price} грн");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Логування винятка
                log.Error("Exception occurred", ex);
            }

        }
    }
}
