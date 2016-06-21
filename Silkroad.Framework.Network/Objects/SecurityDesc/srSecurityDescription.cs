using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.SecurityDesc
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SecurityDescription
    {
        [MarshalAs(UnmanagedType.U4)]
        public int nID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szDesc;
    }
}