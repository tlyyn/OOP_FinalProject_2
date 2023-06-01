using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_FinalProject_2;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OOP_FinalProject_2.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void TestSerializationAndDeserialization()
        {
            // Arrange
            List<JewelryStore> stores = new List<JewelryStore>();

            // Заповнення даними для тестування серіалізації
            JewelryStore store1 = new JewelryStore
            {
                Address = "Address1",
                Count = 2,
                JewelryList = new List<Jewelry>
                {
                    new Jewelry { Name = "Necklace", Metal = "Gold", Weight = 10.5, Price = 1000 },
                    new Jewelry { Name = "Ring", Metal = "Silver", Weight = 5.2, Price = 500 }
                }
            };
            JewelryStore store2 = new JewelryStore
            {
                Address = "Address2",
                Count = 1,
                JewelryList = new List<Jewelry>
                {
                    new Jewelry { Name = "Earrings", Metal = "Gold", Weight = 3.8, Price = 800 }
                }
            };
            stores.Add(store1);
            stores.Add(store2);

            // Виконання серіалізації
            var serializer = new XmlSerializer(typeof(List<JewelryStore>));
            using (var writer = new StreamWriter("test_data.xml"))
            {
                serializer.Serialize(writer, stores);
            }

            // Act
            List<JewelryStore> deserializedStores;
            using (var reader = new StreamReader("test_data.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<JewelryStore>));
                deserializedStores = (List<JewelryStore>)deserializer.Deserialize(reader);
            }

            // Assert
            Assert.AreEqual(stores.Count, deserializedStores.Count);

            for (int i = 0; i < stores.Count; i++)
            {
                JewelryStore originalStore = stores[i];
                JewelryStore deserializedStore = deserializedStores[i];

                Assert.AreEqual(originalStore.Address, deserializedStore.Address);
                Assert.AreEqual(originalStore.Count, deserializedStore.Count);

                Assert.AreEqual(originalStore.JewelryList.Count, deserializedStore.JewelryList.Count);

                for (int j = 0; j < originalStore.JewelryList.Count; j++)
                {
                    Jewelry originalJewelry = originalStore.JewelryList[j];
                    Jewelry deserializedJewelry = deserializedStore.JewelryList[j];

                    Assert.AreEqual(originalJewelry.Name, deserializedJewelry.Name);
                    Assert.AreEqual(originalJewelry.Metal, deserializedJewelry.Metal);
                    Assert.AreEqual(originalJewelry.Weight, deserializedJewelry.Weight);
                    Assert.AreEqual(originalJewelry.Price, deserializedJewelry.Price);
                }
            }
        }
    }
}
