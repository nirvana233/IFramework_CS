using System;
using System.Collections.Generic;
using IFramework;
using IFramework.Serialization.DataTable;
using IFramework.Serialization;
namespace Example
{
    public class SerializationTest : Test
    {
        struct HuMan
        {
            public int age;
            [DataColumnName("The Sex")]
            public string sex;
            public string Name;
            [DataReadColumnIndex(0)]
            public int heigth;
            [DataIgnore][NonSerialized]
            public int width;
        }
        string path= "Mans.csv";


        public void String()
        {
            HuMan hu = new HuMan() { age = 6, Name = "xxl", sex = "sex", heigth = 16, width = 20 };
            Dictionary<int, HuMan> hus = new Dictionary<int, HuMan>() {
                { 1,hu},
                {2,hu }
            };

            var str = StringConvert.ConvertToString(hus);
            Console.WriteLine(str);
            Dictionary<int, HuMan> hus2;
            Console.WriteLine(StringConvert.TryConvert(str, out hus2));
            Console.WriteLine(hus2);
        }
        protected override void Start()
        {
           // String();
           // Write();
            Read();
        }

        //ICsvRow 需要根据具体情况改写一下分离字符串
        //ICsvExplainer 基本不用改
        //读取时候 具体的类型需要有一个公共的无参数构造函数，否则ICsvExplainer 需要改动
        void Read()
        {
            var r =
                DataTableTool.CreateReader(new System.IO.StreamReader(path, System.Text.Encoding.UTF8),
                new DataRow(),
                new DataExplainer());
            var cc = r.Get<HuMan>();
            foreach (var c in cc)
            {
                Log.L(string.Format("Age {0}   Sex  {1}  Name   {2} height {3} width {4}", c.age, c.sex, c.Name,c.heigth,c.width));
            }
            r.Dispose();
        }
        void Write()
        {
            List<HuMan> cs = new List<HuMan>()
            {
            new HuMan(){ age=1,sex="m",Name="xm"},
            new HuMan(){ age=2,sex="m1",Name="xm1"},
            new HuMan(){ age=3,sex="m2",Name="xm2"},
            };
            var w = DataTableTool.CreateWriter(new System.IO.StreamWriter(path, false),
                new DataRow(),
                new DataExplainer());
            w.Write(cs);
            w.Dispose();
        }

       

        protected override void Update()
        {
            
        }

        protected override void Stop()
        {
            
        }
    }
}
