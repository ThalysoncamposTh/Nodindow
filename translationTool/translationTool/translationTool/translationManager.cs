using Google.Cloud.Translation.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace translationTool
{
    public static class TranslationManager
    {
        private static readonly string apiKey = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_API_KEY");
        public static string TranslateText(string text, string targetLanguage)
        {
            // Instancia o cliente de tradução
            var client = TranslationClient.CreateFromApiKey(apiKey);

            // Faz a tradução
            var response = client.TranslateText(text, targetLanguage);

            // Retorna o texto traduzido
            return response.TranslatedText;
        }
        public class Translations
        {
            public List<Translation> translations = new List<Translation>();
            public Dictionary<string, string> languages = new Dictionary<string, string>();
            public void add(Translation translation)
            {
                this.translations.Add(translation);
            }
            public void translate(Translation translationModel)
            {
                languages.Keys.ToList().ForEach(language =>
                {
                    int translationIndex = translations.FindIndex(translation => translation.language == language);
                    if (translationIndex == -1)
                    {
                        translations.Add(new Translation(language, translationModel));
                    }
                    else
                    {
                        translations[translationIndex].generateTranslations(translationModel);
                    }
                });
            }
            public void load(string path)
            {
                this.translations = JsonConvert.DeserializeObject<Translations>(File.ReadAllText(path)).translations;
            }
            public void save(string path)
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            public class Translation
            {
                public Dictionary<string, string> translations = new Dictionary<string, string>();
                public string language { get; set; }
                [JsonConstructor]
                public Translation(string language, Dictionary<string, string> translations = null)
                {
                    this.language = language;
                    this.translations = translations == null ? new Dictionary<string, string>() : translations;
                }
                public Translation(string language, Translation translationModel)
                {
                    this.language = language;
                    generateTranslations(translationModel);
                }
                public void add(string name, string text)
                {
                    translations.Add(name, text);
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
                public void generateTranslations(Translation translationModel)
                {
                    if (translationModel.language != language)
                    {
                        List<string> keys = translationModel.translations.Keys.ToList();
                        foreach (string key in keys)
                        {
                            if (!translations.ContainsKey(key))
                            {
                                translations.Add(key, TranslationManager.TranslateText(translationModel.translations[key], language));
                            }
                        }
                    }
                }
            }
        }
    }
}
