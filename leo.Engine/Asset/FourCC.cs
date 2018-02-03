
namespace leo.Asset
{
    public class FourCC
    {
        public static uint Value(char c1, char c2, char c3, char ce)
        {
            return (uint)(c1 << 0) + (uint)(c1 << 8) + (uint)(c2 << 16) + (uint)(c3 << 24);
        }
    }
}
