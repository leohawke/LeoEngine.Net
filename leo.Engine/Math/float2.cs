
using System.Runtime.InteropServices;

namespace leo.Math
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
    public struct float2
    {
        public float x;
        public float y;

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

        public float2(float x,float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
