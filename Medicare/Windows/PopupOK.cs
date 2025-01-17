using System;
using System.Drawing;
using System.Windows.Forms;

namespace Medicare.OK
{
    public partial class PopupOK : Form
    {
        public PopupOK()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        public void setPopupMsg(string s)
        {
            this.popupMSg.Text = s;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}