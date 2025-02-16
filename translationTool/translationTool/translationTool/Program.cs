using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static translationTool.TranslationManager;

namespace translationTool
{
    internal class Program
    {
        private static TranslationManager.Translations allTranslations = new TranslationManager.Translations();
        private static readonly string filePath = Path.Combine(Directory.GetCurrentDirectory(), "lang.json");

        static void Main(string[] args)
        {
            LoadTranslations();
            InitializeMainTranslations();
            AddTargetLanguages();
            RemoveUnusedLanguages();
            RemoveUnusedKeys();
            TranslateAndSave();
        }

        private static void LoadTranslations()
        {
            if (File.Exists(filePath))
            {
                allTranslations.load(filePath);
            }
        }

        private static void InitializeMainTranslations()
        {
            var mainTranslations = new Dictionary<string, string>
            {
                {"treeNode.ignoredForms", "Ignored forms"},
                {"treeNode.settings", "Settings"},
                {"treeNode.monitors", "Monitors"},
                {"treeNode.sphereOfWork", "Sphere of work"},
                {"treeNode.mainWorkspace", "Main workspace"},
                {"treeNode.mainSpace", "Main space"},


                {"contextMenu.new", "New"},
                {"contextMenu.workspace", "Workspace"},
                {"contextMenu.show", "Show"},
                {"contextMenu.hide", "Hide"},
                {"contextMenu.rename", "Rename"},
                {"contextMenu.spacePerScreen", "Space per screen"},
                {"contextMenu.space", "Space"},
                {"contextMenu.delete", "Delete"},
                {"contextMenu.delete.MsgDialog", "Are you sure you want to delete the (1) ?"},
                {"contextMenu.delete.MsgDialog.caption", "Confirm"},


                {"tools.setNameWorkspace", "Workspace name"},
                {"tools.setNameSpace", "Space name"},
                {"tools.setNameScreenPerSpace", "Space name per screen"},
                {"tools.Rename", "New name"},
                {"tools.button.cancel", "Cancel"},
                {"tools.button.ok", "Ok"},

                {"configuration.button.changeLang", "Change"},
                {"configuration.label.language", "Language:"},
                {"configuration.caption.languageNotBeCorrect", "Attention: translations may not be 100% correct."},
                {"configuration.tab.interface", "Interface"},

                {"configuration.form.title", "Configurations"},
                {"configuration.tab.hotKeys.page", "Hotkeys"},
                {"configuration.tab.hotKeys.label.nextSpace", "Next space"},
                {"configuration.tab.hotKeys.label.previousSpace", "Previous space"},
                {"configuration.tab.hotKeys.label.nextSpacePerScreen", "Next space per screen"},
                {"configuration.tab.hotKeys.label.previousSpacePerScreen", "Previous space per screen"},
                {"configuration.tab.hotKeys.label.showHideNodindow", "Show/hide nodindow"},

                {"menuStrip1.display", "Display"},
                {"menuStrip1.display.configurations", "Configurations"},

                {"notifyIcon.contextMenuStrip.Item.show", "Show"},
                {"notifyIcon.contextMenuStrip.Item.close", "Close"},
                {"notifyIcon.dialogs.applicationRunning", "The application continues to run in the background. Access via the icon in the tray."},
            };

            if (allTranslations.translations.Count > 0)
            {
                allTranslations.translations[0].translations = mainTranslations;
            }
            else
            {
                allTranslations.add(new TranslationManager.Translations.Translation("en", mainTranslations));
            }
        }

        private static void AddTargetLanguages()
        {
            allTranslations.languages = new Dictionary<string, string>
            {
                { "en", "English" },
                { "es", "Spanish" },
                { "zh-CN", "Chinese (Simplified)" },
                { "hi", "Hindi" },
                { "ar", "Arabic" },
                { "pt", "Portuguese" },
                { "fr", "French" },
                { "ru", "Russian" },
                { "de", "German" },
                { "ja", "Japanese" }
            };
        }

        private static void RemoveUnusedLanguages()
        {
            allTranslations.translations = allTranslations.translations
                .Where(translation => allTranslations.languages.ContainsKey(translation.language))
                .ToList();
        }

        private static void RemoveUnusedKeys()
        {
            var baseTranslations = allTranslations.translations[0].translations.Keys;

            foreach (var translation in allTranslations.translations)
            {
                var keysToRemove = translation.translations.Keys
                    .Where(key => !baseTranslations.Contains(key))
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    translation.translations.Remove(key);
                }
            }
        }

        private static void TranslateAndSave()
        {
            allTranslations.translate(allTranslations.translations[0]);
            allTranslations.save(filePath);
        }
    }
}
