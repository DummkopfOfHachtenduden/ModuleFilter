using Silkroad.Framework.Utility;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.SecurityDesc
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SecurityDescriptionGroup : Unmanaged.IUnmanagedStruct, IKeyStruct
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte nID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szDesc;

        public dynamic Key { get { return nID; } }
    }
}