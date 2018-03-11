using leo.Asset;
using Newtonsoft.Json.Linq;
using System.IO;
using leo.Platform.Render;
using leo.Math;

namespace UnityMeshConvert
{
    public class UnityMeshJsonConvert : IUnityMeshConvert
    {
        MeshAsset IUnityMeshConvert.Convert(string path)
        {
            var mesh = new MeshAsset();

            var unity_mesh = JObject.Parse(File.ReadAllText(path));

            var vertices = ((JArray)unity_mesh["vertices"]).ToObject<float3[]>();
            using (var stream = new MemoryStream())
            using (var bw = new BinaryWriter(stream))
            {
                foreach (var vertex in vertices)
                {
                    bw.Write(vertex.x);
                    bw.Write(vertex.y);
                    bw.Write(vertex.z);
                }
                bw.Flush();

                var pos_element = new leo.Platform.Render.Vertex.Element();
                pos_element.Format = EFormat.EF_BGR32F;
                pos_element.Usage = leo.Platform.Render.Vertex.Usage.Position;
                pos_element.UsageIndex = 0;

                mesh.AddVertexStream(pos_element, stream.ToArray());
            }
            var normals = ((JArray)unity_mesh["normals"]).ToObject<float3[]>();
            using (var stream = new MemoryStream())
            using (var bw = new BinaryWriter(stream))
            {
                foreach (var normal in normals)
                {
                    bw.Write(normal.x);
                    bw.Write(normal.y);
                    bw.Write(normal.z);
                }
                bw.Flush();

                var normal_element = new leo.Platform.Render.Vertex.Element();
                normal_element.Format = EFormat.EF_BGR32F;
                normal_element.Usage = leo.Platform.Render.Vertex.Usage.Normal;
                normal_element.UsageIndex = 0;

                mesh.AddVertexStream(normal_element, stream.ToArray());
            }

            var triangles = ((JArray)unity_mesh["triangles"]).ToObject<uint[]>();
            var index_format = EFormat.EF_R16UI;
            if (triangles.Length > short.MaxValue)
                index_format = EFormat.EF_R32UI;
            var use_16 = index_format == EFormat.EF_R16UI;
            using (var stream = new MemoryStream())
            using (var bw = new BinaryWriter(stream))
            {
                foreach (var index in triangles)
                    if (use_16)
                        bw.Write((ushort)index);
                    else
                        bw.Write(index);
                bw.Flush();

                mesh.SetIndexStream(index_format, stream.ToArray());
            }

            mesh.IndexCount = (uint)triangles.Length;
            mesh.VertexCount = (uint)vertices.Length;

            var submesh = new MeshAsset.SubMeshDescrption()
            {
                MaterialIndex = 0,
            };
            submesh.AddLodDesciption(new MeshAsset.SubMeshDescrption.LodDescription() {
                IndexBase = 0,
                IndexNum = mesh.IndexCount,
                VertexBase = 0,
                VertexNum = mesh.VertexCount
            });

            mesh.AddSubMeshDescrption(submesh);

            return mesh;
        }
    }
}
