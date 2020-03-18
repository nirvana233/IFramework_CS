using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace IFramework
{
    public struct Point2 : IEquatable<Point2>
    {
        private static readonly Point2 zeroVector = new Point2(0f, 0f);
        private static readonly Point2 oneVector = new Point2(1f, 1f);
        private static readonly Point2 upVector = new Point2(0f, 1f);
        private static readonly Point2 downVector = new Point2(0f, -1f);
        private static readonly Point2 leftVector = new Point2(-1f, 0f);
        private static readonly Point2 rightVector = new Point2(1f, 0f);
        private static readonly Point2 positiveInfinityVector = new Point2(float.PositiveInfinity, float.PositiveInfinity);
        private static readonly Point2 negativeInfinityVector = new Point2(float.NegativeInfinity, float.NegativeInfinity);

        public float x;
        public float y;

        public static Point2 zero
        {
            get
            {
                return Point2.zeroVector;
            }
        }
        public static Point2 one
        {
            get
            {
                return Point2.oneVector;
            }
        }
        public static Point2 up
        {
            get
            {
                return Point2.upVector;
            }
        }
        public static Point2 down
        {
            get
            {
                return Point2.downVector;
            }
        }
        public static Point2 left
        {
            get
            {
                return Point2.leftVector;
            }
        }
        public static Point2 right
        {
            get
            {
                return Point2.rightVector;
            }
        }
        public static Point2 positiveInfinity
        {
            get
            {
                return Point2.positiveInfinityVector;
            }
        }
        public static Point2 negativeInfinity
        {
            get
            {
                return Point2.negativeInfinityVector;
            }
        }
        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(this.x * this.x + this.y * this.y);
            }
        }
        public float sqrMagnitude
        {
            get
            {
                return this.x * this.x + this.y * this.y;
            }
        }



        public Point2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(float newX, float newY)
        {
            this.x = newX;
            this.y = newY;
        }
        public void Scale(Point2 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }
        public void Normalize()
        {
            float magnitude = this.magnitude;
            if (magnitude > 1E-05f)
            {
                this /= magnitude;
            }
            else
            {
                this = Point2.zero;
            }
        }
        public Point2 normalized
        {
            get
            {
                Point2 result = new Point2(this.x, this.y);
                result.Normalize();
                return result;
            }
        }
        public bool Equals(Point2 other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y);
        }
        public float SqrMagnitude()
        {
            return this.x * this.x + this.y * this.y;
        }


        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1})", new object[]
            {
                this.x,
                this.y
            });
        }
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
        }
        public override bool Equals(object other)
        {
            return other is Point2 && this.Equals((Point2)other);
        }


        public static Point2 Lerp(Point2 a, Point2 b, float t)
        {
            t = t.Clamp01();
            return new Point2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }
        public static Point2 LerpUnclamped(Point2 a, Point2 b, float t)
        {
            return new Point2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }
        public static Point2 MoveTowards(Point2 current, Point2 target, float maxDistanceDelta)
        {
            Point2 a = target - current;
            float magnitude = a.magnitude;
            Point2 result;
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                result = target;
            }
            else
            {
                result = current + a / magnitude * maxDistanceDelta;
            }
            return result;
        }
        public static Point2 Scale(Point2 a, Point2 b)
        {
            return new Point2(a.x * b.x, a.y * b.y);
        }
        public static Point2 Reflect(Point2 inDirection, Point2 inNormal)
        {
            return -2f * Point2.Dot(inNormal, inDirection) * inNormal + inDirection;
        }
        public static Point2 Perpendicular(Point2 inDirection)
        {
            return new Point2(-inDirection.y, inDirection.x);
        }
        public static float Dot(Point2 lhs, Point2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }
        public static float Distance(Point2 a, Point2 b)
        {
            return (a - b).magnitude;
        }
        public static Point2 ClampMagnitude(Point2 vector, float maxLength)
        {
            Point2 result;
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
        public static float SqrMagnitude(Point2 a)
        {
            return a.x * a.x + a.y * a.y;
        }
        public static Point2 Min(Point2 lhs, Point2 rhs)
        {
            return new Point2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }
        public static Point2 Max(Point2 lhs, Point2 rhs)
        {
            return new Point2(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }





        public static Point2 operator +(Point2 a, Point2 b)
        {
            return new Point2(a.x + b.x, a.y + b.y);
        }
        public static Point2 operator -(Point2 a, Point2 b)
        {
            return new Point2(a.x - b.x, a.y - b.y);
        }
        public static Point2 operator *(Point2 a, Point2 b)
        {
            return new Point2(a.x * b.x, a.y * b.y);
        }
        public static Point2 operator /(Point2 a, Point2 b)
        {
            return new Point2(a.x / b.x, a.y / b.y);
        }
        public static Point2 operator -(Point2 a)
        {
            return new Point2(-a.x, -a.y);
        }
        public static Point2 operator *(Point2 a, float d)
        {
            return new Point2(a.x * d, a.y * d);
        }
        public static Point2 operator *(float d, Point2 a)
        {
            return new Point2(a.x * d, a.y * d);
        }
        public static Point2 operator /(Point2 a, float d)
        {
            return new Point2(a.x / d, a.y / d);
        }
        public static bool operator ==(Point2 lhs, Point2 rhs)
        {
            return (lhs - rhs).sqrMagnitude < 9.99999944E-11f;
        }
        public static bool operator !=(Point2 lhs, Point2 rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator Point2(Point3 p3)
        {
            return new Point2(p3.x, p3.y);
        }
        public static implicit operator Point3(Point2 p2)
        {
            return new Point3(p2.x, p2.y,0);
        }
    }
}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
