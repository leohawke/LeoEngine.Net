using leo.Asset;
using leo.Editor.Asset;

namespace platform.Editor.Asset
{
    public class X
    {
        public static void SaveMeshAsset(string path,MeshAsset mesh)
        {
            mesh.Save(path);
        }

        public static void SaveMaterialAsset(string path,MaterialAsset material)
        {
            material.Save(path);
        }
    }
}
