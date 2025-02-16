using System;
using System.Drawing;
using System.Windows.Forms;

namespace Nodindow.myPackages.formTools
{
    public partial class tools : Form
    {
        public object formClass { get; set; }
        public tools(toolsType toolsType, object formClass)
        {
            InitializeComponent();
            switch (toolsType)
            {
                case toolsType.Label1Textbox1Button2:
                    this.formClass = formClass;
                    Label1Textbox1Button2 label1Textbox2Button = (Label1Textbox1Button2)this.formClass;
                    label1Textbox2Button.thisForm = this;
                    label1Textbox2Button.label1.Location = new Point(9, 9);
                    label1Textbox2Button.label1.Size = new Size(150, 13);
                    label1Textbox2Button.TextBox.Location = new Point(12, 27);
                    label1Textbox2Button.TextBox.Size = new Size(165, 20);
                    label1Textbox2Button.buttonCancel.Location = new Point(12, 53);
                    label1Textbox2Button.buttonCancel.Size = new Size(75, 23);
                    label1Textbox2Button.buttonReady.Location = new Point(102, 53);
                    label1Textbox2Button.buttonReady.Size = new Size(75, 23);
                    this.Size = new Size(187, 81);
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.Text = label1Textbox2Button.formText;
                    this.Controls.Add(label1Textbox2Button.label1);
                    this.Controls.Add(label1Textbox2Button.TextBox);
                    this.Controls.Add(label1Textbox2Button.buttonCancel);
                    this.Controls.Add(label1Textbox2Button.buttonReady);
                    break;
                case toolsType.Label2Textbox2Button2:
                    this.formClass = formClass;
                    Label2Textbox2Button2 Label1Textbox2Button2 = (Label2Textbox2Button2)this.formClass;
                    Label1Textbox2Button2.thisForm = this;
                    Label1Textbox2Button2.label1.Location = new Point(9, 9);
                    Label1Textbox2Button2.label1.Size = new Size(150, 13);
                    Label1Textbox2Button2.TextBox1.Location = new Point(12, 27);
                    Label1Textbox2Button2.TextBox1.Size = new Size(165, 20);

                    Label1Textbox2Button2.label2.Location = new Point(9, 60);
                    Label1Textbox2Button2.label2.Size = new Size(150, 13);
                    Label1Textbox2Button2.TextBox2.Location = new Point(12, 78);
                    Label1Textbox2Button2.TextBox2.Size = new Size(165, 20);

                    Label1Textbox2Button2.buttonCancel.Location = new Point(12, 113);
                    Label1Textbox2Button2.buttonCancel.Size = new Size(75, 23);
                    Label1Textbox2Button2.buttonReady.Location = new Point(102, 113);
                    Label1Textbox2Button2.buttonReady.Size = new Size(75, 23);
                    this.Size = new Size(187, 140);
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.Text = Label1Textbox2Button2.formText;
                    this.Controls.Add(Label1Textbox2Button2.label1);
                    this.Controls.Add(Label1Textbox2Button2.TextBox1);
                    this.Controls.Add(Label1Textbox2Button2.label2);
                    this.Controls.Add(Label1Textbox2Button2.TextBox2);
                    this.Controls.Add(Label1Textbox2Button2.buttonCancel);
                    this.Controls.Add(Label1Textbox2Button2.buttonReady);
                    break;
                case toolsType.Label2Textbox2Button2Checkbox1:
                    this.formClass = formClass;
                    Label2Textbox2Button2Checkbox1 Label2Textbox2Button2Checkbox1 = (Label2Textbox2Button2Checkbox1)this.formClass;
                    Label2Textbox2Button2Checkbox1.thisForm = this;
                    Label2Textbox2Button2Checkbox1.label1.Location = new Point(9, 9);
                    Label2Textbox2Button2Checkbox1.label1.Size = new Size(150, 13);
                    Label2Textbox2Button2Checkbox1.TextBox1.Location = new Point(12, 27);
                    Label2Textbox2Button2Checkbox1.TextBox1.Size = new Size(165, 20);

                    Label2Textbox2Button2Checkbox1.label2.Location = new Point(9, 60);
                    Label2Textbox2Button2Checkbox1.label2.Size = new Size(150, 13);
                    Label2Textbox2Button2Checkbox1.TextBox2.Location = new Point(12, 78);
                    Label2Textbox2Button2Checkbox1.TextBox2.Size = new Size(165, 20);

                    Label2Textbox2Button2Checkbox1.CheckBox1.Location = new Point(12, 105);
                    Label2Textbox2Button2Checkbox1.CheckBox1.Size = new Size(80, 17);

                    Label2Textbox2Button2Checkbox1.buttonCancel.Location = new Point(12, 123);
                    Label2Textbox2Button2Checkbox1.buttonCancel.Size = new Size(75, 23);

                    Label2Textbox2Button2Checkbox1.buttonReady.Location = new Point(102, 123);
                    Label2Textbox2Button2Checkbox1.buttonReady.Size = new Size(75, 23);


                    this.Size = new Size(187, 150);
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.Text = Label2Textbox2Button2Checkbox1.formText;
                    this.Controls.Add(Label2Textbox2Button2Checkbox1.label1);
                    this.Controls.Add(Label2Textbox2Button2Checkbox1.TextBox1);
                    this.Controls.Add(Label2Textbox2Button2Checkbox1.label2);
                    this.Controls.Add(Label2Textbox2Button2Checkbox1.TextBox2);
                    this.Controls.Add(Label2Textbox2Button2Checkbox1.CheckBox1);
                    this.Controls.Add(Label2Textbox2Button2Checkbox1.buttonCancel);
                    this.Controls.Add(Label2Textbox2Button2Checkbox1.buttonReady);
                    break;
            }
        }
        public enum toolsType
        {
            Label1Textbox1Button2,
            Label2Textbox2Button2,
            Label2Textbox2Button2Checkbox1
        }
        public class Label1Textbox1Button2
        {
            public Form thisForm { get; set; }
            public Label label1 = new Label();
            public TextBox TextBox = new TextBox();
            public Button buttonCancel = new Button();
            public Button buttonReady = new Button();
            public string formText = "New";
            public bool Ready = false;
            public Label1Textbox1Button2(string label1Text = "name", string textbox1Text = "", string button1Text = "cancel", string button2Text = "Ready")
            {
                this.TextBox.Text = textbox1Text;
                TextBox.KeyDown += TextBox_KeyDown;
                this.label1.Text = label1Text;
                this.buttonCancel.Text = button1Text;
                this.buttonReady.Text = button2Text;
                buttonCancel.Click += new EventHandler(buttonCancel_Click);
                buttonReady.Click += new EventHandler(buttonReady_Click);

            }
            private void buttonCancel_Click(object sender, EventArgs e)
            {
                thisForm.Close();
            }
            private void buttonReady_Click(object sender, EventArgs e)
            {
                Ready = true;
                thisForm.Close();
            }
            public void TextBox_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Ready = true;
                    thisForm.Close();
                }
            }
        }
        public class Label2Textbox2Button2
        {
            public Form thisForm { get; set; }
            public Label label1 = new Label();
            public Label label2 = new Label();
            public TextBox TextBox1 = new TextBox();
            public TextBox TextBox2 = new TextBox();
            public Button buttonCancel = new Button();
            public Button buttonReady = new Button();
            public string formText = "New";
            public bool Ready = false;
            public Label2Textbox2Button2(string label1Text = "name", string label2Text = "text", string textbox1Text = "", string textbox2Text = "", string button1Text = "Cancel", string button2Text = "Ready")
            {
                this.TextBox1.Text = textbox1Text;
                this.TextBox2.Text = textbox2Text;
                TextBox1.KeyDown += TextBox_KeyDown;
                TextBox2.KeyDown += TextBox_KeyDown;
                this.label1.Text = label1Text;
                this.label2.Text = label2Text;
                this.buttonCancel.Text = button1Text;
                this.buttonReady.Text = button2Text;
                buttonCancel.Click += new EventHandler(buttonCancel_Click);
                buttonReady.Click += new EventHandler(buttonReady_Click);

            }
            private void buttonCancel_Click(object sender, EventArgs e)
            {
                thisForm.Close();
            }
            private void buttonReady_Click(object sender, EventArgs e)
            {
                Ready = true;
                thisForm.Close();
            }
            public void TextBox_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Ready = true;
                    thisForm.Close();
                }
            }
        }
        public class Label2Textbox2Button2Checkbox1
        {
            public Form thisForm { get; set; }
            public Label label1 = new Label();
            public Label label2 = new Label();
            public TextBox TextBox1 = new TextBox();
            public TextBox TextBox2 = new TextBox();
            public Button buttonCancel = new Button();
            public Button buttonReady = new Button();
            public CheckBox CheckBox1 = new CheckBox();
            public string formText = "New";
            public bool Ready = false;
            public Label2Textbox2Button2Checkbox1(string label1Text = "name", string label2Text = "text", string textbox1Text = "", string textbox2Text = "", string button1Text = "Cancel", string button2Text = "Ready", string checkbox1Text = "Open in project", bool checkbox1Checked = true)
            {
                this.TextBox1.Text = textbox1Text;
                this.TextBox2.Text = textbox2Text;
                TextBox1.KeyDown += TextBox_KeyDown;
                TextBox2.KeyDown += TextBox_KeyDown;
                this.label1.Text = label1Text;
                this.label2.Text = label2Text;
                this.buttonCancel.Text = button1Text;
                this.buttonReady.Text = button2Text;
                buttonCancel.Click += new EventHandler(buttonCancel_Click);
                buttonReady.Click += new EventHandler(buttonReady_Click);
                CheckBox1.Text = checkbox1Text;
                CheckBox1.Checked = checkbox1Checked;

            }
            private void buttonCancel_Click(object sender, EventArgs e)
            {
                thisForm.Close();
            }
            private void buttonReady_Click(object sender, EventArgs e)
            {
                Ready = true;
                thisForm.Close();
            }
            public void TextBox_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Ready = true;
                    thisForm.Close();
                }
            }
        }

        private void tools_Load(object sender, EventArgs e)
        {

        }

        private void tools_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
