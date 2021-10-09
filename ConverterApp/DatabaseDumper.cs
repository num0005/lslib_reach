using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ConverterApp
{
    class DatabaseDumper : IDisposable
    {
        private StreamWriter Writer;

        public bool DumpUnnamedDbs { get; set; }

        public DatabaseDumper(Stream outputStream)
        {
            Writer = new StreamWriter(outputStream, Encoding.UTF8);
            DumpUnnamedDbs = false;
        }

        public void Dispose()
        {
            Writer.Dispose();
        }

    }
}
