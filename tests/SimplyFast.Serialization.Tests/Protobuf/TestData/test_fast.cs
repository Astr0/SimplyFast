//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// Generated from: test.proto


namespace SimplyFast.Serialization.Tests.Protobuf.TestData
{
    public partial class FTestMessage: IMessage
    {
        // Fields
        private static readonly RepeatedType<InnerMessage> _tinyRepeatedTypeTestroot_Classes_InnerMessage = RepeatedTypes.Message<InnerMessage>();
        private static readonly RepeatedType<SomeEnum> _tinyRepeatedTypeTestroot_Classes_TestEnum = RepeatedTypes.Enum<SomeEnum>(x => (SomeEnum)x, x => (int)x);
        private double _fdouble;
        private float _ffloat;
        private int _fint32;
        private long? _fint64;
        private uint? _fuint32;
        private ulong? _fuint64;
        private int? _fsint32;
        private long? _fsint64;
        private uint? _ffixed32;
        private ulong? _ffixed64;
        private int? _fsfixed32;
        private long? _fsfixed64;
        private bool? _fbool;
        private string _fstring;
        private byte[] _fbytes;
        private SomeEnum? _fenum;
        private InnerMessage _finner;
        private global::System.Collections.Generic.List<InnerMessage> _frep;
        private global::System.Collections.Generic.List<SomeEnum> _frepEnum;
        private global::System.Collections.Generic.List<string> _frepString;
        private global::System.Collections.Generic.List<uint> _frepFixed32;
        private global::System.Collections.Generic.List<uint> _frepUint32;

        // Properties
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public double Fdouble
        {
            get { return _fdouble; }
            set { _fdouble = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public float Ffloat
        {
            get { return _ffloat; }
            set { _ffloat = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int Fint32
        {
            get { return _fint32; }
            set { _fint32 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public long? Fint64
        {
            get { return _fint64; }
            set { _fint64 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public uint? Fuint32
        {
            get { return _fuint32; }
            set { _fuint32 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public ulong? Fuint64
        {
            get { return _fuint64; }
            set { _fuint64 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int? Fsint32
        {
            get { return _fsint32; }
            set { _fsint32 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public long? Fsint64
        {
            get { return _fsint64; }
            set { _fsint64 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public uint? Ffixed32
        {
            get { return _ffixed32; }
            set { _ffixed32 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public ulong? Ffixed64
        {
            get { return _ffixed64; }
            set { _ffixed64 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int? Fsfixed32
        {
            get { return _fsfixed32; }
            set { _fsfixed32 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public long? Fsfixed64
        {
            get { return _fsfixed64; }
            set { _fsfixed64 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool? Fbool
        {
            get { return _fbool; }
            set { _fbool = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string Fstring
        {
            get { return _fstring; }
            set { _fstring = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public byte[] Fbytes
        {
            get { return _fbytes; }
            set { _fbytes = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public SomeEnum? Fenum
        {
            get { return _fenum; }
            set { _fenum = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public InnerMessage Finner
        {
            get { return _finner; }
            set { _finner = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::System.Collections.Generic.List<InnerMessage> Frep
        {
            get { return _frep; }
            set { _frep = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::System.Collections.Generic.List<SomeEnum> FrepEnum
        {
            get { return _frepEnum; }
            set { _frepEnum = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::System.Collections.Generic.List<string> FrepString
        {
            get { return _frepString; }
            set { _frepString = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::System.Collections.Generic.List<uint> FrepFixed32
        {
            get { return _frepFixed32; }
            set { _frepFixed32 = value; }
        }
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::System.Collections.Generic.List<uint> FrepUint32
        {
            get { return _frepUint32; }
            set { _frepUint32 = value; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        void IMessage.WriteTo(IOutputStream output)
        {
            output.WriteRawTag(9);
            output.WriteDouble(_fdouble);
            output.WriteRawTag(21);
            output.WriteFloat(_ffloat);
            output.WriteRawTag(24);
            output.WriteInt32(_fint32);
            if (_fint64.HasValue)
            {
                output.WriteRawTag(32);
                output.WriteInt64(_fint64.Value);
            }
            if (_fuint32.HasValue)
            {
                output.WriteRawTag(40);
                output.WriteUInt32(_fuint32.Value);
            }
            if (_fuint64.HasValue)
            {
                output.WriteRawTag(48);
                output.WriteUInt64(_fuint64.Value);
            }
            if (_fsint32.HasValue)
            {
                output.WriteRawTag(56);
                output.WriteSInt32(_fsint32.Value);
            }
            if (_fsint64.HasValue)
            {
                output.WriteRawTag(64);
                output.WriteSInt64(_fsint64.Value);
            }
            if (_ffixed32.HasValue)
            {
                output.WriteRawTag(77);
                output.WriteFixed32(_ffixed32.Value);
            }
            if (_ffixed64.HasValue)
            {
                output.WriteRawTag(81);
                output.WriteFixed64(_ffixed64.Value);
            }
            if (_fsfixed32.HasValue)
            {
                output.WriteRawTag(93);
                output.WriteSFixed32(_fsfixed32.Value);
            }
            if (_fsfixed64.HasValue)
            {
                output.WriteRawTag(97);
                output.WriteSFixed64(_fsfixed64.Value);
            }
            if (_fbool.HasValue)
            {
                output.WriteRawTag(104);
                output.WriteBool(_fbool.Value);
            }
            if (!string.IsNullOrEmpty(_fstring))
            {
                output.WriteRawTag(114);
                output.WriteString(_fstring);
            }
            if (_fbytes != null && _fbytes.Length > 0)
            {
                output.WriteRawTag(122);
                output.WriteBytes(_fbytes);
            }
            if (_fenum.HasValue)
            {
                output.WriteRawTag(128, 1);
                output.WriteEnum((int)_fenum.Value);
            }
            if (_finner != null)
            {
                output.WriteRawTag(138, 1);
                output.WriteMessage(_finner);
            }
            if (_frep != null && _frep.Count > 0)
            {
                output.WriteRawTag(146, 1);
                output.WriteRepeated(_frep, _tinyRepeatedTypeTestroot_Classes_InnerMessage);
            }
            if (_frepEnum != null && _frepEnum.Count > 0)
            {
                output.WriteRawTag(152, 1);
                output.WriteRepeated(_frepEnum, _tinyRepeatedTypeTestroot_Classes_TestEnum);
            }
            if (_frepString != null && _frepString.Count > 0)
            {
                output.WriteRawTag(162, 1);
                output.WriteRepeated(_frepString, RepeatedTypes.String);
            }
            if (_frepFixed32 != null && _frepFixed32.Count > 0)
            {
                output.WriteRawTag(173, 1);
                output.WriteRepeated(_frepFixed32, RepeatedTypes.Fixed32);
            }
            if (_frepUint32 != null && _frepUint32.Count > 0)
            {
                output.WriteRawTag(176, 1);
                output.WriteRepeated(_frepUint32, RepeatedTypes.UInt32);
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        void IMessage.ReadFrom(IInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch (tag)
                {
                    case 9:
                        _fdouble = input.ReadDouble();
                        break;
                    case 21:
                        _ffloat = input.ReadFloat();
                        break;
                    case 24:
                        _fint32 = input.ReadInt32();
                        break;
                    case 32:
                        _fint64 = input.ReadInt64();
                        break;
                    case 40:
                        _fuint32 = input.ReadUInt32();
                        break;
                    case 48:
                        _fuint64 = input.ReadUInt64();
                        break;
                    case 56:
                        _fsint32 = input.ReadSInt32();
                        break;
                    case 64:
                        _fsint64 = input.ReadSInt64();
                        break;
                    case 77:
                        _ffixed32 = input.ReadFixed32();
                        break;
                    case 81:
                        _ffixed64 = input.ReadFixed64();
                        break;
                    case 93:
                        _fsfixed32 = input.ReadSFixed32();
                        break;
                    case 97:
                        _fsfixed64 = input.ReadSFixed64();
                        break;
                    case 104:
                        _fbool = input.ReadBool();
                        break;
                    case 114:
                        _fstring = input.ReadString();
                        break;
                    case 122:
                        _fbytes = input.ReadBytes();
                        break;
                    case 128:
                        _fenum = (SomeEnum)input.ReadEnum();
                        break;
                    case 138:
                        if (_finner == null)
                        {
                            _finner = new InnerMessage();
                        }
                        input.ReadMessage(_finner);
                        break;
                    case 146:
                        if (_frep == null)
                        {
                            _frep = new global::System.Collections.Generic.List<InnerMessage>();
                        }
                        input.ReadRepeated(_frep.Add, _tinyRepeatedTypeTestroot_Classes_InnerMessage, x => _frep.Capacity = _frep.Count + x);
                        break;
                    case 152:
                        if (_frepEnum == null)
                        {
                            _frepEnum = new global::System.Collections.Generic.List<SomeEnum>();
                        }
                        input.ReadRepeated(_frepEnum.Add, _tinyRepeatedTypeTestroot_Classes_TestEnum, x => _frepEnum.Capacity = _frepEnum.Count + x);
                        break;
                    case 162:
                        if (_frepString == null)
                        {
                            _frepString = new global::System.Collections.Generic.List<string>();
                        }
                        input.ReadRepeated(_frepString.Add, RepeatedTypes.String, x => _frepString.Capacity = _frepString.Count + x);
                        break;
                    case 173:
                        if (_frepFixed32 == null)
                        {
                            _frepFixed32 = new global::System.Collections.Generic.List<uint>();
                        }
                        input.ReadRepeated(_frepFixed32.Add, RepeatedTypes.Fixed32, x => _frepFixed32.Capacity = _frepFixed32.Count + x);
                        break;
                    case 176:
                        if (_frepUint32 == null)
                        {
                            _frepUint32 = new global::System.Collections.Generic.List<uint>();
                        }
                        input.ReadRepeated(_frepUint32.Add, RepeatedTypes.UInt32, x => _frepUint32.Capacity = _frepUint32.Count + x);
                        break;
                    default:
                        input.SkipField();
                        break;
                }
            }
        }

    }
}
