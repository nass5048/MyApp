using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace WordleGuesser
{
    public static class Utilitys
    {
        public static string GetEmbeddedTextResource(string resourceName)
        {
            // Get the assembly where the resource is embedded
            var assembly = Assembly.GetExecutingAssembly();

            // Construct the full resource name. This usually follows the pattern:
            // "YourDefaultNamespace.YourSubfolderIfAny.YourFileName.txt"
            // You can find the exact name using assembly.GetManifestResourceNames() if unsure.
            string fullResourceName = $"{assembly.GetName().Name}.{resourceName}";

            using (Stream stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Embedded resource '{fullResourceName}' not found.");
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static bool IsWordInText(string word, string resourceName)
        {
            var ValidWords = new List<string>();

            var wordList = Utilitys.GetEmbeddedTextResource(resourceName);

            ValidWords = wordList.Split('\n').ToList();

            return ValidWords.Contains(word);
        }
    }
}
