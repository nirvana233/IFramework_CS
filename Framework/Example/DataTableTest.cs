using System.Collections.Generic;
using IFramework;
using IFramework.Serialization.DataTable;

namespace Example
{
    public class DataTableTest : Test
    {
        class Man
        {
            [DataIgnore]
            public int age;
            [DataColumnName("性别")]
            public string sex;
            public string Name;
        }
        string path= "Mans.csv";
        protected override void Start()
        {
            //Write();
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
            var cc = r.Get<Man>();
            foreach (var c in cc)
            {
                Log.L(string.Format("Age {0}   Sex  {1}  Name   {2}", c.age, c.sex, c.Name));
            }
            r.Dispose();
        }
        void Write()
        {
            List<Man> cs = new List<Man>()
            {
            new Man(){ age=1,sex="m",Name="xm"},
            new Man(){ age=2,sex="m1",Name="xm1"},
            new Man(){ age=3,sex="m2",Name="xm2"},
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
