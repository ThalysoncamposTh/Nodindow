using Nodindow.myPackages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nodindow.nodindowObjects.forms
{
    public partial class configuration : Form
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        private Keys currentHotkeyMainKey = Keys.None;
        private uint currentHotkeyModifiers = 0;
        private string currentHotkeyName = "";
        private List<Form> forms { get; set; }
        public configuration(List<Form> forms)
        {
            this.forms = forms;
            InitializeComponent();
        }

        private void configuration_Load(object sender, EventArgs e)
        {
            langCb.Text = globalValues.allTranslations.languages[globalValues.language];
            globalValues.allTranslations.languages.Values.ToList().ForEach(langName =>
            {
                langCb.Items.Add(langName);
            });
            nextSpaceTxt.Text = globalValues.hotkeyManager.allHotkeys.hotkeys[globalValues.hotkeyManager.allHotkeys.hotkeys.FindIndex(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.nextSpace.ToString())].GetStrHotkey();
            nextSpaceTxt.KeyDown += (object sender1, KeyEventArgs e1) =>
            {
                getHotkeyEvent(sender1, e1, globalValues.hotkeyManager.hotkeysNames.nextSpace.ToString());
            };
            nextSpaceTxt.KeyUp += (object sender1, KeyEventArgs e1) =>
            {
                setHotKeyEvent(sender1, e1);
            };

            previousSpaceTxt.Text = globalValues.hotkeyManager.allHotkeys.hotkeys[globalValues.hotkeyManager.allHotkeys.hotkeys.FindIndex(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.previousSpace.ToString())].GetStrHotkey();
            previousSpaceTxt.KeyDown += (object sender1, KeyEventArgs e1) =>
            {
                getHotkeyEvent(sender1, e1, globalValues.hotkeyManager.hotkeysNames.previousSpace.ToString());
            };
            previousSpaceTxt.KeyUp += (object sender1, KeyEventArgs e1) =>
            {
                setHotKeyEvent(sender1, e1);
            };

            showHideNodindowTxt.Text = globalValues.hotkeyManager.allHotkeys.hotkeys[globalValues.hotkeyManager.allHotkeys.hotkeys.FindIndex(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.showHideNodindow.ToString())].GetStrHotkey();
            showHideNodindowTxt.KeyDown += (object sender1, KeyEventArgs e1) =>
            {
                getHotkeyEvent(sender1, e1, globalValues.hotkeyManager.hotkeysNames.showHideNodindow.ToString());
            };
            showHideNodindowTxt.KeyUp += (object sender1, KeyEventArgs e1) =>
            {
                setHotKeyEvent(sender1, e1);
            };

            nextSpacePerScreenTxt.Text = globalValues.hotkeyManager.allHotkeys.hotkeys[globalValues.hotkeyManager.allHotkeys.hotkeys.FindIndex(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.nextSpacePerScreen.ToString())].GetStrHotkey();
            nextSpacePerScreenTxt.KeyDown += (object sender1, KeyEventArgs e1) =>
            {
                getHotkeyEvent(sender1, e1, globalValues.hotkeyManager.hotkeysNames.nextSpacePerScreen.ToString());
            };
            nextSpacePerScreenTxt.KeyUp += (object sender1, KeyEventArgs e1) =>
            {
                setHotKeyEvent(sender1, e1);
            };

            previousSpacePerScreenTxt.Text = globalValues.hotkeyManager.allHotkeys.hotkeys[globalValues.hotkeyManager.allHotkeys.hotkeys.FindIndex(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.previousSpacePerScreen.ToString())].GetStrHotkey();
            previousSpacePerScreenTxt.KeyDown += (object sender1, KeyEventArgs e1) =>
            {
                getHotkeyEvent(sender1, e1, globalValues.hotkeyManager.hotkeysNames.previousSpacePerScreen.ToString());
            };
            previousSpacePerScreenTxt.KeyUp += (object sender1, KeyEventArgs e1) =>
            {
                setHotKeyEvent(sender1, e1);
            };

            updateLanguage();
            void getHotkeyEvent(object sender1, KeyEventArgs e1, string currentHotkeyName)
            {
                // Limpar o texto do TextBox
                var textBox = sender1 as TextBox;
                textBox.Clear();
                globalValues.hotkeyManager.allHotkeys.DisableHotkeys();

                // Capturar os modificadores manualmente
                uint modifiers = 0;
                if ((GetAsyncKeyState((int)Keys.ControlKey) & 0x8000) != 0)
                    modifiers |= Hotkey.MOD_CONTROL;
                if ((GetAsyncKeyState((int)Keys.Menu) & 0x8000) != 0) // Alt
                    modifiers |= Hotkey.MOD_ALT;
                if ((GetAsyncKeyState((int)Keys.ShiftKey) & 0x8000) != 0)
                    modifiers |= Hotkey.MOD_SHIFT;
                if ((GetAsyncKeyState((int)Keys.LWin) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.RWin) & 0x8000) != 0)
                    modifiers |= Hotkey.MOD_WIN;

                // Capturar a tecla principal
                Keys mainKey = e1.KeyCode;

                // Atualizar o TextBox com a combinação
                string hotkeyString = Hotkey.FormatHotkey(mainKey, modifiers);
                textBox.Text = hotkeyString;

                // Atualizar o atalho no sistema
                currentHotkeyMainKey = mainKey;
                currentHotkeyModifiers = modifiers;
                this.currentHotkeyName = currentHotkeyName;

                // Impedir que outros eventos de tecla sejam acionados
                e1.SuppressKeyPress = true;
                e1.Handled = true;
            }
            void setHotKeyEvent(object sender1, KeyEventArgs e1)
            {
                var textBox = sender1 as TextBox;
                // Verificar se a tecla principal é válida
                if (currentHotkeyMainKey == Keys.ControlKey || currentHotkeyMainKey == Keys.Menu || currentHotkeyMainKey == Keys.ShiftKey ||
                    currentHotkeyMainKey == Keys.LWin || currentHotkeyMainKey == Keys.RWin || currentHotkeyMainKey == Keys.None)
                {
                    // Mostrar mensagem de erro ou notificar o usuário
                    MessageBox.Show("Atalho inválido. Por favor, inclua uma tecla principal além dos modificadores.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = globalValues.hotkeyManager.allHotkeys.hotkeys[globalValues.hotkeyManager.allHotkeys.hotkeys.FindIndex(hotkey => hotkey.name == currentHotkeyName)].GetStrHotkey();
                    hotKeysPage.Focus();
                    return;
                }
                string name = globalValues.hotkeyManager.allHotkeys.hotkeys[globalValues.hotkeyManager.allHotkeys.hotkeys.FindIndex(hotkey => hotkey.name == currentHotkeyName)].name;
                // Atualizar o atalho no sistema
                globalValues.hotkeyManager.allHotkeys.UpdateHotkey(globalValues.hotkeyManager.allHotkeys.hotkeys.Find(hotkey => hotkey.name == currentHotkeyName), currentHotkeyMainKey, currentHotkeyModifiers);
                globalValues.hotkeyManager.allHotkeys.EnableHotkeys();
                textBox.Text = globalValues.hotkeyManager.allHotkeys.hotkeys.Find(hotkey => hotkey.name == name).GetStrHotkey();
                currentHotkeyMainKey = Keys.None;
                currentHotkeyModifiers = 0;
                currentHotkeyName = "";
                globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.globalHotkeys.ToString(),globalValues.hotkeyManager.allHotkeys.getJsonSave());
                // Definir o foco na página de atalhos
                hotKeysPage.Focus();
            }
        }
        public void updateLanguage()
        {
            this.Text = globalValues.translationsManager.configurationsTranslation.formTitle;
            interfacePage.Text = globalValues.translationsManager.configurationsTranslation.interfaceConfig.interfaceTab;
            langLbl.Text = globalValues.translationsManager.configurationsTranslation.interfaceConfig.languageLbl;
            captionLangLbl.Text = globalValues.translationsManager.configurationsTranslation.interfaceConfig.languageNotBeCorrect;
            changeLangBtn.Text = globalValues.translationsManager.configurationsTranslation.interfaceConfig.changeLang;

            nextSpaceLbl.Text = globalValues.translationsManager.configurationsTranslation.hotKeysConfig.nextSpace;
            previousSpaceLbl.Text = globalValues.translationsManager.configurationsTranslation.hotKeysConfig.previousSpace;
            nextSpacePerScreenLbl.Text = globalValues.translationsManager.configurationsTranslation.hotKeysConfig.nextSpacePerScreen;
            previousSpacePerScreenLbl.Text = globalValues.translationsManager.configurationsTranslation.hotKeysConfig.previousSpacePerScreen;
            showHideNodindowLbl.Text = globalValues.translationsManager.configurationsTranslation.hotKeysConfig.showHideNodindow;
            hotKeysPage.Text = globalValues.translationsManager.configurationsTranslation.hotKeysConfig.hotkeysPage;
            ((Form1)forms.Find(form => form.GetType() == typeof(Form1))).updateLanguage();
        }

        private void changeLangBtn_Click(object sender, EventArgs e)
        {
            globalValues.language = globalValues.allTranslations.getAcronym(langCb.Text);
            updateLanguage();
            globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.currentLanguage.ToString(), globalValues.language);
        }

        private void nextSpacePic_Click(object sender, EventArgs e)
        {
            nextSpaceTxt.Text = globalValues.hotkeyManager.allHotkeys.UpdateHotkey(globalValues.hotkeyManager.allHotkeys.hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.nextSpace.ToString()), globalValues.hotkeyManager.defaultHotkeys.nexSpace.keys, globalValues.hotkeyManager.defaultHotkeys.nexSpace.modifiers).GetStrHotkey();
            globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.globalHotkeys.ToString(), globalValues.hotkeyManager.allHotkeys.getJsonSave());
        }

        private void previousSpacePic_Click(object sender, EventArgs e)
        {
            previousSpaceTxt.Text = globalValues.hotkeyManager.allHotkeys.UpdateHotkey(globalValues.hotkeyManager.allHotkeys.hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.previousSpace.ToString()), globalValues.hotkeyManager.defaultHotkeys.previousSpace.keys, globalValues.hotkeyManager.defaultHotkeys.previousSpace.modifiers).GetStrHotkey();
            globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.globalHotkeys.ToString(), globalValues.hotkeyManager.allHotkeys.getJsonSave());
        }

        private void nextSpacePerScreenPic_Click(object sender, EventArgs e)
        {
            nextSpacePerScreenTxt.Text = globalValues.hotkeyManager.allHotkeys.UpdateHotkey(globalValues.hotkeyManager.allHotkeys.hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.nextSpacePerScreen.ToString()), globalValues.hotkeyManager.defaultHotkeys.nextSpacePerScreen.keys, globalValues.hotkeyManager.defaultHotkeys.nextSpacePerScreen.modifiers).GetStrHotkey();
            globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.globalHotkeys.ToString(), globalValues.hotkeyManager.allHotkeys.getJsonSave());
        }

        private void previousSpacePerScreenPic_Click(object sender, EventArgs e)
        {
            previousSpacePerScreenTxt.Text = globalValues.hotkeyManager.allHotkeys.UpdateHotkey(globalValues.hotkeyManager.allHotkeys.hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.previousSpacePerScreen.ToString()), globalValues.hotkeyManager.defaultHotkeys.previousSpacePerScreen.keys, globalValues.hotkeyManager.defaultHotkeys.previousSpacePerScreen.modifiers).GetStrHotkey();
            globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.globalHotkeys.ToString(), globalValues.hotkeyManager.allHotkeys.getJsonSave());
        }

        private void showHideNodindowPic_Click(object sender, EventArgs e)
        {
            showHideNodindowTxt.Text = globalValues.hotkeyManager.allHotkeys.UpdateHotkey(globalValues.hotkeyManager.allHotkeys.hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.hotkeysNames.showHideNodindow.ToString()), globalValues.hotkeyManager.defaultHotkeys.showHideNodindow.keys, globalValues.hotkeyManager.defaultHotkeys.showHideNodindow.modifiers).GetStrHotkey();
            globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.globalHotkeys.ToString(), globalValues.hotkeyManager.allHotkeys.getJsonSave());
        }
    }
}
