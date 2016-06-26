using Silkroad.Framework.Utility;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects.Certification
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class srServiceType : Unmanaged.IUnmanagedStruct, IKeyStruct
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte ID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Name;

        public dynamic Key
        {
            get
            {
                return ID;
            }
        }

        public override string ToString()
        {
            return $"{ID} - {Name}";
        }
    }
}