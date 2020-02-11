
using System.Runtime.InteropServices;

namespace leo.Math
{
    [StructLayout(LayoutKind.Sequential,Pack =4,Size =16)]
    public struct float3
    {
        public float x;
        public float y;
        public float z;

        public float u
        {
            get => x;
            set => x = value;
        }
        public float v
        {
            get => y;
            set => y = value;
        }

        public float w
        {
            get => z;
            set => z = value;
        }

        public float r
        {
            get => x;
            set => x = value;
        }
        public float g
        {
            get => y;
            set => y = value;
        }

        public float b
        {
            get => z;
            set => z = value;
        }

        public float3(float x, float y,float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float3(float2 xy,float z)
        {
            x = xy.x;
            y = xy.y;
            this.z = z;
        }

        public float3(float x, float2 yz)
        {
            this.x = x;
            y = yz.x;
            z = yz.y;
        }

        public static float3 operator*(float3 lhs,float3 rhs)
        {
            return new float3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }

        public static float3 operator *(float3 lhs, float rhs)
        {
            return new float3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }

        public static float3 operator *(float rhs,float3 lhs)
        {
            return new float3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }

        public static float3 operator +(float3 lhs, float3 rhs)
        {
            return new float3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        public static float3 operator -(float3 lhs, float3 rhs)
        {
            return new float3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static float3 operator -(float3 rhs)
        {
            return new float3(- rhs.x, rhs.y,- rhs.z);
        }
    }
}
