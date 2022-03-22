using System;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace IFramework
{
    public struct Point3 : IEquatable<Point3> 
    {
        private static readonly Point3 zeroVector = new Point3(0f, 0f, 0f);
        private static readonly Point3 oneVector = new Point3(1f, 1f, 1f);
        private static readonly Point3 upVector = new Point3(0f, 1f, 0f);
        private static readonly Point3 downVector = new Point3(0f, -1f, 0f);
        private static readonly Point3 leftVector = new Point3(-1f, 0f, 0f);
        private static readonly Point3 rightVector = new Point3(1f, 0f, 0f);
        private static readonly Point3 forwardVector = new Point3(0f, 0f, 1f);
        private static readonly Point3 backVector = new Point3(0f, 0f, -1f);
        private static readonly Point3 positiveInfinityVector = new Point3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        private static readonly Point3 negativeInfinityVector = new Point3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public float x;
        public float y;
        public float z;

        public static Point3 zero
        {
            get
            {
                return Point3.zeroVector;
            }
        }
        public static Point3 one
        {
            get
            {
                return Point3.oneVector;
            }
        }
        public static Point3 forward
        {
            get
            {
                return Point3.forwardVector;
            }
        }
        public static Point3 back
        {
            get
            {
                return Point3.backVector;
            }
        }
        public static Point3 up
        {
            get
            {
                return Point3.upVector;
            }
        }
        public static Point3 down
        {
            get
            {
                return Point3.downVector;
            }
        }
        public static Point3 left
        {
            get
            {
                return Point3.leftVector;
            }
        }
        public static Point3 right
        {
            get
            {
                return Point3.rightVector;
            }
        }
        public static Point3 positiveInfinity
        {
            get
            {
                return Point3.positiveInfinityVector;
            }
        }
        public static Point3 negativeInfinity
        {
            get
            {
                return Point3.negativeInfinityVector;
            }
        }
        public Point3 normalized
        {
            get
            {
                return Point3.Normalize(this);
            }
        }
        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
            }
        }
        public float sqrMagnitude
        {
            get
            {
                return this.x * this.x + this.y * this.y + this.z * this.z;
            }
        }


        public Point3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Point3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0f;
        }

      

        public bool Equals(Point3 other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z);
        }
        public void Normalize()
        {
            float num = Point3.Magnitude(this);
            if (num > 1E-05f)
            {
                this /= num;
            }
            else
            {
                this = Point3.zero;
            }
        }
        public void Set(float newX, float newY, float newZ)
        {
            this.x = newX;
            this.y = newY;
            this.z = newZ;
        }
        public void Scale(Point3 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }
        public override bool Equals(object other)
        {
            return other is Point3 && this.Equals((Point3)other);
        }
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
        }
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", new object[]
            {
                this.x,
                this.y,
                this.z
            });
        }


        public static Point3 Lerp(Point3 a, Point3 b, float t)
        {
            t = t.Clamp01();
            return new Point3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }
        public static Point3 LerpUnclamped(Point3 a, Point3 b, float t)
        {
            return new Point3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }
        public static Point3 MoveTowards(Point3 current, Point3 target, float maxDistanceDelta)
        {
            Point3 a = target - current;
            float magnitude = a.magnitude;
            Point3 result;
            if (magnitude <= maxDistanceDelta || magnitude < 1.401298E-45f)
            {
                result = target;
            }
            else
            {
                result = current + a / magnitude * maxDistanceDelta;
            }
            return result;
        }
        public static float SqrMagnitude(Point3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }
        public static Point3 Scale(Point3 a, Point3 b)
        {
            return new Point3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Point3 Cross(Point3 lhs, Point3 rhs)
        {
            return new Point3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }
        public static Point3 Min(Point3 lhs, Point3 rhs)
        {
            return new Point3(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }
        public static Point3 Max(Point3 lhs, Point3 rhs)
        {
            return new Point3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }
        public static float Dot(Point3 lhs, Point3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }
        public static Point3 Reflect(Point3 inDirection, Point3 inNormal)
        {
            return -2f * Point3.Dot(inNormal, inDirection) * inNormal + inDirection;
        }
        public static Point3 Normalize(Point3 value)
        {
            float num = Point3.Magnitude(value);
            Point3 result;
            if (num > 1E-05f)
            {
                result = value / num;
            }
            else
            {
                result = Point3.zero;
            }
            return result;
        }
        public static float Distance(Point3 a, Point3 b)
        {
            Point3 vector = new Point3(a.x - b.x, a.y - b.y, a.z - b.z);
            return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }
        public static Point3 ClampMagnitude(Point3 vector, float maxLength)
        {
            Point3 result;
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                result = vector.normalized * maxLength;
            }
            else
            {
                result = vector;
            }
            return result;
        }
        public static float Magnitude(Point3 vector)
        {
            return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }


        public static Point3 operator +(Point3 a, Point3 b)
        {
            return new Point3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Point3 operator -(Point3 a, Point3 b)
        {
            return new Point3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static Point3 operator -(Point3 a)
        {
            return new Point3(-a.x, -a.y, -a.z);
        }
        public static Point3 operator *(Point3 a, float d)
        {
            return new Point3(a.x * d, a.y * d, a.z * d);
        }
        public static Point3 operator *(float d, Point3 a)
        {
            return new Point3(a.x * d, a.y * d, a.z * d);
        }
        public static Point3 operator /(Point3 a, float d)
        {
            return new Point3(a.x / d, a.y / d, a.z / d);
        }
        public static bool operator ==(Point3 lhs, Point3 rhs)
        {
            return Point3.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
        }
        public static bool operator !=(Point3 lhs, Point3 rhs)
        {
            return !(lhs == rhs);
        }


    }
}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
