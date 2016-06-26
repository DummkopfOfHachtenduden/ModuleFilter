using Silkroad.Framework.Utility;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.SecurityDesc
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SecurityDescription : Unmanaged.IUnmanagedStruct, IKeyStruct
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint nID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szDesc;

        public dynamic Key { get { return nID; } }
    }
}