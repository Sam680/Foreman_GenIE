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

namespace Foreman_GenIE
{
    public partial class Form1 : Form
    {
        public static List<Machine> jobList = new List<Machine>();
        private List<Machine> load_CSV(string path, string catFilter)
        {
            string whole_file = "";
            try
            {
                whole_file = File.ReadAllText(path);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Unable to find '" + path + "'.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (IOException)
            {
                MessageBox.Show("'" + path + "' is being used by another process.\nPlease close all instances.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            whole_file = whole_file.Replace('\n', '\r');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            var Machines = new List<Machine>();

            foreach (string line in lines)
            {
                string[] attribute = line.Split(',');

                if (attribute[0] == catFilter)
                {
                    Machines.Add(new Machine
                    {
                        Name = attribute[1],
                        Catagory = attribute[0],
                        Role = attribute[2],
                        Action = "n/a"

                    });
                }
                else if (catFilter == "All")
                {
                    if (attribute[1] != "Machine")
                    {
                        Machines.Add(new Machine
                        {
                            Name = attribute[1],
                            Catagory = attribute[0],
                            Role = attribute[2],
                            Action = "n/a"

                        });
                    }
                }
            }
            return Machines;
        }

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
            if (envirCb.Text == "GENNFT" || envirCb.Text == "GENPREP" || envirCb.Text == "GENPROD" || envirCb.Text == "GENSYS")
            {
                filterCb.Enabled = true;
                if (filterCb.Text == "")
                {
                    filterCb.Text = "All";
                }

                if (filterCb.Text == "All" || filterCb.Text == "1Generalise DB" || filterCb.Text == "1Generalise" || filterCb.Text == "1Validate" || filterCb.Text == "Editor Framework" || filterCb.Text == "Publication") {
                    failTBar.Enabled = true;
                    submitBtn.Enabled = true;
                    removeBtn.Enabled = true;
                    allBtn.Enabled = true;
                    clearBtn.Enabled = true;
                    actionCb.Enabled = true;
                    runBtn.Enabled = true;
                    filterCb_TextChanged(sender,e);
                    failLbl.Text = "Maximum Fails: " + failTBar.Value;  
                }
                else
                {
                    failTBar.Enabled = false;
                    submitBtn.Enabled = false;
                    removeBtn.Enabled = false;
                    allBtn.Enabled = false;
                    clearBtn.Enabled = false;
                    actionCb.Enabled = false;
                    runBtn.Enabled = false;

                    selectionList.Clear();
                    filterCb.Text = "";
                }
            }
            else
            {
                filterCb.Enabled = false;
                failTBar.Enabled = false;
                submitBtn.Enabled = false;
                removeBtn.Enabled = false;
                allBtn.Enabled = false;
                clearBtn.Enabled = false;
                actionCb.Enabled = false;
                runBtn.Enabled = false;

                selectionList.Clear();
                filterCb.Text = "";
            }
        }

        private void filterCb_TextChanged(object sender, EventArgs e)
        {
            if (filterCb.Text == "All" || filterCb.Text == "1Generalise DB" || filterCb.Text == "1Generalise" || filterCb.Text == "1Validate" || filterCb.Text == "Editor Framework" || filterCb.Text == "Publication")
            {
                failTBar.Enabled = true;
                submitBtn.Enabled = true;
                removeBtn.Enabled = true;
                allBtn.Enabled = true;
                clearBtn.Enabled = true;
                actionCb.Enabled = true;
                runBtn.Enabled = true;

                selectionList.Clear();

                failLbl.Text = "Maximum Fails: " + failTBar.Value;

                List<Machine> filteredList = new List<Machine>();

                if (envirCb.Text == "GENNFT")
                {
                    filteredList = load_CSV(@"C:\Users\stravers\source\repos\Foreman_GenIE\GENNFT.csv", filterCb.Text);
                }
                else if (envirCb.Text == "GENPREP")
                {
                    filteredList = load_CSV(@"C:\Users\stravers\source\repos\Foreman_GenIE\GENPREP.csv", filterCb.Text);
                }
                else if (envirCb.Text == "GENPROD")
                {
                    filteredList = load_CSV(@"C:\Users\stravers\source\repos\Foreman_GenIE\GENPROD.csv", filterCb.Text);
                }
                else if (envirCb.Text == "GENSYS")
                {
                    filteredList = load_CSV(@"C:\Users\stravers\source\repos\Foreman_GenIE\GENSYS.csv", filterCb.Text);
                }

                selectionList.View = View.Details;

                selectionList.Columns.Add("");
                selectionList.Columns.Add("Name");
                selectionList.Columns.Add("Role");

                selectionList.CheckBoxes = true;


                foreach (Machine item in filteredList)
                {
                    selectionList.Items.Add(new ListViewItem(new string[] { "", item.Name, item.Role, item.Action }));
                }

                selectionList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                selectionList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            }
            
            else
            {
                failTBar.Enabled = false;
                submitBtn.Enabled = false;
                removeBtn.Enabled = false;
                allBtn.Enabled = false;
                clearBtn.Enabled = false;
                actionCb.Enabled = false;

                selectionList.Clear();
            }
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
            int numOfJobs = 0;
            foreach (ListViewItem job in listView1.Items)
            {              
                if (job.Checked == true)
                {
                    numOfJobs++;
                    job.Remove();
                }
            }
            if (numOfJobs == 0)
            {
                MessageBox.Show("No jobs selected to be removed.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void submitBtn_Click(object sender, EventArgs e)
        {

            if (actionCb.Text == "Rebuild" || actionCb.Text == "Deploy Puppet" || actionCb.Text == "Restart")
            {
                jobList = new List<Machine>();
                int jobListSize = 0;

                foreach (ListViewItem item in selectionList.Items)
                {
                    bool duplicate = false;
                    if (item.Checked == true)
                    {
                        foreach (ListViewItem job in listView1.Items)
                        {
                            if (job.SubItems[1].Text == item.SubItems[1].Text)
                            {
                                DialogResult result = MessageBox.Show(item.SubItems[1].Text + " already in job list. Replace old job with new one?", "Warning",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                                if (result == DialogResult.Yes)
                                {
                                    duplicate = false;
                                    job.Remove();
                                }
                                else if (result == DialogResult.No)
                                {
                                    duplicate = true;
                                }
                                else if (result == DialogResult.Cancel)
                                {
                                    duplicate = true;
                                }
                            }
                        }
                    }
                    
                    if (item.Checked == true && duplicate == false)
                    {
                        jobList.Add(new Machine
                        {
                            Name = item.SubItems[1].Text,
                            Environment = envirCb.Text,
                            Role = item.SubItems[2].Text,
                            Action = actionCb.Text,
                            Passed = false,
                            MaxFail = failTBar.Value,
                        });

                        jobListSize++;
                    }
                }

                int n = 0;
                foreach (ListViewItem item in listView1.Items)
                {
                    n++;
                }

                if (n == 0)
                {
                    listView1.Clear();
                    listView1.View = View.Details;

                    listView1.Columns.Add("");
                    listView1.Columns.Add("Name");
                    listView1.Columns.Add("Environment");
                    listView1.Columns.Add("Role");
                    listView1.Columns.Add("Action");
                    listView1.Columns.Add("Fails");

                    listView1.CheckBoxes = true;
                }

                if (jobListSize > 0)
                {
                    foreach (Machine item in jobList)
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { "", item.Name, item.Environment, item.Role, item.Action, item.MaxFail.ToString() }));
                    }

                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
                else
                {
                    MessageBox.Show("No machine(s) selected.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("'Select Action' field is invalid.\nChoose an action to perform on the machine(s) before trying to submit.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void puppetBtn_Click(object sender, EventArgs e)
        {

        }

        private void selectionList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void allBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in selectionList.Items)
            {
                item.Checked = true;
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in selectionList.Items)
            {
                item.Checked = false;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = false;
            }
        }

        private void runBtn_Click(object sender, EventArgs e)
        {
            jobList = new List<Machine>();

            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked == true)
                {
                    jobList.Add(new Machine
                    {
                        Name = item.SubItems[1].Text,
                        Environment = item.SubItems[2].Text,
                        Role = item.SubItems[3].Text,
                        Action = item.SubItems[4].Text,
                        Power_State = "?",
                        MaxFail = Convert.ToInt32(item.SubItems[5].Text),
                        Reports = 0,
                        Fails = 0,
                        Skipped = false,
                        First_Run = true,
                        Failure = null
                    });
                }
            }

            int n = 0;
            foreach (Machine item in jobList)
            {
                n++;
            }
           
            if (n > 0)
            {
                Form2 settingsForm = new Form2(this);
                settingsForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("No machine(s) to run.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
    }
}
