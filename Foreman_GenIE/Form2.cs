using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using System.Diagnostics;

namespace Foreman_GenIE
{
    public partial class Form2 : Form
    {
        IWebDriver driver;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            progressView.View = View.Details;
            passView.View = View.Details;
            failView.View = View.Details;

            progressView.Columns.Add("Name");
            progressView.Columns.Add("Environment");
            progressView.Columns.Add("Role");
            progressView.Columns.Add("Action");
            progressView.Columns.Add("Passes");
            progressView.Columns.Add("Fails");

            foreach (Machine item in Form1.jobList)
            {
                string passes = item.Passes.ToString() + "/" + item.MinPass.ToString();
                string fails = item.Fails.ToString() + "/" + item.MaxFail.ToString();
                progressView.Items.Add(new ListViewItem(new string[] { item.Name, item.Catagory, item.Role, item.Action, passes, fails }));
            }

            progressView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            progressView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            passView.Columns.Add("Name");
            passView.Columns.Add("Environment");
            passView.Columns.Add("Role");
            passView.Columns.Add("Action Performed");

            passView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            passView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            failView.Columns.Add("Name");
            failView.Columns.Add("Environment");
            failView.Columns.Add("Role");
            failView.Columns.Add("Action Performed");

            failView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            failView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void toggleBtn_Click(object sender, EventArgs e)
        {
            if (toggleBtn.Text == "Run")
            {               
                logBox.Items.Add("TASK: Begin.");
                login();
                toggleBtn.Text = "Cancel";
            }
            else if(toggleBtn.Text == "Cancel")
            {
                try
                {
                    driver.Close();
                }
                catch { }
                
                try
                {
                    foreach (Process proc in Process.GetProcessesByName("geckodriver"))
                    {
                        proc.Kill();
                    }
                }
                catch { }

                logBox.Items.Add("TASK: Stopped.");
                logBox.Items.Add("");
                toggleBtn.Text = "Run";
            }
            
        }

        private void login()
        {
            logBox.Items.Add("TASK: Starting Webdriver...");
            try
            {
                driver = new FirefoxDriver();
                driver.Navigate().GoToUrl("https://foreman.ordsvy.gov.uk/users/login");
                logBox.Items.Add("SUCCESS: Webdriver started.");
            }
            catch
            {
                logBox.Items.Add("ERROR: Unable to initiate Webdriver.\n");
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                driver.Close();
            }
            catch {  }

            try
            {
                foreach (Process proc in Process.GetProcessesByName("geckodriver"))
                {
                    proc.Kill();
                }
            }
            catch { }

            this.Close();
        }
    }
}
