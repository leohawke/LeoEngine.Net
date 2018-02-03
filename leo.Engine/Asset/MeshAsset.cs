﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using leo.Platform.Render.Vertex;
using leo.Platform.Render;
namespace leo.Asset
{
    public class MeshAsset
    {
        public class SubMeshDescrption
        {
            public byte MaterialIndex { get; set; }
            [StructLayout(LayoutKind.Sequential)]
            public struct LodDescription
            {
                public uint VertexNum { get; set; }
                public uint VertexBase { get; set; }
                public uint IndexNum { get; set; }
                public uint IndexBase { get; set; }
            }

            List<LodDescription> _lodsDescription = new List<LodDescription>();
            public IEnumerable<LodDescription> LodsDescription => _lodsDescription;

            public void AddLodDesciption(LodDescription desc) => _lodsDescription.Add(desc);
        }

        List<Element> _vertexElements = new List<Element>();
        public IEnumerable<Element> VertexElements => _vertexElements;

        public EFormat IndexFormat { get; private set;}

        byte[] _indexStream;
        public ReadOnlySpan<byte> IndexStream => _indexStream;

        List<SubMeshDescrption> _subMeshs = new List<SubMeshDescrption>();
        public IEnumerable<SubMeshDescrption> SubMeshs => _subMeshs;

        List<ReadOnlySpan<byte>> _vertexStreams = new List<ReadOnlySpan<byte>>();
        public IEnumerable<ReadOnlySpan<byte>> VertexStreams => _vertexStreams;

        public void AddVertexStream(Element element, ReadOnlySpan<Byte> stream)
        {
            _vertexElements.Add(element);
            _vertexStreams.Add(stream);
        }

        public void SetIndexStream(EFormat format, ReadOnlySpan<Byte> stream)
        {
            _indexStream = stream.ToArray();
            IndexFormat = format;
        }

        public void AddSubMeshDescrption(SubMeshDescrption desc)
        {
            _subMeshs.Add(desc);
        }

        public uint VertexCount { get; set; }
        public uint IndexCount { get; set; }
    }
}
