
using System.Runtime.InteropServices;

namespace leo.Math
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
    public struct float4
    {
        public float x;
        public float y;
        public float z;
        public float w;

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

        public float a
        {
            get => w;
            set => w = value;
        }

        public float4(float x, float y, float z,float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public float4(float2 xy, float z,float w)
        {
            x = xy.x;
            y = xy.y;
            this.z = z;
            this.w = w;
        }

        public float4(float x, float2 yz,float w)
        {
            this.x = x;
            y = yz.x;
            z = yz.y;
            this.w = w;
        }

        public float4(float x,float y,float2 zw)
        {
            this.x = x;
            this.y = y;
            this.z = zw.x;
            this.w = zw.y;
        }

        public float4(float x,float3 yzw)
        {
            this.x = x;
            y = yzw.x;
            z = yzw.y;
            w = yzw.z;
        }

        public float4(float3 xyz, float w)
        {
            x = xyz.x;
            y = xyz.y;
            z = xyz.z;
            this.w = w;
        }

        #region swizzle
        public float3 zwx => new float3(z, w, x);

        public float3 wzy => new float3(w, z, y);

        public float3 xyz => new float3(x, y, z);

        #endregion
    }
}
