using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SynapseMiNET.utils
{
    class Config
    {
        Dictionary<string, string> things = new Dictionary<string, string>();

        public Config()
        {
            Class1.Message("CONFIG PATH = " + getPath());
            if (!File.Exists(getPath())){
                File.Create(getPath()).Dispose();
                WriteDefaults();
            }
            else
            {
                Load();
                Validate();
            }
        }

        protected void Load()
        {
            List<string> readLines = new List<string>();
            using (TextReader reader = new StreamReader(getPath()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    readLines.Add(line);
                }

            }
            for (var i = 0; i < readLines.Count; i++)
            {
                if (!readLines[i].Contains("=")) continue;
                string[] keyAndValue = readLines[i].Split("=");

                string Key = keyAndValue[0];
                string Value = keyAndValue[1];
                things.Add(Key, Value);
            }
        }

        protected Dictionary<string, string> getDefaults()
        {
            Dictionary<string, string> required = new Dictionary<string, string>
            {
                { "address", "127.0.0.1" },
                { "port", "19133" },
                { "password", "123456789"},
                { "isLobbyServer", "false" },
                { "isMainServer", "false" },
                { "transferOnShutdown", "true" },
                { "identifier", "Synapse client for MiNET" }
            };

            return required;
        }

        protected void Validate()
        {
            Dictionary<string, string> defaults = getDefaults();

            int Missing = 0;

            foreach(KeyValuePair<string, string> thing in defaults)
            {
                if (getValue(thing.Key) == null) Missing++;
            }

            if(Missing > 0)
            {
                WriteDefaults();
                Class1.Message("Invalid config file! Replacing...");
            }
        }

        protected void WriteDefaults()
        {
            using (TextWriter writer = new StreamWriter(getPath()))
            {
                foreach(KeyValuePair<string, string> thing in getDefaults())
                {
                    writer.WriteLine(thing.Key + "=" + thing.Value);
                }
            }
            things = getDefaults();
        }

        public string getPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/SynapseMiNET/config.txt";
        }

        public string getValue(string key)
        {
            if (!things.ContainsKey(key)) return null;

            return things.GetValueOrDefault(key);
        }
    }
}
