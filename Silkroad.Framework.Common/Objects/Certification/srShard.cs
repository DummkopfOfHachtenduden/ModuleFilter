using Silkroad.Framework.Utility;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.Certification
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct srShard : Unmanaged.IUnmanagedStruct, IKeyStruct
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort ID;

        [MarshalAs(UnmanagedType.U1)]
        public byte GlobalOperationID;

        [MarshalAs(UnmanagedType.U1)]
        public byte OperationType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Query;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string QueryLog;

        [MarshalAs(UnmanagedType.U2)]
        public ushort Capacity;

        [MarshalAs(UnmanagedType.U2)]
        public ushort ShardManagerNodeID;

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

        [MarshalAs(UnmanagedType.U1)]
        public byte u6;

        [MarshalAs(UnmanagedType.U1)]
        public byte u7;

        public dynamic Key { get { return ID; } }
    }
}