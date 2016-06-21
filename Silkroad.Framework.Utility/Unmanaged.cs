using System;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Utility
{
    public class Unmanaged
    {
        public static T BufferToStruct<T>(byte[] buffer, int offset = 0) where T : struct
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    return (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));
                }
            }
        }

        public static byte[] StructToBuffer<T>(T structure) where T : struct
        {
            var buffer = new byte[Marshal.SizeOf(typeof(T))];
            unsafe
            {
                fixed (byte* ptr = &buffer[0])
                {
                    Marshal.StructureToPtr(structure, (IntPtr)ptr, false);
                    return buffer;
                }
            }
        }
    }
}