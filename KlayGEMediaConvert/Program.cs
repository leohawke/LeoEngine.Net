using System;

namespace KlayGEMediaConvert
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error:No Input File\n Example:KlayGEMediaConvert xxx.meshml");
                return -1;
            }

            try
            {
                var convert = new KlayGEmeshmlConvert();
               convert.Convert(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: Exception:{e}");
                return -2;
            }
            return 0;
        }
    }
}
