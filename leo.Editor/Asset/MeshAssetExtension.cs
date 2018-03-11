using leo.Asset;
using System;
using System.IO;
using System.Linq;

namespace leo.Editor.Asset
{
    internal static class MeshAssetExtension
    {
        struct MeshHeader
        {
            public uint Signature;
            public ushort Machine;
            public ushort NumberOfSections;
            public ushort SizeOfOptional;
        }

        struct SectionCommonHeader
        {
            public ushort SectionIndex;
            public uint NextSectionOffset;
            public byte[] Name;
            public uint Size;
        }

        struct GeomertySectionHeader
        {
            public uint FourCC;
            public uint Version;
            public uint CompressVersion;
            public ushort SizeOfOptional;
        }


        public static void Save(this MeshAsset mesh, string path)
        {
            using (var stream = File.OpenWrite(path))
            using (var sw = new BinaryWriter(stream))
            {
                var header = mesh.WriterHeader(sw);
                var geomerty_header = mesh.WriterGeomerty(sw);
            }
        }

        static MeshHeader WriterHeader(this MeshAsset mesh, BinaryWriter sw)
        {
            MeshHeader header = new MeshHeader();
            header.Signature = FourCC.Value('M', 'E', 'S', 'H');
            header.Machine = 0;
            header.NumberOfSections = 1;
            header.SizeOfOptional = 0;

            sw.Write(header.Signature);
            sw.Write(header.Machine);
            sw.Write(header.NumberOfSections);
            sw.Write(header.SizeOfOptional);
            return header;
        }

        static Tuple<SectionCommonHeader,GeomertySectionHeader> WriterGeomerty(this MeshAsset mesh, BinaryWriter sw)
        {
            SectionCommonHeader common_header = new SectionCommonHeader();
            GeomertySectionHeader geometry_header = new GeomertySectionHeader();

            var common_offset = sw.Seek(0, SeekOrigin.Current);

            common_header.SectionIndex = 0;
            common_header.NextSectionOffset = uint.MaxValue;
            common_header.Name = new byte[] {
                (byte)'G',(byte)'E',(byte)'O',(byte)'M',
                (byte)'E',(byte)'R',(byte)'T',(byte)'Y',
            };
            common_header.Size = uint.MaxValue;

            //先写入用于计算大小
            sw.Write(common_header.SectionIndex);
            sw.Write(common_header.NextSectionOffset);
            sw.Write(common_header.Name);
            sw.Write(common_header.Size);

            geometry_header.FourCC = FourCC.Value('L', 'E', 'M', 'E');
            geometry_header.Version = 0;
            geometry_header.CompressVersion = 0;
            geometry_header.SizeOfOptional = 0;

            sw.Write(geometry_header.FourCC);
            sw.Write(geometry_header.Version);
            sw.Write(geometry_header.CompressVersion);
            sw.Write(geometry_header.SizeOfOptional);

            //取得头写入完后的Offset
            var data_offset = sw.Seek(0, SeekOrigin.Current);
            var data_size = mesh.WrtieGeometryData(sw);

            //Size包含头部大小
            common_header.Size =(uint)(data_offset - common_offset + data_size);
            //相对Origin的偏移
            common_header.NextSectionOffset = (uint)(data_offset + data_size);

            //重新写入头
            sw.Seek((int)common_offset, SeekOrigin.Begin);
            sw.Write(common_header.SectionIndex);
            sw.Write(common_header.NextSectionOffset);
            sw.Write(common_header.Name);
            sw.Write(common_header.Size);

            //跳到下一个Section
            sw.Seek((int)common_header.NextSectionOffset, SeekOrigin.Begin);

            return Tuple.Create(common_header, geometry_header);
        }

        static long WrtieGeometryData(this MeshAsset mesh,BinaryWriter sw)
        {
            var data_offset = sw.Seek(0, SeekOrigin.Current);

            var vertex_elements = mesh.VertexElements;
            var VertexElmentsCount = vertex_elements.Count();

            sw.Write((byte)VertexElmentsCount);

            foreach(var vertex_element in vertex_elements)
            {
                sw.Write((byte)vertex_element.Usage);
                sw.Write(vertex_element.UsageIndex);
                sw.Write((ulong)vertex_element.Format);
            }

            sw.Write((ulong)mesh.IndexFormat);

            if(mesh.IndexFormat == Platform.Render.EFormat.EF_R16UI)
            {
                sw.Write((ushort)mesh.VertexCount);
                sw.Write((ushort)mesh.IndexCount);
            }
            else
            {
                sw.Write(mesh.VertexCount);
                sw.Write(mesh.IndexCount);
            }

            foreach(var vertex_stream in mesh.VertexStreams)
            {
                sw.Write(vertex_stream);
            }

            sw.Write(mesh.IndexStream.ToArray());

            var submesh_count = mesh.SubMeshs.Count();
            sw.Write((byte)submesh_count);
            foreach (var submesh in mesh.SubMeshs)
            {
                sw.Write(submesh.MaterialIndex);
                sw.Write((byte)submesh.LodsDescription.Count());

                var float3 = new byte[12];
                var float2 = new byte[8];
                sw.Write(float3);//pc
                sw.Write(float3);//po
                sw.Write(float2);//tc
                sw.Write(float2);//to

                foreach (var lod_desc in submesh.LodsDescription)
                {
                    if (mesh.IndexFormat == Platform.Render.EFormat.EF_R16UI)
                    {
                        sw.Write((ushort)lod_desc.VertexNum);
                        sw.Write((ushort)lod_desc.VertexBase);
                        sw.Write((ushort)lod_desc.IndexNum);
                        sw.Write((ushort)lod_desc.IndexBase);
                    }
                    else
                    {
                        sw.Write(lod_desc.VertexNum);
                        sw.Write(lod_desc.VertexBase);
                        sw.Write(lod_desc.IndexNum);
                        sw.Write(lod_desc.IndexBase);
                    }
                }
            }
           
            return sw.Seek(0, SeekOrigin.Current) - data_offset;
        }
    }
}
