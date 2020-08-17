namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    internal interface ILifeTimeObject
    {
        void Awake();
        void OnEnable();
        void OnDisable();
        void Update();
        void Destory();
        bool enable { get; set; }
        string name { get; set; }
    }

    public class LifeTimeObject : RecyclableObject, ILifeTimeObject
    {
        private bool _enable;
        public bool enable
        {
            get { return _enable; }
            set
            {
                if (recyled) return;

                if (_enable != value)
                    _enable = value;
                if (_enable)
                    OnEnable();
                else
                    OnDisable();
            }
        }
        private bool _binded;
        public bool binded { get { return _binded; } }
        public void BindEnv()
        {
            Framework.BindEnvUpdate(Update, this.env);
            _binded = true;
        }
        public void UnBindEnv()
        {
            if (_binded)
            {
                Framework.UnBindEnvUpdate(Update, this.env);
                _binded = false;
            }
        }

        protected override void OnAllocate()
        {
            base.OnAllocate();
            (this as ILifeTimeObject).Awake();
            (this as ILifeTimeObject).enable = true; 

        }

        protected override void OnRecyle() { }
        protected override void OnDataReset() { }


        protected virtual void Awake() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void Update() { }
        protected virtual void OnDestory() { }

        protected override void OnDispose() { }

        public void Destory()
        {
            (this as ILifeTimeObject).enable = false;
            OnDestory();
            ResetData();
            UnBindEnv();
            Recyle();
        }

        void ILifeTimeObject.Awake()
        {
            Awake();
        }
        void ILifeTimeObject.OnEnable()
        {
            OnEnable();
        }
        void ILifeTimeObject.OnDisable()
        {
            OnDisable();
        }
        void ILifeTimeObject.Update()
        {
            if (recyled) return;
            Update();
        }
       

    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
