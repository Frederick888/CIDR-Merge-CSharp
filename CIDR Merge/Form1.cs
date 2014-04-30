using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace CIDR_Merge
{
    public partial class Form1 : Form
    {
        List<string> bitsList = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        string cidr2bits(string cidr)
        {
            string[] param = cidr.Split(new char[] { '.', '/' });
            string bits = "";
            for (int i = 0; i < 4; i++)
            {
                string tmp = Convert.ToString(Convert.ToInt32(param[i]), 2);
                while (tmp.Length < 8) tmp = "0" + tmp;
                bits += tmp;
            }
            bits = bits.Substring(0, Convert.ToInt32(param[4]));
            return bits;
        }

        string bits2cidr(string bits)
        {
            int len = bits.Length;
            while (bits.Length < 32) bits += "0";
            string cidr = "";
            for (int i = 0; i <= 24; i += 8)
            {
                cidr += Convert.ToInt32(bits.Substring(i, 8), 2).ToString() + ".";
            }
            cidr = cidr.Substring(0, cidr.Length - 1);
            cidr += "/" + len.ToString();
            return cidr;
        }

        void merge(string bits)
        {
            /* whether any CIDR existed is completely same to the one to add */
            foreach (string exist in bitsList)
            {
                if (exist == bits)
                    return;
            }

            /* whether any CIDR existed is smaller than the one to add, if so, delete it */
            foreach (string exist in bitsList)
            {
                if (exist.Length > bits.Length)
                    if (exist.Substring(0, bits.Length) == bits)
                        bitsList.Remove(exist);
            }

            /* whether any CIDR existed can "plus" the one to add, if so, calculate the result, delete the existed item, and finally modify the bits */
            for (int i = 0; i < bitsList.Count; i++)
            {
                if(bitsList[i].Length==bits.Length)
                    if (bitsList[i].Substring(0, bitsList[i].Length - 1) == bits.Substring(0, bits.Length - 1))
                    {
                        bitsList.RemoveAt(i);
                        bits = bits.Substring(0, bits.Length - 1);
                        break;
                    }
            }
            
            /* whether andy CIDR existed is larger than the one to add, if so, abandon the one to add; if not, add it to the list */
            foreach (string exist in bitsList)
            {
                if (exist.Length < bits.Length)
                    if (exist == bits.Substring(0, exist.Length))
                        return;
            }
            bitsList.Add(bits);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            bitsList.Clear();
            textBoxSts.Text = "Processing";
            textBoxSts.Update();
            textBoxIn.Enabled = false;
            textBoxOut.Enabled = false;
            buttonIn.Enabled = false;
            buttonIn.Enabled = false;
            buttonStart.Enabled = false;

            var sr = new StreamReader(textBoxIn.Text, Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                merge(cidr2bits(sr.ReadLine()));
            }
            sr.Close();

            var fs = new FileStream(textBoxOut.Text, FileMode.Create);
            var sw = new StreamWriter(fs, Encoding.UTF8);
            bitsList.Sort();
            foreach (string exist in bitsList)
                sw.WriteLine(bits2cidr(exist));
            sw.Close();
            fs.Close();

            textBoxSts.Text = "Idle";
            textBoxSts.Update();
            textBoxIn.Enabled = true;
            textBoxOut.Enabled = true;
            buttonIn.Enabled = true;
            buttonOut.Enabled = true;
            buttonStart.Enabled = true;
            MessageBox.Show("Finish!");
        }

        private void buttonIn_Click(object sender, EventArgs e)
        {
            var dial = new OpenFileDialog();
            dial.Filter = "Text Files (*.txt)|*.txt";
            if (dial.ShowDialog() == DialogResult.OK)
            {
                textBoxIn.Text = dial.FileName;
            }
        }

        private void buttonOut_Click(object sender, EventArgs e)
        {
            var dial = new SaveFileDialog();
            dial.Filter = "Text Files (*.txt)|*.txt";
            if (dial.ShowDialog() == DialogResult.OK)
            {
                textBoxOut.Text = dial.FileName;
            }
        }
    }
}
