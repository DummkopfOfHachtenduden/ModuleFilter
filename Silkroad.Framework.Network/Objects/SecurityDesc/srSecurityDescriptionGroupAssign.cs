using Silkroad.Framework.Utility;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.SecurityDesc
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SecurityDescriptionGroupAssign : Unmanaged.IUnmanagedStruct
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte nGroupID;

        [MarshalAs(UnmanagedType.U4)]
        public uint nDescriptionID;
    }
}