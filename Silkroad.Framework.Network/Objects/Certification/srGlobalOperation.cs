using Silkroad.Framework.Utility;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.Certification
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct srGlobalOperation : Unmanaged.IUnmanagedStruct, IKeyStruct
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte ID;

        [MarshalAs(UnmanagedType.U1)]
        public byte OperationType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Query;

        public dynamic Key { get { return ID; } }
    }
}