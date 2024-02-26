// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using System.Collections.Generic;

namespace Kaitai
{
    public partial class Flh : KaitaiStruct
    {
        public static Flh FromFile(string fileName)
        {
            return new Flh(new KaitaiStream(fileName));
        }

        public Flh(KaitaiStream p__io, KaitaiStruct p__parent = null, Flh p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _lw1 = m_io.ReadU4le();
            _framecount = m_io.ReadU2le();
            _maxwidth = m_io.ReadU2le();
            _maxheight = m_io.ReadU2le();
            _padding = m_io.ReadU2le();
            _frames = new List<Anim>();
            for (var i = 0; i < Framecount; i++)
            {
                _frames.Add(new Anim(m_io, this, m_root));
            }
        }
        public partial class Anim : KaitaiStruct
        {
            public static Anim FromFile(string fileName)
            {
                return new Anim(new KaitaiStream(fileName));
            }

            public Anim(KaitaiStream p__io, Flh p__parent = null, Flh p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _x1 = m_io.ReadU2le();
                _y1 = m_io.ReadU2le();
                _x2 = m_io.ReadU2le();
                _y2 = m_io.ReadU2le();
                _zero = m_io.ReadU4le();
                _f = m_io.ReadU4le();
                _name = System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(16));
                _flags = m_io.ReadU4le();
                _offset = m_io.ReadU4le();
            }
            private ushort _x1;
            private ushort _y1;
            private ushort _x2;
            private ushort _y2;
            private uint _zero;
            private uint _f;
            private string _name;
            private uint _flags;
            private uint _offset;
            private Flh m_root;
            private Flh m_parent;
            public ushort X1 { get { return _x1; } }
            public ushort Y1 { get { return _y1; } }
            public ushort X2 { get { return _x2; } }
            public ushort Y2 { get { return _y2; } }
            public uint Zero { get { return _zero; } }
            public uint F { get { return _f; } }
            public string Name { get { return _name; } }
            public uint Flags { get { return _flags; } }
            public uint Offset { get { return _offset; } }
            public Flh M_Root { get { return m_root; } }
            public Flh M_Parent { get { return m_parent; } }
        }
        private uint _lw1;
        private ushort _framecount;
        private ushort _maxwidth;
        private ushort _maxheight;
        private ushort _padding;
        private List<Anim> _frames;
        private Flh m_root;
        private KaitaiStruct m_parent;
        public uint Lw1 { get { return _lw1; } }
        public ushort Framecount { get { return _framecount; } }
        public ushort Maxwidth { get { return _maxwidth; } }
        public ushort Maxheight { get { return _maxheight; } }
        public ushort Padding { get { return _padding; } }
        public List<Anim> Frames { get { return _frames; } }
        public Flh M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
