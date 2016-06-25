using Silkroad.Framework.Utility;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.Certification
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct srNodeType : Unmanaged.IUnmanagedStruct, IKeyStruct
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint ID;

        [MarshalAs(UnmanagedType.U1)]
        public byte OperationType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string WIP;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string NIP;

        [MarshalAs(UnmanagedType.U2)]
        public ushort MachineManagerNodeID;

        public dynamic Key { get { return this.ID; } }
    }
}