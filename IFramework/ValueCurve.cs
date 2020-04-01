using System.Collections.Generic;

namespace IFramework
{
    /// <summary>
    /// 用于取值的曲线
    /// </summary>
    public class ValueCurve
    {
        private List<Point2> _points;
        private float _step = 0.002f;
        private List<Point2> _steps;
        /// <summary>
        /// 点的总个数
        /// </summary>
        public int count { get { return _steps.Count; } }
        /// <summary>
        /// 步长
        /// </summary>
        public float step { get { return _step; } set { _step = value; ClacSteps(); } }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="points"></param>
        /// <param name="step"></param>
        public ValueCurve(List<Point2> points, float step = 0.002f)
        {
            this._points = points;
            this._step = step;
            _steps = new List<Point2>();
            ClacSteps();
        }
        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(Point2 point)
        {
            _points.Add(point);
            ClacSteps();
        }
        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="point"></param>
        public void RemovePoint(Point2 point)
        {
            var count = _points.RemoveAll((p) => { return point.Equals(p); });
            if (count > 0)
                ClacSteps();
        }

        /// <summary>
        /// 获取一步
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Point2 GetStep(int step)
        {
            step = step.Clamp(0, count-1);
            return _steps[step];
        }
        /// <summary>
        /// 按照百分比获取步
        /// </summary>
        /// <param name="mul"></param>
        /// <returns></returns>
        public Point2 GetPercent(float mul)
        {
            mul = mul.Clamp01();
            mul = count * mul;
            return GetStep((int)mul);
        }
        /// <summary>
        /// 通过x获取y
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float GetYWithX(float x)
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                Point2 v = _steps[i];
                if (v.x >= x - step && v.x <= x + step)
                {
                    return v.y;
                }
            }
            Log.E(x);
            return -1;
        }
        /// <summary>
        /// 计算每一步
        /// </summary>
        public void ClacSteps()
        {
            _steps.Clear();
            _steps = new List<Point2>();
            float val = 0f;
            do
            {
                _steps.Add(CalcBezierPoint(val, _points));
                val += _step;
            } while (val <= 1);
        }

        private static Point2 CalcBezierPoint(float delta, List<Point2> _points)
        {
            while (_points.Count > 1)
            {
                List<Point2> newp = new List<Point2>();
                for (int i = 0; i < _points.Count - 1; i++)
                {
                    Point2 p0p1 = Point2.Lerp(_points[i], _points[i + 1], delta);
                    newp.Add(p0p1);
                }
                _points = newp;
            }
            return _points[0];
        }

        /// <summary>
        /// 0，0 -- 1，1 对称往上拱起
        /// </summary>
        public static ValueCurve ccurve { get { return _ccurve; } }
        private static ValueCurve _ccurve = new ValueCurve(new List<Point2>()
                {
                    new  Point2(0,0),
                    new  Point2(0.3f,0.6f),

                    new Point2(0.5f,0.7f),
                    new Point2(0.7f,0.9f),
                    new Point2(1,1)
                });
        /// <summary>
        ///  0，0 -- 1，1 s形状
        /// </summary>
        public static ValueCurve scurve { get { return _scurve; } }
        private static ValueCurve _scurve = new ValueCurve(new List<Point2>()
                {
                    new  Point2(0,0),
                    new  Point2(0.3f,0.7f),

                    new Point2(0.5f,0.5f),
                    new Point2(0.7f,0.3f),
                    new Point2(1,1)
                });

        /// <summary>
        ///  0，0 -- 1，1 直线
        /// </summary>
        public static ValueCurve linecurve { get { return _linecuve; } }
        private static ValueCurve _linecuve = new ValueCurve(new List<Point2>()
        {
            new  Point2(0,0),
            new Point2(0.5f,0.5f),
            new Point2(1,1)
        });


        /// <summary>
        /// 0，0 -- 1，1 对称往上拱起(粗糙)
        /// </summary>
        public static ValueCurve ccurve_rough { get { return _ccurve_rough; } }
        private static ValueCurve _ccurve_rough = new ValueCurve(new List<Point2>()
                {
                    new  Point2(0,0),
                    new  Point2(0.3f,0.6f),

                    new Point2(0.5f,0.7f),
                    new Point2(0.7f,0.9f),
                    new Point2(1,1)
                },0.02f);
        /// <summary>
        ///  0，0 -- 1，1 s形状(粗糙)
        /// </summary>
        public static ValueCurve scurve_rough { get { return _scurve_rough; } }
        private static ValueCurve _scurve_rough = new ValueCurve(new List<Point2>()
                {
                    new  Point2(0,0),
                    new  Point2(0.3f,0.7f),

                    new Point2(0.5f,0.5f),
                    new Point2(0.7f,0.3f),
                    new Point2(1,1)
                },0.02f);
        /// <summary>
        ///  0，0 -- 1，1 直线(粗糙)
        /// </summary>
        public static ValueCurve linecurve_rough { get { return _linecuve_rough; } }
        private static ValueCurve _linecuve_rough = new ValueCurve(new List<Point2>()
        {
            new  Point2(0,0),
            new Point2(0.5f,0.5f),
            new Point2(1,1)
        },0.02f);
    }

}
