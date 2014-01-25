using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lostics.NCryptoExchange
{
    public static class TestUtils
    {
        public static T LoadTestData<T>(string folder, string filename)
            where T : JToken
        {
            return (T)JToken.Parse(LoadTestDataRaw(folder, filename));
        }

        public static string LoadTestDataRaw(string folder, string filename)
        {
            string testDir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string testDataDir = Path.Combine(Path.Combine(testDir, "Sample_Data"), folder);
            FileInfo fileName = new FileInfo(Path.Combine(testDataDir, filename));

            using (StreamReader reader = new StreamReader(new FileStream(fileName.FullName, FileMode.Open)))
            {
                return reader.ReadToEndAsync().Result;
            }
        }
    }
}
