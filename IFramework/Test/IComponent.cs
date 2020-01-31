using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFramework.Test
{
    public interface IComponentData
    {
        int index { get; set; }
    }
    public interface IExcuteSystem { }
    public class Enity
    {
        private List<IComponentData> components;
        public IComponentData GetComponent(int index)
        {
            return components.Find((com) => { return com.index == index; });
        }
    }
    public class Enitys
    {
        private List<Enity> _enitys;
        public void CreateEnity()
        { }
    }
    public class Systems
    {
        private List<IExcuteSystem> _sysyems;
    }
    public class World
    {
        private Systems systems;
        private Enitys _enitys;
    }
}
