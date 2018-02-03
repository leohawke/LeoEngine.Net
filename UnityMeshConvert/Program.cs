using platform.Editor.Asset;
using System;
using System.IO;
using UnityMeshConvert;

namespace leo.Tools
{
    class UnityMeshConvertProgram
    {
        static int Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Error:No Input File\n Example:UnityMeshConvert xxx.json");
                return -1;
            }

            try
            {
                IUnityMeshConvert convert = new UnityMeshJsonConvert();
                var mesh = convert.Convert(args[0]);
                X.SaveMeshAsset(Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileName(args[0]) + ".asset"), mesh);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: Exception:{e}");
                return -2;
            }
            return 0;
        }
    }
}
