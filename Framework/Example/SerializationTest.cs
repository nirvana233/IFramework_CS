using System;
using System.Collections.Generic;
using IFramework;
using IFramework.Serialization.DataTable;
using IFramework.Serialization;
namespace Example
{
    public class SerializationTest : Test
    {
        /// <summary>
        /// 结构体定义
        /// </summary>
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
        string path = "Mans.csv";

        protected override void Start()
        {
            String();
            Write();
            Read();
        }

        /// <summary>
        /// 对象与字符串的相互转换
        /// </summary>
        public void String()
        {
            Log.L("");
            Log.L("开始对象与字符串的相互转换：");
            Log.L("生成一个字典对象,并转换成字符串");
            HuMan hu = new HuMan() { age = 6, Name = "abc", sex = "sex", heigth = 16, width = 20 };
            Dictionary<int, HuMan> hus = new Dictionary<int, HuMan>() {
                { 1, hu },
                { 2, hu }
            };

            var str = StringConvert.ConvertToString(hus);
            Log.L("转换出来的字符串是：");
            Console.WriteLine(str + "\n");
            Log.L("将字符串转换回去：");
            Dictionary<int, HuMan> hus2;
            Log.L("转换" + (StringConvert.TryConvert(str, out hus2) ? "成功" : "失败"));
            Console.WriteLine($"由字符串转换回来的对象：{hus2}\n");
        }

        /// <summary>
        /// 对象的序列化操作
        /// </summary>
        void Write()
        {
            Log.L("类中对字段添加Attribute可以实现一系列操作：");
            Log.L("DataColumnName\t\t将对应标题设置为相应的字符串");
            Log.L("DataIgnore\t\t序列化和反序列化时都忽略此项");
            Log.L("DataReadColumnIndex\t读取时选择读取对应索引列的数据\n");
            Log.L("开始将列表对象序列化到本地：");
            Log.L("生成一个列表对象,并序列化到本地csv文件中");

            List<HuMan> cs = new List<HuMan>()
            {
            new HuMan(){ age=1,sex="m",Name="xm",heigth=0},
            new HuMan(){ age=2,sex="m1",Name="xm1",heigth=0},
            new HuMan(){ age=3,sex="m2",Name="xm2",heigth=0},
            };
            var w = DataTableTool.CreateWriter(new System.IO.StreamWriter(path, false),
                new DataRow(),
                new DataExplainer()); //这里可以设置分离符
            //dot指的是逗号替换的值，quotes是引号替换的值
            //分离符主要用于将带有引号和逗号的值隔离出来，一般选用不常用的字符
            w.Write(cs);
            w.Dispose();
            Log.L("写入完成");
            //非windows系统请注释下面这一句，因为我没体验过其它系统打开文件的方法
            System.Diagnostics.Process.Start("notepad", path);  //用记事本打开写入的文件
        }


        /// <summary>
        /// 对象的反序列化操作
        /// </summary>
        void Read()
        {
            Log.L("");
            Log.L("开始从本地反序列化出对象，注意分离符要与写入时一致：");
            var r =
                DataTableTool.CreateReader(new System.IO.StreamReader(path, System.Text.Encoding.UTF8),
                new DataRow(),
                new DataExplainer());
            var cc = r.Get<HuMan>();
            Log.L("读取出的列表对象的内容为:\n");
            Log.L("Age\tSex\tName\theight\twidth");
            foreach (var c in cc)
            {
                Log.L($"{c.age}\t{c.sex}\t{c.Name}\t{c.heigth}\t{c.width}");
            }
            r.Dispose();

            Log.L("");
            Log.L("由于height标记了DataReadColumnIndex(0)这个Attribute所以读的是age列的数据");
            Log.L("width标记了DataIgnore所以没有被写入文件（即使文件里有也不会读），所以都是0");
        }




        protected override void Update()
        {

        }

        protected override void Stop()
        {

        }
    }
}
