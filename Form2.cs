using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FutureMonitor
{
    public partial class Form2 : Form
    {
        public double cost = 0;
        public int accuracy = 0;
        public string name;
        public int isLong = 1;
        public Form2()
        {
            InitializeComponent();
        }

        private void setValue(string key, Object value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("data.xml");
            XmlElement root = (XmlElement)xmlDoc.SelectSingleNode("app");//查找<bookstore>
            if (root == null)
                return;
            root.SetAttribute(key, value.ToString());
            xmlDoc.Save("data.xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                name = textBoxName.Text.Trim().ToUpper();
                accuracy = comboBoxAccuracy.SelectedIndex;
                isLong = comboBoxIsLong.SelectedIndex == 0 ? 1 : -1;
                cost = Double.Parse(textBoxCost.Text.Trim());
                setValue("name", name);
                setValue("cost", cost);
                setValue("accuracy", accuracy);
                setValue("isLong", isLong);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
