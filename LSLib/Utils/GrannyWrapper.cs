using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LSLib.Utils
{
    class GrannyWrapper
    {
        [DllImport("granny2.dll)")]
        private unsafe static extern bool GrannyDecompressData(Int32 format, bool FileIsByteReversed, int CompressedBytesSize, byte* compressedData, int Stop0, int Stop1, int Stop2, byte* decompressedData);
        [DllImport("granny2.dll)")]
        private unsafe static extern IntPtr GrannyBeginFileDecompression(Int32 format, bool FileIsByteReversed, int DecompressedBytesSize, byte* DecompressedBytes, int WorkMemSize, IntPtr WorkMemBuffer);
        [DllImport("granny2.dll)")]
        private unsafe static extern bool GrannyDecompressIncremental(IntPtr handle, int CompressedBytesSize, void* CompressedBytes);
        [DllImport("granny2.dll)")]
        private static extern bool GrannyEndFileDecompression(IntPtr handle);

        public static byte[] Decompress(Int32 format, byte[] compressed, Int32 decompressedSize, Int32 stop0, Int32 stop1, Int32 stop2)
        {
            byte[] decompressed = new byte[decompressedSize];
            unsafe
            {
                fixed (byte* compressedPtr = compressed, decompressedPtr = decompressed)
                {
                    if (!GrannyDecompressData(format, false, compressed.Length, compressedPtr, stop0, stop1, stop2, decompressedPtr))
                        throw new InvalidDataException("Failed to decompress Oodle compressed section.");
                }
            }
            return decompressed;
        }

        public static byte[] Decompress4(byte[] compressed, Int32 decompressedSize)
        {
            const int chunkSize = 0x2000;
            const int workBufferSize = 2 * chunkSize;

            byte[] decompressed = new byte[decompressedSize];
            IntPtr workerBuffer = Marshal.AllocHGlobal(workBufferSize);
            try
            {
                unsafe
                {
                    fixed (byte* decompressedPtr = decompressed, compressedPtr = compressed)
                    {
                        IntPtr handle = GrannyBeginFileDecompression(4, false, decompressedSize, decompressedPtr, workBufferSize, workerBuffer);
                        try
                        {
                            int offset = 0;
                            while (offset < compressed.Length)
                            {
                                int size = Math.Min(chunkSize, compressed.Length - offset);
                                if (!GrannyDecompressIncremental(handle, size, &compressedPtr[offset]))
                                    throw new InvalidDataException("Failed to decompress GR2 section increment.");
                                offset += size;

                            }
                        } finally
                        {
                            GrannyEndFileDecompression(handle);
                        }
                    }
                }
            } finally
            {
                Marshal.FreeHGlobal(workerBuffer);
            }
            return decompressed;
        }
    }
}
