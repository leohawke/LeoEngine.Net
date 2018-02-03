
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
    }
}
