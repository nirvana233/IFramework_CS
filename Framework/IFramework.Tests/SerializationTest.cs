using IFramework.Modules.Fsm;
using IFramework.Net;
using IFramework.Serialization;
using IFramework.Serialization.DataTable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace IFramework.Tests
{
    [TestClass()]
    public class SerializationTest
    {
        struct HuMan
        {
            public int age;
            [DataColumnName("The Sex")]
            public string sex;
            public string Name;
            [DataReadColumnIndex(0)]
            public int heigth;
            [DataIgnore]
            [NonSerialized]
            public int width;
        }

        [TestMethod]
        public void DeserializeString()
        {
            var str = "[{k:1,v:{age:6,sex:\"sex\",Name:\"xxl\",heigth:16,}},{k:2,v:{age:6,sex:\"sex\",Name:\"xxl\",heigth:16,}}]";
            var success = StringConvert.TryConvert(str, out Dictionary<int, HuMan> hus);
            Assert.IsTrue(success);
            Assert.AreEqual(hus.Count, 2);
            Assert.IsTrue(hus.ContainsKey(1));
            Assert.AreEqual(hus[1].age, 6);
            Assert.AreEqual(hus[1].Name, "xxl");
            Assert.AreEqual(hus[1].heigth, 16);
            Assert.AreEqual(hus[1].width, 0);
        }

        [TestMethod]
        public void SerializeString()
        {
            var hu = new HuMan() { age = 6, Name = "xxl", sex = "sex", heigth = 16, width = 20 };
            var hus = new Dictionary<int, HuMan>() { { 1, hu }, { 2, hu } };
            var str = StringConvert.ConvertToString(hus);
            var strRight = "[{k:1,v:{age:6,sex:\"sex\",Name:\"xxl\",heigth:16,}},{k:2,v:{age:6,sex:\"sex\",Name:\"xxl\",heigth:16,}}]";
            Assert.AreEqual(str, strRight);
        }

        [TestMethod]
        public void SerializeToFile()
        {
            var path = "Mans.csv";
            var hus = new List<HuMan>()
            {
                new HuMan(){ age=1,sex="m",Name="xm"},
                new HuMan(){ age=2,sex="m1",Name="xm1"},
                new HuMan(){ age=3,sex="m2",Name="xm2"},
            };
            var writer = DataTableTool.CreateWriter(new StreamWriter(path, false), new DataRow(), new DataExplainer());
            writer.Write(hus);
            writer.Dispose();
            var txt = File.ReadAllText(path);
            var txtRight = "age,The Sex,Name,heigth,\r\n1,◜m◜,◜xm◜,0,\r\n2,◜m1◜,◜xm1◜,0,\r\n3,◜m2◜,◜xm2◜,0,\r\n";
            Assert.AreEqual(txt, txtRight);
        }

        [TestMethod]
        public void DeserializeFromFile()
        {
            var txtRight = "age,The Sex,Name,heigth,\r\n1,◜m◜,◜xm◜,0,\r\n2,◜m1◜,◜xm1◜,0,\r\n3,◜m2◜,◜xm2◜,0,\r\n";
            var path = "Mans.csv";
            File.WriteAllText(path, txtRight);
            var reader = DataTableTool.CreateReader(new StreamReader(path, Encoding.UTF8), new DataRow(), new DataExplainer());
            var hus = reader.Get<HuMan>();
            reader.Dispose();
            Assert.AreEqual(hus.Count, 3);
            Assert.AreEqual(hus[0].age, 1);
            Assert.AreEqual(hus[0].sex, "m");
            Assert.AreEqual(hus[0].Name, "xm");
            Assert.AreEqual(hus[1].age, 2);
            Assert.AreEqual(hus[1].sex, "m1");
            Assert.AreEqual(hus[1].Name, "xm1");
            Assert.AreEqual(hus[2].age, 3);
            Assert.AreEqual(hus[2].sex, "m2");
            Assert.AreEqual(hus[2].Name, "xm2");
        }
    }
}
