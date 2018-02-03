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
    }
}
