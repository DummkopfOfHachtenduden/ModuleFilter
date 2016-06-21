using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.Certification
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct srUnknown
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte GlobalOperationID;

        [MarshalAs(UnmanagedType.U1)]
        public byte OperationType;

        [MarshalAs(UnmanagedType.U1)]
        public byte u3;

        [MarshalAs(UnmanagedType.U1)]
        public byte u4;

        [MarshalAs(UnmanagedType.U1)]
        public byte u5;

        [MarshalAs(UnmanagedType.U1)]
        public byte u6;
    }
}