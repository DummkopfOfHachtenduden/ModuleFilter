using Silkroad.Framework.Utility;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.Certification
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct srNodeLink : Unmanaged.IUnmanagedStruct, IKeyStruct
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint ID;

        [MarshalAs(UnmanagedType.U2)]
        public ushort ChildNodeID;

        [MarshalAs(UnmanagedType.U2)]
        public ushort ParentNodeID;

        [MarshalAs(UnmanagedType.U4)]
        public int PLabel;

        [MarshalAs(UnmanagedType.U1)]
        public byte u1;

        [MarshalAs(UnmanagedType.U1)]
        public byte u2;

        [MarshalAs(UnmanagedType.U1)]
        public byte u3;

        [MarshalAs(UnmanagedType.U1)]
        public byte u4;

        [MarshalAs(UnmanagedType.U1)]
        public byte u5;

        public dynamic Key { get { return ID; } }
    }
}