using Force.Crc32;

namespace LSLib.Utils
{
    public sealed class Crc32 
    {
        public static uint Compute(byte[] input, uint previousCrc32)
        {
            return Crc32Algorithm.Append(previousCrc32, input);
        }
    }
}
