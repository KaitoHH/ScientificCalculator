using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace calculator
{
    public partial class Form1 : Form
    {
        bool inDot;
        Calc calc;
        bool toClear;
        bool lastIsSymbol;
        List<Button> buttonOp;
        List<Button> buttonHexNumber;
        List<Button> buttonDecNumber;
        List<Button> buttonOctNumber;
        List<Button> buttonBinNumber;
        string lastText;            //备份上一次的文本
        int lastId = 0;             //上一个符号
        int lastRadix = 1;          //上一个进制
        public Form1()
        {
            InitializeComponent();
            calc = new Calc();
            reset();
            initControl();
            Text = "科学计算器 V1.0  By HuangHui";
        }

        private void AddNumber(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string str = (string)btn.Tag;
            _AddNumber(str);

        }

        private void AddOp(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            _AddOp((string)btn.Tag);
        }

        private void numScreen_TextChanged(object sender, EventArgs e)
        {
            inDot = numScreen.Text.IndexOf('.') != -1;
            if (numScreen.Text.Length > 26)
            {
                numScreen.Text = numScreen.Text.Substring(0, numScreen.Text.Length - 1);
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!inDot && ch == '.' || ch >= '0' && ch <= '9')
            {
                _AddNumber(ch.ToString());
            }
            else
            {
                switch (e.KeyChar)
                {
                    case '+':
                        _AddOp("1");
                        break;
                    case '-':
                        _AddOp("2");
                        break;
                    case '*':
                        _AddOp("3");
                        break;
                    case '/':
                        _AddOp("4");
                        break;
                    case '(':
                        _AddOp("0");
                        break;
                    case ')':
                        _AddOp("-1");
                        break;
                    case '\b':
                        del();
                        break;
                }
            }
        }
        private void GetAns(object sender, EventArgs e)
        {
            if (!Calc.isPost(lastId)) calc.PushNum(numScreen.Text);
            string ans;
            try
            {
                calc.CalcAll(out ans);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ans = "0";
            }
            numScreen.Text = ans;
            calc.reset();
            reset();
        }

        private void _AddNumber(string s)
        {
            if (toClear)
            {
                numScreen.Text = "";
                toClear = false;
            }
            if (!inDot || s != ".")
            {
                numScreen.Text += s;
            }
            if (numScreen.Text == ".") { numScreen.Text = "0."; }
            lastIsSymbol = false;
        }

        private void _AddOp(string id)
        {
            int cid = int.Parse(id);
            if (lastIsSymbol && Calc.isDyadic(lastId) && Calc.isDyadic(cid))
            {
                textBox1.Text = lastText;
                calc.PopLastOp();
            }
            else if (!lastIsSymbol && (Calc.isDyadic(cid) || cid == -1) || Calc.isPost(cid) && cid != -1 && lastId != -1)
            {
                textBox1.AppendText(numScreen.Text);
                calc.PushNum(numScreen.Text);
            }
            lastText = textBox1.Text;
            calc.PushOp(id);
            textBox1.AppendText(Calc.op(cid));
            lastIsSymbol = true;
            lastId = cid;
            toClear = true;
        }

        private void resetFocus(object sender, EventArgs e)
        {
            button12.Focus();

        }

        private void reset()
        {
            textBox1.Text = "";
            lastIsSymbol = false;
            toClear = true;
            inDot = false;
        }

        private void ClearAll(object sender, EventArgs e)
        {
            numScreen.Text = "0";
            reset();
            calc.reset();

        }
        private void del()
        {
            numScreen.Text = numScreen.Text.Remove(numScreen.Text.Length - 1);
            if (numScreen.Text == "")
            {
                toClear = true;
                numScreen.Text = "0";
            }
        }

        private void Delete(object sender, EventArgs e)
        {
            del();
        }

        private void SetNumber(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string str = (string)btn.Tag;
            switch (str)
            {
                case "ran":
                    str = Calc.random().ToString();
                    break;
            }
            numScreen.Text = str;
            toClear = true;
            lastIsSymbol = false;
        }

        private void Opp(object sender, EventArgs e)
        {
            numScreen.Text = numScreen.Text.IndexOf('-') == -1 ? "-" + numScreen.Text : numScreen.Text.Substring(1);
            lastIsSymbol = false;
        }

        private void About(object sender, EventArgs e)
        {
            About dlg = new About();
            dlg.ShowDialog();
        }

        private void modeChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (btn.Checked)
            {
                calc.calcMode = (Calc.mode)btn.TabIndex;
            }

        }

        private void radixChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (btn.Checked)
            {
                calc.calcRadix = (Calc.radix)btn.TabIndex;
                reset();
                numScreen.Text = calc.radixlize(numScreen.Text, (Calc.radix)lastRadix);
                lastRadix = btn.TabIndex;
                setControl(buttonOp, false);
                setControl(buttonHexNumber, false);
                switch (btn.TabIndex)
                {
                    case 0:
                        setControl(buttonHexNumber, true);
                        break;
                    case 1:
                        setControl(buttonDecNumber, true);
                        setControl(buttonOp, true);
                        break;
                    case 2:
                        setControl(buttonOctNumber, true);
                        break;
                    case 3:
                        setControl(buttonBinNumber, true);
                        break;
                }
                calc.reset();
            }
        }
        private void setControl(List<Button> btnList,bool flags)
        {
            for(int i = 0; i < btnList.Count; i++)
            {
                btnList[i].Enabled = flags;
            }
        }
        private void initControl()
        {
            buttonOp = new List<Button>();
            buttonOp.Add(button11);
            buttonOp.Add(button21);
            buttonOp.Add(button22);
            buttonOp.Add(button23);
            buttonOp.Add(button24);
            buttonOp.Add(button25);
            buttonOp.Add(button26);
            buttonOp.Add(button27);
            buttonOp.Add(button28);
            buttonOp.Add(button29);
            buttonOp.Add(button30);
            buttonOp.Add(button31);
            buttonOp.Add(button32);
            buttonOp.Add(button33);
            buttonOp.Add(button34);
            buttonOp.Add(button35);
            buttonOp.Add(button36);
            buttonOp.Add(button37);
            buttonOp.Add(button47);
            buttonOp.Add(button38);
            buttonOp.Add(button45);
            buttonOp.Add(button53);
            buttonHexNumber = new List<Button>();
            buttonHexNumber.Add(button1);
            buttonHexNumber.Add(button2);
            buttonHexNumber.Add(button3);
            buttonHexNumber.Add(button4);
            buttonHexNumber.Add(button5);
            buttonHexNumber.Add(button6);
            buttonHexNumber.Add(button7);
            buttonHexNumber.Add(button8);
            buttonHexNumber.Add(button9);
            buttonHexNumber.Add(button10);
            buttonHexNumber.Add(button39);
            buttonHexNumber.Add(button40);
            buttonHexNumber.Add(button41);
            buttonHexNumber.Add(button42);
            buttonHexNumber.Add(button43);
            buttonHexNumber.Add(button44);
            buttonDecNumber = new List<Button>();
            buttonDecNumber.Add(button1);
            buttonDecNumber.Add(button2);
            buttonDecNumber.Add(button3);
            buttonDecNumber.Add(button4);
            buttonDecNumber.Add(button5);
            buttonDecNumber.Add(button6);
            buttonDecNumber.Add(button7);
            buttonDecNumber.Add(button8);
            buttonDecNumber.Add(button9);
            buttonDecNumber.Add(button10);
            buttonOctNumber = new List<Button>();
            buttonOctNumber.Add(button1);
            buttonOctNumber.Add(button2);
            buttonOctNumber.Add(button3);
            buttonOctNumber.Add(button4);
            buttonOctNumber.Add(button5);
            buttonOctNumber.Add(button6);
            buttonOctNumber.Add(button7);
            buttonOctNumber.Add(button10);
            buttonBinNumber = new List<Button>();
            buttonBinNumber.Add(button1);
            buttonBinNumber.Add(button10);
        }

    }
}
