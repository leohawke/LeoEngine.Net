using leo.Asset;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace leo.Editor.Asset
{
    internal static class MaterialAssetExtension
    {
        public static void Save(this MaterialAsset material, string path)
        {
            using (var stream = File.OpenWrite(path))
            using (var sw = new StreamWriter(stream))
            {
                Save(material, sw);
            }
        }

        static void Save(MaterialAsset material,TextWriter writer)
        {
            //TODO 转换为LSL节点输出 ! Wait leo.Scheme
            writer.WriteLine("(material");

            writer.WriteLine($"\t(effect {material.EffectName})");

            foreach(var pair in material.BindValues)
            {
                writer.WriteLine($"\t({pair.Key} {pair.Value})");
            }

            writer.WriteLine(")");
        }
    }
}
