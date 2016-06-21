using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.Certification
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct srNodeData
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort NodeID;

        [MarshalAs(UnmanagedType.U1)]
        public byte OperationType;

        [MarshalAs(UnmanagedType.U1)]
        public byte GlobalOperationID;

        [MarshalAs(UnmanagedType.U2)]
        public ushort AssociatedShardID;

        [MarshalAs(UnmanagedType.U4)]
        public uint NodeType;

        [MarshalAs(UnmanagedType.U2)]
        public ushort ServiceType;

        [MarshalAs(UnmanagedType.U2)]
        public ushort CertificationNodeID;

        [MarshalAs(UnmanagedType.U2)]
        public ushort Port;

        [MarshalAs(UnmanagedType.U4)]
        public uint NodeIcon;

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

        [MarshalAs(UnmanagedType.U1)]
        public byte u8;

        [MarshalAs(UnmanagedType.U1)]
        public byte u9;

        [MarshalAs(UnmanagedType.U1)]
        public byte u10;

        [MarshalAs(UnmanagedType.U1)]
        public byte u11;

        [MarshalAs(UnmanagedType.U1)]
        public byte u12;

        [MarshalAs(UnmanagedType.U1)]
        public byte u13;

        [MarshalAs(UnmanagedType.U1)]
        public byte u14;

        [MarshalAs(UnmanagedType.U1)]
        public byte u15;

        [MarshalAs(UnmanagedType.U1)]
        public byte u16;

        [MarshalAs(UnmanagedType.U1)]
        public byte u17;

        [MarshalAs(UnmanagedType.U1)]
        public byte u18;

        [MarshalAs(UnmanagedType.U1)]
        public byte u19;

        [MarshalAs(UnmanagedType.U1)]
        public byte u20;
    }
}