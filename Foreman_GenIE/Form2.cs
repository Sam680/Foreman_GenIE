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
        bool cancelled = true;
        static List<Machine> jobList = new List<Machine>();
        static List<Machine> passedList = new List<Machine>();
        static List<Machine> failedList = new List<Machine>();

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Form2.CheckForIllegalCrossThreadCalls = false;
            progressView.View = View.Details;
            passView.View = View.Details;
            failView.View = View.Details;

            progressView.Columns.Add("Name");
            progressView.Columns.Add("Environment");
            progressView.Columns.Add("Role");
            progressView.Columns.Add("Action");
            progressView.Columns.Add("State");
            progressView.Columns.Add("Passes");
            progressView.Columns.Add("Fails");

            jobList.Clear();
            foreach (Machine item in Form1.jobList)
            {
                string passes = item.Passes.ToString() + "/" + item.MinPass.ToString();
                string fails = item.Fails.ToString() + "/" + item.MaxFail.ToString();
                progressView.Items.Add(new ListViewItem(new string[] { item.Name, item.Environment, item.Role, item.Action, item.Power_State, passes, fails }));
                jobList.Add(item);
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
            failView.Columns.Add("Action Attempted");
            failView.Columns.Add("Fails");
            failView.Columns.Add("Failure");

            failView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            failView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void toggleBtn_Click(object sender, EventArgs e)
        {
            if (toggleBtn.Text == "Run")
            {
                Thread MainThread = new Thread(Main_Thread);
                MainThread.Start();
                toggleBtn.Text = "Cancel";
                cancelled  = false;
                
            }
            else if(toggleBtn.Text == "Cancel")
            {
                cancelled  = true;
                kill_webdriver();

                logBox.Items.Add("TASK:\tStopped.");
                logBox.Items.Add("");
                toggleBtn.Text = "Run";
            }
            
        }

        private void Main_Thread()
        {
            update_Log("TASK:\tBegin.");


            bool login_successful = false;
            int n = 0;

            //login
            while (login_successful == false && cancelled == false)
            {
                if (toggleBtn.Text == "Run")
                {
                    cancelled  = true;
                }
                else
                {
                    login_successful = Login();
                }
            }

            //main loop
            n = num_of_jobs();
            while (n != 0 && cancelled  == false)
            {
                foreach (Machine item in jobList)
                {
                    if (cancelled  == true)
                    {
                        break;
                    }
                    //open foreman page
                    string url = "https://foreman.ordsvy.gov.uk/hosts/" + item.Name + ".ordsvy.gov.uk";
                    try
                    {
                        driver.Navigate().GoToUrl(url);

                        int count = 0;
                        IWebElement username_field = driver.FindElement(By.XPath(@"//*[@id='title_action']/div/div[3]/a"));
                        while (username_field.Text.Contains("Power") == false)
                        {
                            count++;
                            if (count > 80)
                            {
                                update_Log("ERROR:\tTimed out " + item.Name + " Foreman page.");
                                update_Log("Task:\tSkipped.");
                                break;
                            }
                            Thread.Sleep(250);
                            username_field = driver.FindElement(By.XPath(@"//*[@id='title_action']/div/div[3]/a"));
                        }

                    }
                    catch
                    {
                        if (cancelled  == false)
                        { 
                            try
                            {
                                String host_not_found = driver.FindElement(By.XPath(@"/html/body/div/div/div[2]/div/strong")).Text;
                                if (host_not_found == "Host not found")
                                {
                                    update_Log("ERROR:\t" + item.Name + " Foreman page not found.");
                                    failedList.Add(item);
                                    failView.Items.Add(new ListViewItem(new string[] { item.Name, item.Environment, item.Role, item.Action, item.Fails.ToString(), "page not found"}));
                                    failView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                                    failView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                                    for (int x = 0; x < progressView.Items.Count; x++)
                                    {
                                        if (progressView.Items[x].SubItems[0].Text == item.Name)
                                        {
                                            progressView.Items[x].Remove();
                                        }
                                    }
                                    jobList.Remove(item);
                                    update_Log("TASK:\tMoved " + item.Name + " to Failed.");
                                    break;
                                }

                            }
                            catch
                            {
                                Thread.Sleep(1000);
                            }
                            
                            
                        }
                    }

                    //check power state
                    int i = 0;
                    while (i < 100)     //wait up to 25 seconds
                    {
                        i++;
                        try
                        {
                            IWebElement pwr_btn_state = driver.FindElement(By.XPath(@"//*[@id='title_action']/div/div[3]/a"));
                            if (pwr_btn_state.Text == "Power Off")
                            {
                                item.Power_State = "ON";
                                i = 100;
                            }
                            else if (pwr_btn_state.Text == "Power On")
                            {
                                item.Power_State = "OFF";
                                i = 100;
                            }
                            else if (i == 100)
                            {
                                update_Log("ERROR:\tTimed out loading power state for " + item.Name + ".");
                            }
                            else
                            {
                                Thread.Sleep(250);
                            }
                        }
                        catch
                        {
                            if (cancelled  == true)
                            {
                                break;
                            }
                            else if (i == 100)
                            {
                                update_Log("ERROR:\tUnable to find power button element for " + item.Name + ".");
                                break;
                            }
                            Thread.Sleep(250);
                        }
                    } 

                    //check recent reports
                    int total_reports = 0;
                    url = "https://foreman.ordsvy.gov.uk/hosts/" + item.Name + @".ordsvy.gov.uk/config_reports";
                    driver.Navigate().GoToUrl(url);

                    //more than 30 reports?
                    try
                    {
                        IWebElement first_page_num = driver.FindElement(By.XPath(@"/html/body/div[3]/div/div[3]/div/div/b"));
                        string[] report_text = first_page_num.Text.Split(' ');
                        if (report_text.Length == 2)
                        {
                            total_reports = Int32.Parse(report_text[1]);
                        }
                        else if (report_text.Length == 1)
                        {
                            total_reports = Int32.Parse(report_text[0]);
                        }
                        else if (report_text.Length == 3)
                        {
                            total_reports = Int32.Parse(driver.FindElement(By.XPath(@"//*[@id='pagination']/div[1]/div/b[2]")).Text);
                            
                        }
                        update_Log("COUT:\t" + total_reports);
                    }
                    catch { }







                    //apply action
                    while (false)
                    {
                        switch (item.Action)
                        {
                            case "Deploy Puppet":
                                if (item.Reports != total_reports)
                                {

                                }
                                break;
                            case "Rebuild":
                                Console.WriteLine("Case 2");
                                break;
                            case "Restart":
                                Console.WriteLine("Case 3");
                                break;
                            default:
                                Console.WriteLine("Default case");
                                break;
                        }
                    }
                    Update_Lists(item);
                }
                n = num_of_jobs();
            }
            


        }

        private bool Login()
        {
            bool loaded = false;
            bool login_successfull = false;
            update_Log("TASK:\tStarting Webdriver...");
            try
            {
                //start webdriver
                driver = new FirefoxDriver();
                driver.Navigate().GoToUrl("https://foreman.ordsvy.gov.uk/users/login");
                update_Log("SUCCESS:\tWebdriver started.");

                try
                {
                    IWebElement username_field = driver.FindElement(By.XPath(@"//*[@id='login_login']"));
                    IWebElement password_field = driver.FindElement(By.XPath(@"//*[@id='login_password']"));
                    loaded = true;
                }
                catch
                {
                    update_Log("ERROR:\tUnable to load login page.\n");
                    loaded = false;

                }
            }

            catch
            {
                update_Log("ERROR:\tUnable to initiate Webdriver.\n");
            }
 

            //login using credentials
            if (loaded == true && cancelled  == false)
            {
                update_Log("TASK:\tWaiting for successful Login...");
                while (login_successfull == false && cancelled  == false)
                {
                    try
                    {
                        IWebElement hat_in_homescreen = driver.FindElement(By.XPath(@"/html/body/div[1]/div/div/div[1]/img"));
                        login_successfull = true;
                        update_Log("SUCCESS:\tLogin successful.");
                    }
                    catch (NoSuchElementException)
                    {
                        Thread.Sleep(250);
                    }
                    catch (WebDriverException)
                    {
                        update_Log("ERROR:\tLost Webdriver.");
                        kill_webdriver();
                        login_successfull = false;
                        return login_successfull;
                    }
                    catch (InvalidOperationException)
                    {
                        update_Log("ERROR:\tLost Webdriver.");
                        kill_webdriver();
                        login_successfull = false;
                        return login_successfull;
                    }

                }
            }
            return login_successfull;
        }
        

        private void closeBtn_Click(object sender, EventArgs e)
        {
            cancelled  = true;
            kill_webdriver();
            this.Close();
        }


        private void update_Log(string text)
        {
            if (InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<string>(update_Log), new object[] { text });
                    return;
                }
                catch { }
            }
            try
            {
                logBox.Items.Add(text);
            }
            catch { }
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
                    //proc.Kill();
                }
            }
            catch { }
        }

        private void turn_on()
        {

        }

        private void turn_off()
        {

        }

        private void restart()
        {

        }

        private void rebuild()
        {

        }

        private void deploy_puppet()
        {

        }

        private int num_of_jobs()
        {
            //get number of jobs in progress
            int num_of_jobs = 0;
            foreach (Machine item in jobList)
            {
                num_of_jobs++;
            }
            return num_of_jobs;
        }

        private void Update_Lists(Machine machine)
        {
            for (int i = 0; i < progressView.Items.Count; i++)
            {
                if (progressView.Items[i].SubItems[0].Text == machine.Name)
                {
                    progressView.Items[i].SubItems[4].Text = machine.Power_State;
                    progressView.Items[i].SubItems[5].Text = machine.Passes.ToString();
                    progressView.Items[i].SubItems[6].Text = machine.Fails.ToString();
                }
            }
            //progressView.Items.Add(new ListViewItem(new string[] { item.Name, item.Environment, item.Role, item.Action, item.Power_State, passes, fails }));
            
        }
    }
}
