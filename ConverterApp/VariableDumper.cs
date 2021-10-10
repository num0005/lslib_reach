using LSLib.LS;
using System;
using System.IO;
using System.Text;

namespace ConverterApp
{
    class VariableDumper : IDisposable
    {
        private StreamWriter Writer;
        private Resource Rsrc;

        public bool IncludeDeletedVars { get; set; }
        public bool IncludeLocalScopes { get; set; }

        public VariableDumper(Stream outputStream)
        {
            Writer = new StreamWriter(outputStream, Encoding.UTF8);
            IncludeDeletedVars = false;
            IncludeLocalScopes = false;
        }

        public void Dispose()
        {
            Writer.Dispose();
        }

        private void DumpCharacter(Node characterNode)
        {
        }

        private void DumpItem(Node itemNode)
        {
        }



        public void Load(Resource resource)
        {
            Rsrc = resource;
            Node osiHelper = resource.Regions["OsirisVariableHelper"];
        }

        public void DumpGlobals()
        {
            Node osiHelper = Rsrc.Regions["OsirisVariableHelper"];
            var globalVarsNode = osiHelper.Children["VariableManager"][0];

            Writer.WriteLine(" === DUMP OF GLOBALS === ");
        }

        public void DumpCharacters()
        {
            Writer.WriteLine();
            Writer.WriteLine(" === DUMP OF CHARACTERS === ");
            var characters = Rsrc.Regions["Characters"].Children["CharacterFactory"][0].Children["Characters"][0].Children["Character"];
            foreach (var character in characters)
            {
                DumpCharacter(character);
            }
        }

        public void DumpItems()
        {
            Writer.WriteLine();
            Writer.WriteLine(" === DUMP OF ITEMS === ");
            var items = Rsrc.Regions["Items"].Children["ItemFactory"][0].Children["Items"][0].Children["Item"];
            foreach (var item in items)
            {
                DumpItem(item);
            }
        }
    }
}
