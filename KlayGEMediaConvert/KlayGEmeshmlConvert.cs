using leo.Asset;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System;
using System.IO;
using platform.Editor.Asset;
using System.Linq;
using leo.Math;
using leo.Platform.Render;

namespace KlayGEMediaConvert
{
    public class KlayGEmeshmlConvert
    {
        public static float3 cross(float3 l,float3 r)
        {
            return new float3(
                l.y * r.z - l.z * r.y,
                l.z * r.x - l.x * r.z,
                l.x * r.y - l.y * r.x);
        }

        public static float3 transform_quat(float3 v,float4 quat)
        {
            return v + cross(quat.xyz, cross(quat.xyz, v) + quat.w * v) * 2;
        }

        public void Convert(string path)
        {

            var KlayGE_meshml = XDocument.Load(path);

            var dir = Path.GetDirectoryName(path);

            if (KlayGE_meshml.Root.Attribute("version").Value != "6")
                throw new NotSupportedException();

            //material pass output
            /*
             * (material
             *  (effect Shading)
             *  (albedo (float3))
             *  (albedo_tex)~macro
             *  (smoothness)
             *  (metaless)
             *  (normal)
             * )
             */

            var materials = new List<Tuple<string, MaterialAsset>>();

            foreach (var material_node in KlayGE_meshml.XPathSelectElements("//material"))
            {
                var material = new MaterialAsset();

                var material_name = materials.Count.ToString();

                material.EffectName = "ForwardPointLightDiffuseShading";

                var albedo_node = material_node.Element("albedo");

                {
                    var albedo_float4 = albedo_node?.Attribute("color");

                    if (albedo_float4 != null)
                    {
                        var albedo = albedo_float4.Value;
                        var albedo_array = albedo.Substring(0, albedo.LastIndexOf(' ')).Split(' ').Select(component=>component+'f');
                        material["albedo"] = $"(float3 {string.Join(' ',albedo_array)})";
                    }

                    var albedo_texture = albedo_node?.Attribute("texture");
                    if (albedo_texture != null)
                    {
                        var albedo_tex = albedo_texture.Value;
                        material["albedo_tex"] = $"\"{albedo_tex}\"";

                        material_name = $"{material_name}_{Path.GetFileNameWithoutExtension(albedo_tex)}";
                    }
                }

                var metalness_node = material_node.Element("metalness");
                {
                    var metalness = metalness_node?.Attribute("value");
                    if (metalness != null)
                        material["metalness"] = $"(float2 {metalness.Value}f 0)";
                }

                var glossiness_node = material_node.Element("glossiness");
                {
                    var glossiness = glossiness_node?.Attribute("value");
                    var glossiness_texture = glossiness_node?.Attribute("texture");
                    if (glossiness != null)
                    {
                        var value = double.Parse(glossiness.Value);
                        material["glossiness"] = $"(float2 {value}f {(glossiness_texture != null ? 1 : 0)})";
                    }
                    if (glossiness_texture != null)
                    {
                        var glossiness_tex = glossiness_texture.Value;
                        material["glossiness_tex"] = $"\"{glossiness_tex}\"";
                        material_name = $"{material_name}_{Path.GetFileNameWithoutExtension(glossiness_tex)}";
                    }
                }

                //todo support normal map

                var material_tuple = Tuple.Create(material_name, material);

                materials.Add(material_tuple);

                var material_file = material_name + ".mat.lsl";

                X.SaveMaterialAsset(Path.Combine(dir, material_file), material);
            }

            var meshes = new List<Tuple<string, string>>();

            foreach (var mesh_node in KlayGE_meshml.XPathSelectElements("//mesh"))
            {
                var mesh = new MeshAsset();

                var vertices_node = mesh_node.XPathSelectElements("vertices_chunk/vertex");

                var vertices = vertices_node.Select(vertex_node =>
                {
                    var pos_array = vertex_node.Attribute("v").Value.Split(' ').Select(component => float.Parse(component)).ToArray();
                    var pos = new float3(pos_array[0], pos_array[1], pos_array[2]);

                    var tangent_quat_array = vertex_node.Element("tangent_quat").Attribute("v").Value.Split(' ').Select(component => float.Parse(component)).ToArray();
                    var tangnet_quat = new float4(tangent_quat_array[0], tangent_quat_array[1], tangent_quat_array[2], tangent_quat_array[3]);

                    var tangent = transform_quat(new float3(1, 0, 0), tangnet_quat);
                    var binormal = transform_quat(new float3(0, 1, 0), tangnet_quat) * (tangnet_quat.w<0?-1.0f:1.0f);
                    var normal = transform_quat(new float3(0, 0, 1), tangnet_quat);
                    
                    var tex_coord_array = vertex_node.Element("tex_coord").Attribute("v").Value.Split(' ').Select(component => float.Parse(component)).ToArray();
                    var tex_coord = new float2(tex_coord_array[0], tex_coord_array[1]);
                    return Tuple.Create(pos, normal, tex_coord);
                }).ToList();

                using (var stream = new MemoryStream())
                using (var bw = new BinaryWriter(stream))
                {
                    foreach (var vertex in vertices)
                    {
                        var pos = vertex.Item1;
                        bw.Write(pos.x);
                        bw.Write(pos.y);
                        bw.Write(pos.z);
                    }
                    bw.Flush();

                    var pos_element = new leo.Platform.Render.Vertex.Element();
                    pos_element.Format = EFormat.EF_BGR32F;
                    pos_element.Usage = leo.Platform.Render.Vertex.Usage.Position;
                    pos_element.UsageIndex = 0;

                    mesh.AddVertexStream(pos_element, stream.ToArray());
                }

                using (var stream = new MemoryStream())
                using (var bw = new BinaryWriter(stream))
                {
                    foreach (var vertex in vertices)
                    {
                        var normal = vertex.Item2;
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

                using (var stream = new MemoryStream())
                using (var bw = new BinaryWriter(stream))
                {
                    foreach (var vertex in vertices)
                    {
                        var tex_coord = vertex.Item3;
                        bw.Write(tex_coord.x);
                        bw.Write(tex_coord.y);
                    }
                    bw.Flush();

                    var texcoord_element = new leo.Platform.Render.Vertex.Element();
                    texcoord_element.Format = EFormat.EF_GR32F;
                    texcoord_element.Usage = leo.Platform.Render.Vertex.Usage.TextureCoord;
                    texcoord_element.UsageIndex = 0;

                    mesh.AddVertexStream(texcoord_element, stream.ToArray());
                }

                var triangles_node = mesh_node.XPathSelectElements("triangles_chunk/triangle");

                var triangles = triangles_node.Select(triangle_node =>
                        triangle_node.Attribute("index").Value.Split(' ')
                        .Select(index => uint.Parse(index)))
                        .SelectMany(triangle => triangle)
                        .ToList();

                var index_format = EFormat.EF_R16UI;
                if (triangles.Count > short.MaxValue)
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

                mesh.IndexCount = (uint)triangles.Count;
                mesh.VertexCount = (uint)vertices.Count;

                var submesh = new MeshAsset.SubMeshDescrption()
                {
                    MaterialIndex = 0,
                };
                submesh.AddLodDesciption(new MeshAsset.SubMeshDescrption.LodDescription()
                {
                    IndexBase = 0,
                    IndexNum = mesh.IndexCount,
                    VertexBase = 0,
                    VertexNum = mesh.VertexCount
                });

                mesh.AddSubMeshDescrption(submesh);

                var mesh_name = mesh_node.Attribute("name").Value;

                X.SaveMeshAsset(Path.Combine(dir, mesh_name + ".asset"), mesh);

                var mtl_id = int.Parse(mesh_node.Attribute("mtl_id").Value);
                var material_name = materials[mtl_id].Item1;

                var mesh_tuple = Tuple.Create(mesh_name, material_name);

                meshes.Add(mesh_tuple);
            }

            var filename = Path.Combine(dir, Path.GetFileNameWithoutExtension(path) + ".entities.lsl");
            using (var stream = File.OpenWrite(filename))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("(entities");
                foreach (var mesh_tuple in meshes)
                    writer.WriteLine($"\t(entity (mesh {mesh_tuple.Item1}) (material {mesh_tuple.Item2}))");
                writer.WriteLine(")");
            }
        }
    }
}
