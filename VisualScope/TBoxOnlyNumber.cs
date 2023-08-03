using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 地面站
{
    class TBoxOnlyNumber
    {
        public TBoxOnlyNumber(TextBox tb)
        {
            tb.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(TBox_KeyPress);
            tb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TBox_KeyPress);
            tb.Leave -= new System.EventHandler(TBox_Leave);
            tb.Leave += new System.EventHandler(TBox_Leave);
        }
        private void TBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != '\b' && e.KeyChar != '.'&& e.KeyChar != '-')//只允许输入数字和退格
                e.Handled = true;
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                ((TextBox)sender).Parent.Focus();
            }
        }
        private void TBox_Leave(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToDouble(((TextBox)sender).Text) == 0)
                    ((TextBox)sender).Text = "0";
            }
            catch
            {
                ((TextBox)sender).Text = "0";
            }
        }
    }
}
