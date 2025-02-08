using System;
using System.Windows.Forms;
using System.Drawing;

namespace msprint.src
{
    public class wrapperForm : Form
    {
        TextBox tb1;
        Button btn1;
        public wrapperForm()
        {
            this.Text = "Wrapper";
            this.ClientSize = new Size(500, 400);
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;
            this.TopMost = false;
            this.Visible = true;

            this.Controls.Add(tb1 = new TextBox
            {
                Text = "Hello World",
                Location = new Point(10, 10)
            });

            tb1.TextChanged += tb1_TextChanged;
        }

        void tb1_TextChanged(object sender, EventArgs e)
        {
            if(tb1.Text.Length != 16)
            {
                return;
            }
            try {
                var text = main.ValidateAndParse(tb1.Text);
                var data = main.GetDataListFromInt(text, main.WPQuery());
                var result = main.FilteredID(data);
                main.CallSetResult(result);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}