using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Medicare.OKCancel
{
    public partial class PopupOkCancel : Form
    {
        public PopupOkCancel()
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

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}