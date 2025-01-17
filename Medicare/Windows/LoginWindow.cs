using System;
using System.Drawing;
using System.Windows.Forms;
using Medicare.Main;
using Medicare.Utility;

namespace Medicare.Windows
{
    public partial class LoginWindow : Form
    {
        public LoginWindow()
        {
            InitializeComponent();

            initColor();

            msgLabel.Visible = false;
        }

        private void initColor()
        {
            this.BackColor = Color.FromArgb(235, 236, 238);
            msgLabel.ForeColor = Color.FromArgb(0, 54, 105);
            signInButton.BackColor = Color.FromArgb(0, 54, 105);
            signInButton.ForeColor = Color.White;
        }
        private void signInButton_Click(object sender, EventArgs e)
        {
            string userpasswd = passWordTextBox.Text;

            if (userpasswd.Equals(CTPMain.pwd))
            {
                //Util.openPopupOk("성공");
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                //Util.openPopupOk("패스워드가 틀렸습니다");
                msgLabel.Visible = true;
                DialogResult = DialogResult.None;
            }
        }

        public void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                signInButton_Click(sender, e);
        }
    }
}