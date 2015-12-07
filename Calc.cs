using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using calcType = System.Double;

namespace calculator
{
    class Calc
    {
        /*

        **Class:        计算器类

        **Author:       HuangHui 2015

        **Description:  只需根据一定的格式传入数字与字符，通过调用CalcAll()即可得到正确的结果

        */
        public enum mode { deg, rad, gra }
        public enum radix { hex, dec, oct, bin }
        struct node
        {
            public string data;
            public bool isnum;
        }
        mode _calcMode = mode.rad;
        radix _calcRadix = radix.dec;
        List<node> q;
        Stack symbol;
        Stack number;
        calcType lastAns;
        static Random ran;
        static string[] sym = new string[] { "(", "+", "-", "*", "/", "sin(", "cos(", "tan(", "!", "²", "³", "^(", "^(-1)", "ln(", "log(", "√(", "³√(", "ⁿ√(", "10^(", "e^(", "Int(", "Not(", "or", "and", "xor", "mod" };
        static int[] prior = new int[] { 100, 20, 20, 22, 22, 40, 40, 40, 50, 45, 45, 45, 45, 40, 40, 45, 45, 45, 50, 50, 55, 16, 10, 14, 12, 35 };
        static bool[] _isDyadic = new bool[] { false, true, true, true, true, false, false, false, false, false, false, true, false, false, false, false, false, true, false, false, false, false, true, true, true, true };
        static bool[] _isPost = new bool[] { false, false, false, false, false, false, false, false, true, true, true, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false };
        static bool[] addBracket = new bool[] { false, false, false, false, false, true, true, true, false, false, false, true, false, true, true, true, true, true, true, true, true, true, false, false, false, false };
        static int[] radixBase = new int[] { 16, 10, 8, 2 };
        public radix calcRadix
        {
            set
            {
                _calcRadix = value;
            }
        }
        public mode calcMode
        {
            set
            {
                _calcMode = value;
            }
        }
        public Calc()
        {
            symbol = new Stack();
            number = new Stack();
            ran = new Random();
            q = new List<node>();
        }
        public static bool isPost(int id)
        {
            return id == -1 ? true : _isPost[id];
        }
        public static bool isDyadic(int id)
        {
            return id == -1 ? false : _isDyadic[id];
        }
        public static string op(int id)
        {
            if (id == -1) return ")";
            return sym[id];
        }
        public static double random()
        {
            return ran.NextDouble();
        }
        public void PopLastOp()
        {
            q.RemoveAt(q.Count - 1);
        }
        public void PushNum(string s)
        {
            switch (s)
            {
                case "π":
                    s = Math.PI.ToString();
                    break;
                case "e":
                    s = Math.E.ToString();
                    break;
                case "Ans":
                    s = lastAns.ToString();
                    break;
                default:
                    radix r = _calcRadix;
                    _calcRadix = radix.dec;
                    if (r != radix.dec) { s = radixlize(s, r); }
                    _calcRadix = r;
                    break;
            }
            node t;
            t.data = s;
            t.isnum = true;
            q.Add(t);
        }
        public void PushOp(string id)
        {
            node t;
            t.data = id;
            t.isnum = false;
            q.Add(t);
        }
        void getone(out calcType x)
        {
            x = 0;
            try
            {
                x = (calcType)number.Pop();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        void gettwo(out calcType x, out calcType y)
        {
            x = y = 0;
            try
            {
                getone(out x);
                getone(out y);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DoOp(int id, out calcType ans)
        {
            calcType x, y;
            ans = 0;
            try
            {
                switch (id)
                {
                    case 1:
                        gettwo(out x, out y);
                        ans = x + y;
                        break;
                    case 2:
                        gettwo(out x, out y);
                        ans = y - x;
                        break;
                    case 3:
                        gettwo(out x, out y);
                        ans = x * y;
                        break;
                    case 4:
                        gettwo(out x, out y);
                        ans = y / x;
                        break;
                    case 5:
                        getone(out x);
                        ans = Math.Sin(radBase(x));
                        break;
                    case 6:
                        getone(out x);
                        ans = Math.Cos(radBase(x));
                        break;
                    case 7:
                        getone(out x);
                        ans = Math.Tan(radBase(x));
                        break;
                    case 8:
                        getone(out x);
                        ans = 1;
                        if (x >= 10000) ans = 1.0 / 0;
                        else { for (int i = 1; i <= x; i++) { ans *= i; } }
                        break;
                    case 9:
                        getone(out x);
                        ans = x * x;
                        break;
                    case 10:
                        getone(out x);
                        ans = x * x * x;
                        break;
                    case 11:
                        gettwo(out x, out y);
                        ans = Math.Pow(y, x);
                        break;
                    case 12:
                        getone(out x);
                        ans = 1.0 / x;
                        break;
                    case 13:
                        getone(out x);
                        ans = Math.Log(x);
                        break;
                    case 14:
                        getone(out x);
                        ans = Math.Log10(x);
                        break;
                    case 15:
                        getone(out x);
                        ans = Math.Sqrt(x);
                        break;
                    case 16:
                        getone(out x);
                        ans = Math.Pow(x, 1f / 3);
                        break;
                    case 17:
                        gettwo(out x, out y);
                        ans = Math.Pow(x, 1f / y);
                        break;
                    case 18:
                        getone(out x);
                        ans = Math.Pow(10, x);
                        break;
                    case 19:
                        getone(out x);
                        ans = Math.Pow(Math.E, x);
                        break;
                    case 20:
                        getone(out x);
                        ans = (int)x;
                        break;
                    case 21:
                        getone(out x);
                        ans = ~(int)x;
                        break;
                    case 22:
                        gettwo(out x, out y);
                        ans = (int)x | (int)y;
                        break;
                    case 23:
                        gettwo(out x, out y);
                        ans = (int)x & (int)y;
                        break;
                    case 24:
                        gettwo(out x, out y);
                        ans = (int)x ^ (int)y;
                        break;
                    case 25:
                        gettwo(out x, out y);
                        ans = y % x;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string radixlize(string orig,radix origRadix)
        {
            if (origRadix != radix.dec)
            {
                int r = radixBase[(int)origRadix];
                try
                {
                    orig = Convert.ToInt64(orig, r).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return "0";
                }
                
            }
            else
            {
                try {
                    orig = ((long)calcType.Parse(orig)).ToString();
                }
                catch
                {
                    return "0";
                }
            }
            return Convert.ToString(long.Parse(orig), radixBase[(int)_calcRadix]);
        }
        calcType radBase(calcType d)
        {
            if (_calcMode == mode.deg) { d /= 180 / Math.PI; }
            else if (_calcMode == mode.gra) { d /= 200 / Math.PI; }
            return d;
        }
        public void CalcAll(out string _ans)
        {
            for (int i = 0; i < q.Count; i++)
            {
                if (q[i].isnum) { number.Push(calcType.Parse(q[i].data)); }
                else
                {
                    int id = int.Parse(q[i].data);
                    if (id == -1)//右括号
                    {
                        CalcToEnd();
                    }
                    else
                    {
                        if (symbol.Count != 0)
                        {
                            int pid = (int)symbol.Peek();
                            while (prior[id] <= prior[pid] && pid != 0)
                            {
                                symbol.Pop();
                                calcType t;
                                try
                                {
                                    DoOp(pid, out t);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }

                                number.Push(t);
                                if (symbol.Count == 0) break;
                                pid = (int)symbol.Peek();
                            }
                        }
                        symbol.Push(id);
                        if (addBracket[id])
                        {
                            symbol.Push(0);
                        }
                    }
                }
            }
            while (symbol.Count != 0)
            {
                try
                {
                    CalcToEnd();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            try
            {
                lastAns = (calcType)number.Pop();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (number.Count != 0) { throw new Exception("堆栈错误,请检查是否省略了任何操作符!"); }
            reset();
            if (Math.Abs(lastAns) < 1e-14) { lastAns = 0; }//消除极小误差
            _ans = lastAns.ToString();
            if (_calcRadix != radix.dec) { _ans = radixlize(_ans, radix.dec); }
            
        }
        public bool CalcToEnd()
        {
            bool ret = true;
            while (ret && symbol.Count != 0)
            {
                int id = (int)symbol.Pop();
                if (id == 0) { break; }
                calcType t;
                try
                {
                    DoOp(id, out t);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                number.Push(t);
            }
            return ret;
        }
        public void reset()
        {
            q.Clear();
            symbol.Clear();
            number.Clear();
        }
    }
}
