using Nodindow.myPackages;
using static Nodindow.myPackages.TranslationManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;
using System.Drawing;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace Nodindow.nodindowObjects
{
    public static class globalValues
    {
        public static TranslationManager.Translations allTranslations = new TranslationManager.Translations();
        public static LiteDBManager liteDBManager = new LiteDBManager($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Nodindow\\saves\\data.db");
        public static string language = "pt";
        static public void loadTranslations()
        {
            allTranslations.load($"{Directory.GetCurrentDirectory()}\\lang.json");
        }

        public static class hotkeyManager
        {
            public static Hotkey allHotkeys { get; set; }

            public class defaultHotkeys
            {
                public static Hotkey.HotkeyO nexSpace = new Hotkey.HotkeyO(0, globalValues.hotkeyManager.hotkeysNames.nextSpace.ToString(), Keys.Right, Hotkey.MOD_WIN | Hotkey.MOD_CONTROL | Hotkey.MOD_ALT);
                public static Hotkey.HotkeyO previousSpace = new Hotkey.HotkeyO(1, globalValues.hotkeyManager.hotkeysNames.previousSpace.ToString(), Keys.Left, Hotkey.MOD_WIN | Hotkey.MOD_CONTROL | Hotkey.MOD_ALT);
                public static Hotkey.HotkeyO nextSpacePerScreen = new Hotkey.HotkeyO(2, globalValues.hotkeyManager.hotkeysNames.nextSpacePerScreen.ToString(), Keys.Down, Hotkey.MOD_WIN | Hotkey.MOD_CONTROL | Hotkey.MOD_ALT);
                public static Hotkey.HotkeyO previousSpacePerScreen = new Hotkey.HotkeyO(3, globalValues.hotkeyManager.hotkeysNames.previousSpacePerScreen.ToString(), Keys.Up, Hotkey.MOD_WIN | Hotkey.MOD_CONTROL | Hotkey.MOD_ALT);
                public static Hotkey.HotkeyO showHideNodindow = new Hotkey.HotkeyO(4, globalValues.hotkeyManager.hotkeysNames.showHideNodindow.ToString(), Keys.Home, Hotkey.MOD_WIN | Hotkey.MOD_CONTROL | Hotkey.MOD_ALT);
            }
            public enum hotkeysNames
            {
                nextSpace,
                previousSpace,
                nextSpacePerScreen,
                previousSpacePerScreen,
                showHideNodindow
            }
        }
        public static void showBoundsView(Size size, Point position, Color color, int timeVisible = 1000, int borderWidth = 2)
        {
            Thread thread = new Thread(() =>
            {
                myPackages.formTools.boundsView boundsView = new myPackages.formTools.boundsView(size, position, color, timeVisible, borderWidth);
                Application.Run(boundsView);
            });
            thread.Start();
        }

        public class LiteDBManager
        {
            public LiteDatabase LiteDatabase { get; set; }
            public enum namesCollections
            {
                mainTreeViewAdvanced,
                globalHotkeys,
                currentLanguage,
                modifiedWindows,
            }
            public LiteDBManager(string filePath)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                LiteDatabase = new LiteDatabase(filePath);
            }
            public string getData(string collectionName)
            {
                var colecao = LiteDatabase.GetCollection<BsonDocument>(collectionName);
                var documento = colecao.FindOne(Query.All());
                if (documento != null)
                {
                    return documento["data"].AsString;
                }
                return string.Empty;
            }
            public void addData(string collectionName, string data)
            {
                var dataSave = new BsonDocument
                {
                    ["data"] = data
                };
                this.LiteDatabase.GetCollection<BsonDocument>(collectionName).Insert(dataSave);
            }
            public void updateData(string collectionName, string data)
            {
                var collection = LiteDatabase.GetCollection<BsonDocument>(collectionName);
                var documentExist = collection.FindOne(Query.All());

                if (documentExist != null)
                {
                    documentExist["data"] = data;
                    collection.Update(documentExist);
                }
                else
                {
                    addData(collectionName, data);
                }
            }
        }
        static public class translationsManager
        {
            static public class mainFormTranslation
            {
                static public class treeNodeAdvanced
                {
                    static public string ignoredForms { get { return allTranslations.getTranslation("treeNode.ignoredForms", language); } }
                    static public string settings { get { return allTranslations.getTranslation("treeNode.settings", language); } }
                    static public string monitors { get { return allTranslations.getTranslation("treeNode.monitors", language); } }
                    static public string sphereOfWork { get { return allTranslations.getTranslation("treeNode.sphereOfWork", language); } }
                    static public string mainWorkspace { get { return allTranslations.getTranslation("treeNode.mainWorkspace", language); } }
                    static public string mainSpace { get { return allTranslations.getTranslation("treeNode.mainSpace", language); } }
                }
                static public class contextMenu
                {
                    static public string newNode { get { return allTranslations.getTranslation("contextMenu.new", language); } }
                    static public string workspace { get { return allTranslations.getTranslation("contextMenu.workspace", language); } }
                    static public string Space { get { return allTranslations.getTranslation("contextMenu.space", language); } }
                    static public string rename { get { return allTranslations.getTranslation("contextMenu.rename", language); } }
                    static public string show { get { return allTranslations.getTranslation("contextMenu.show", language); } }
                    static public string hide { get { return allTranslations.getTranslation("contextMenu.hide", language); } }
                    static public string spacePerScreen { get { return allTranslations.getTranslation("contextMenu.spacePerScreen", language); } }
                    static public string delete { get { return allTranslations.getTranslation("contextMenu.delete", language); } }
                    static public class messageDialog
                    {
                        static public string delete { get { return allTranslations.getTranslation("contextMenu.delete.MsgDialog", language); } }
                        static public string deleteCaption { get { return allTranslations.getTranslation("contextMenu.delete.MsgDialog.caption", language); } }
                    }
                    static public class tools
                    {
                        static public string setNameWorkspace { get { return allTranslations.getTranslation("tools.setNameWorkspace", language); } }
                        static public string setNameSpace { get { return allTranslations.getTranslation("tools.setNameSpace", language); } }
                        static public string setNameScreenPerSpace { get { return allTranslations.getTranslation("tools.setNameScreenPerSpace", language); } }
                        static public string newName { get { return allTranslations.getTranslation("tools.Rename", language); } }
                        static public string cancel { get { return allTranslations.getTranslation("tools.button.cancel", language); } }
                        static public string ok { get { return allTranslations.getTranslation("tools.button.ok", language); } }
                    }
                }
                static public class toolStrip
                {
                    static public string display { get { return allTranslations.getTranslation("menuStrip1.display", language); } }
                    static public string configurations { get { return allTranslations.getTranslation("menuStrip1.display.configurations", language); } }
                }
            }
            public class configurationsTranslation
            {
                static public string formTitle { get { return allTranslations.getTranslation("configuration.form.title", language); } }
                public class interfaceConfig
                {
                    static public string changeLang { get { return allTranslations.getTranslation("configuration.button.changeLang", language); } }
                    static public string languageLbl { get { return allTranslations.getTranslation("configuration.label.language", language); } }
                    static public string languageNotBeCorrect { get { return allTranslations.getTranslation("configuration.caption.languageNotBeCorrect", language); } }
                    static public string interfaceTab { get { return allTranslations.getTranslation("configuration.tab.interface", language); } }
                }
                public class hotKeysConfig
                {
                    static public string hotkeysPage { get { return allTranslations.getTranslation("configuration.tab.hotKeys.page", language); } }
                    static public string nextSpace { get { return allTranslations.getTranslation("configuration.tab.hotKeys.label.nextSpace", language); } }
                    static public string previousSpace { get { return allTranslations.getTranslation("configuration.tab.hotKeys.label.previousSpace", language); } }
                    static public string nextSpacePerScreen { get { return allTranslations.getTranslation("configuration.tab.hotKeys.label.nextSpacePerScreen", language); } }
                    static public string previousSpacePerScreen { get { return allTranslations.getTranslation("configuration.tab.hotKeys.label.previousSpacePerScreen", language); } }
                    static public string showHideNodindow { get { return allTranslations.getTranslation("configuration.tab.hotKeys.label.showHideNodindow", language); } }
                }
            }
        }
        static public class filePaths
        {
            static public string appPath { get { return Directory.GetCurrentDirectory(); } }
            static public string workspace { get { return $"{appPath}\\src\\Workspace 24x24.png"; } }
            static public string space { get { return $"{appPath}\\src\\Space 24x24.png"; } }
            static public string monitors { get { return $"{appPath}\\src\\monitors24x24.png"; } }

            static public string appIcon { get { return $"{appPath}\\src\\appIcon 256x256.ico"; } }
            static public string excludedFormIcon { get { return $"{appPath}\\src\\ExcludedForm 24x24.ico"; } }
            static public string workspaceIcon { get { return $"{appPath}\\src\\Workspace 24x24.ico"; } }
            static public string spaceIcon { get { return $"{appPath}\\src\\Space 24x24.ico"; } }
            static public string formIcon { get { return $"{appPath}\\src\\Form 24x24.ico"; } }
            static public string windowIcon { get { return $"{appPath}\\src\\window 24x24.ico"; } }
            static public string monitorIcon { get { return $"{appPath}\\src\\monitor24x24.ico"; } }
            static public string monitorsIcon { get { return $"{appPath}\\src\\monitors24x24.ico"; } }
            static public string configIcon { get { return $"{appPath}\\src\\Config 24x24.ico"; } }
            static public string packageFormIcon { get { return $"{appPath}\\src\\packageForm24x24.ico"; } }
        }
    }
}
