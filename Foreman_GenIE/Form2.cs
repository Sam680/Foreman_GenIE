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
using System.Threading;

namespace Foreman_GenIE
{
    public partial class Form2 : Form
    {
        IWebDriver driver;
        string username;
        string password;
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
                update_Log("TASK:\tBegin.");
                Thread logon = new Thread(login);
                logon.Start();
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

                logBox.Items.Add("TASK:\tStopped.");
                logBox.Items.Add("");
                toggleBtn.Text = "Run";
            }
            
        }

        private void login()
        {
            update_Log("TASK:\tStarting Webdriver...");
            try
            {
                //start webdriver
                driver = new FirefoxDriver();
                driver.Navigate().GoToUrl("https://foreman.ordsvy.gov.uk/users/login");
                update_Log("SUCCESS:\tWebdriver started.");

                //login using credentials
                bool logon_successfull = false;
                IWebElement username_field = driver.FindElement(By.XPath(@"//*[@id='login_login']"));
                IWebElement password_field = driver.FindElement(By.XPath(@"//*[@id='login_password']"));

                update_Log("TASK:\tWaiting for successful Login...");
                while (logon_successfull == false)
                {
                    try
                    {
                        IWebElement hat_in_homescreen = driver.FindElement(By.XPath(@"/html/body/div[1]/div/div/div[1]/img"));
                        logon_successfull = true;
                    }
                    catch (NoSuchElementException)
                    {
                        
                    }
                    catch (WebDriverException)
                    {
                        update_Log("ERROR:\tLost Webdriver.");
                        kill_webdriver();
                        update_Log("TASK:\tRestarting Webdriver.");
                        Thread logon = new Thread(login);
                        logon.Start();
                        return;
                    }
                    catch (InvalidOperationException)
                    {
                        update_Log("ERROR:\tLost Webdriver.");
                        kill_webdriver();
                        update_Log("TASK:\tRestarting Webdriver.");
                        Thread logon = new Thread(login);
                        logon.Start();
                        return;
                    }
                    
                }
                update_Log("SUCCESS:\tLogin successful.");

            }
            catch
            {
                update_Log("ERROR:\tUnable to initiate Webdriver.\n");
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            kill_webdriver();

            this.Close();
        }

        private void update_Log(string text)
        {            
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(update_Log), new object[] { text });
                return;
            }
            logBox.Items.Add(text);
        }

        private void kill_webdriver()
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
        }

        private void Main_Thread()
        {

        }
    }
}
