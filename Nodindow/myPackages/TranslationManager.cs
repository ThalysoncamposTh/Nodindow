using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodindow.myPackages
{
    public static class TranslationManager
    {
        public class Translations
        {
            public List<Translation> translations = new List<Translation>();
            public Dictionary<string,string> languages = new Dictionary<string, string>();
            public void add(Translation translation)
            {
                this.translations.Add(translation);
            }
            public string getTranslation(string name, string lang)
            {
                return translations.Find(translation => translation.language == lang).translations[name];
            }
            public string getAcronym(string name)
            {
                List<string> acronymLangs = languages.Keys.ToList();
                foreach (var item in acronymLangs)
                {
                    if(languages[item] == name)
                    {
                        return item;
                    }
                }
                return "en";
            }
            public void load(string path)
            {
                Translations translations = JsonConvert.DeserializeObject<Translations>(File.ReadAllText(path));
                this.translations = translations.translations;
                this.languages = translations.languages;
            }
            public class Translation
            {
                public Dictionary<string, string> translations = new Dictionary<string, string>();
                public string language { get; set; }
                public Translation(string language, Dictionary<string, string> translations = null)
                {
                    this.language = language;
                    this.translations = translations == null ? new Dictionary<string, string>() : translations;
                }
                public string get(string name)
                {
                    if (translations.ContainsKey(name))
                    {
                        return translations[name];
                    }
                    else
                    {
                        throw new Exception($"No translation for {name}");
                    }
                }
            }
        }
    }
}
