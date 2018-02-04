
namespace leo.Asset
{
    public class FourCC
    {
        public static uint Value(char c1, char c2, char c3, char c4)
        {
            return (uint)(c1 << 0) + (uint)(c2 << 8) + (uint)(c3 << 16) + (uint)(c4 << 24);
        }
    }
}
