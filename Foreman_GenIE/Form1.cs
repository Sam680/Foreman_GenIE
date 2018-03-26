using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LMWRCM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public DataTable ReadCsv(string fileName)
        {
            DataTable dt = new DataTable("Data");
            return dt;
        }
        private void envirCb_TextChanged(object sender, EventArgs e)
        {
            if(envirCb.Text == "GENNFT" || envirCb.Text == "GENPREP" || envirCb.Text == "GENPROD" || envirCb.Text == "GENSYS")
            {
                filterCb.Enabled = true;
                if (filterCb.Text == "All" || filterCb.Text == "1Generalise DB" || filterCb.Text == "1Generalise" || filterCb.Text == "1Validate" || filterCb.Text == "Editor Framework") {
                    passTBar.Enabled = true;
                    failTBar.Enabled = true;
                    rebuiltBtn.Enabled = true;
                    puppetBtn.Enabled = true;

                    passLbl.Text = "Minimum Passes: " + passTBar.Value;
                    failLbl.Text = "Maximum Fails: " + failTBar.Value;

                    if (filterCb.Text == "All")
                    {

                    }
                }
                else
                {
                    passTBar.Enabled = false;
                    failTBar.Enabled = false;
                    rebuiltBtn.Enabled = false;
                    puppetBtn.Enabled = false;
                }
            }
            else
            {
                filterCb.Enabled = false;
                passTBar.Enabled = false;
                failTBar.Enabled = false;
                rebuiltBtn.Enabled = false;
                puppetBtn.Enabled = false;
            }
        }

        private void filterCb_TextChanged(object sender, EventArgs e)
        {
            if (filterCb.Text == "All" || filterCb.Text == "1Generalise DB" || filterCb.Text == "1Generalise" || filterCb.Text == "1Validate" || filterCb.Text == "Editor Framework")
            {
                passTBar.Enabled = true;
                failTBar.Enabled = true;
                rebuiltBtn.Enabled = true;
                puppetBtn.Enabled = true;

                passLbl.Text = "Minimum Passes: " + passTBar.Value;
                failLbl.Text = "Maximum Fails: " + failTBar.Value;
            }
            else
            {
                passTBar.Enabled = false;
                failTBar.Enabled = false;
                rebuiltBtn.Enabled = false;
                puppetBtn.Enabled = false;
            }
        }

        private void passTBar_Scroll(object sender, EventArgs e)
        {
            passLbl.Text = "Minimum Passes: " + passTBar.Value;
        }

        private void failTBar_Scroll(object sender, EventArgs e)
        {
            failLbl.Text = "Maximum Fails: " + failTBar.Value;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
