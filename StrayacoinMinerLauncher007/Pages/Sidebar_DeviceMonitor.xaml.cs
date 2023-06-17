using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Cpu;
using MahApps.Metro.Controls;
using StrayacoinMinerLauncher007.Resources;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace StrayacoinMinerLauncher007.Pages
{
    /// <summary>
    /// this is the DeviceMonitor page which is used to monitor the device for issues as we mine strayacoin
    /// </summary>
    public partial class Sidebar_DeviceMonitor : Page
    {
        // create a reference to the main window so we can get data from it
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        // dictionaries for creating and updating panels
        public Dictionary<string, float> CoresAndThreads = new Dictionary<string, float>();
        public List<ISensor> PackageOrAverage = new List<ISensor>();

        Dictionary<string, DevMonPanel> dmpObjectsNormal = new Dictionary<string, DevMonPanel>();
        Dictionary<string, DevMonPanelCurrentOnly> dmpObjectsCurrentOnly = new Dictionary<string, DevMonPanelCurrentOnly>();
        Dictionary<string, DevMonPanelHeading> dmpObjectsHeading = new Dictionary<string, DevMonPanelHeading>();

        // variable to store the total number of sensors so we know how many panels to add to the sidebar

        // create a variable to alternate the devmonpanel's background color
        private bool IsLightDevMonPanel = true;

        // create variavles to store the number of cpu cores and threads
        private int CPU_Threads = 0;
        private int CPU_Cores = 0;

        DispatcherTimer devmonTimer = new DispatcherTimer();

        public Sidebar_DeviceMonitor()
        {
            InitializeComponent();

            devmonTimer.Interval = TimeSpan.FromSeconds(5);
            devmonTimer.Tick += DevmonTimer_Tick;
            devmonTimer.Start();

            CPU_Threads = Environment.ProcessorCount;
            CPU_Cores = CPU_Threads / 2;

            // get cpu data from main window
            initUpdateCpuData(mainWindow.CPUSensors, CPU_Cores + 1, CPU_Threads + 1);

            // add a heading panel for core temperatures
            AddHeadingPanel("Core Temperatures", false);

            // add a devmonpanel for each core
            for (int i = 1; i <= CPU_Cores; i++)
            {
                if (CoresAndThreads.ContainsKey($"CPU Core #{i}"))
                {
                    AddDevMonPanel("DevMonPanelCore" + i, i, CoresAndThreads[$"CPU Core #{i}"], 0);
                }
            
            }

            // add a devmonheading panel for thread load
            AddHeadingPanel("Threads Load", false);

            // add a devmonpanel for each thread
            for (int i = 1; i <= CPU_Threads; i++)
            {
                if (CoresAndThreads.ContainsKey($"CPU Core #{i} Thread #1"))
                {
                    AddDevMonPanel($"Core{i}Thread1", i, CoresAndThreads[$"CPU Core #{i} Thread #1"], 0);
            
                }
            
                if (CoresAndThreads.ContainsKey($"CPU Core #{i} Thread #2"))
                {
                    AddDevMonPanel($"Core{i}Thread2", i, CoresAndThreads[$"CPU Core #{i} Thread #2"], 0);
            
                }
            }
            
            // add a devmonheading panel for thread load
            AddHeadingPanel("Testong", false);

            // add panels for clock speeds
            for (int i = 1; i <= CPU_Cores; i++)
            {
                if (!dmpObjectsCurrentOnly.ContainsKey("Core 1 Clock")){
                    if (CoresAndThreads.ContainsKey($"CPU Core #{i}"))
                    {
                        AddDevMonPanel($"Core{i}Clock", i, mainWindow.test_clock_sensors[i - 1].Value.Value, 0);
            
                    }
                }
            
            }

            // update the static panels on the sidebar
            updateStaticDevMonPanels();


            //
            //for (int i = 0; i < CPU_Cores; i++)
            //{
            //    CheckForDevMonPanel("CurrentOnly", "DevMonPanelClock" + i, dmpObjectsNormal, dmpObjectsCurrentOnly, dmpObjectsHeading);
            //}
            //for (int i = 0; i < CPU_Cores; i++)
            //{
            //    UpdateClocks("CurrentOnly", "DevMonPanelClock" + i, i, mainWindow.test_clock_sensors, dmpObjectsNormal, dmpObjectsCurrentOnly, dmpObjectsHeading);
            //}

        }

        private void DevmonTimer_Tick(object? sender, EventArgs e)
        {
            UpdateThreadData("Core1Thread1", 1, 1, mainWindow.Sensors_CPU_Load, dmpObjectsNormal);
        }

        private void updateStaticDevMonPanels()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblPackageTemp.Content = PackageOrAverage[0].Value + " °C"; if (PackageOrAverage[0].Value > 0 && PackageOrAverage[0].Value < 70) { lblPackageTemp.Foreground = Brushes.MediumSpringGreen; } else if (PackageOrAverage[0].Value >= 71 && PackageOrAverage[0].Value < 90) { lblPackageTemp.Foreground = Brushes.Orange; }
                lblTotalLoad.Content = PackageOrAverage[1].Value.Value.ToString("n2") + " %"; if (PackageOrAverage[1].Value > 0 && PackageOrAverage[1].Value < 70) { lblPackageTemp.Foreground = Brushes.MediumSpringGreen; } else if (PackageOrAverage[1].Value >= 71 && PackageOrAverage[1].Value < 90) { lblPackageTemp.Foreground = Brushes.Orange; };
                lblPackagePower.Content = PackageOrAverage[2].Value.Value.ToString("n2"); if (PackageOrAverage[2].Value > 0 && PackageOrAverage[2].Value < 70) { lblPackageTemp.Foreground = Brushes.MediumSpringGreen; } else if (PackageOrAverage[2].Value >= 71 && PackageOrAverage[2].Value < 90) { lblPackageTemp.Foreground = Brushes.Orange; }
                lblBusSpeed.Content = PackageOrAverage[3].Value.Value.ToString("n2"); if (PackageOrAverage[3].Value > 0 && PackageOrAverage[3].Value < 70) { lblPackageTemp.Foreground = Brushes.Orange; } else if (PackageOrAverage[3].Value >= 71 && PackageOrAverage[3].Value < 90) { lblPackageTemp.Foreground = Brushes.MediumSpringGreen; }
            }));

        }

        private void AddDevMonPanel(string name, int num, float curval, float maxval)
        {
            DevMonPanel dmp = new DevMonPanel();
            DevMonPanelCurrentOnly dmpco = new DevMonPanelCurrentOnly();
            DevMonPanelHeading dmph = new DevMonPanelHeading();

            {
                // 
                if (name.Contains("Core") && !name.Contains("Thread") && !name.Contains("Clock"))
                {
                    dmp.Name = name;
                    dmp.SensorName = $"Core #{num}";

                    // update the max value if it is lower than the current value.
                    if (maxval < curval)
                    {
                        maxval = curval;
                        dmp.MaxValue = curval.ToString();
                    }
                    dmp.MaxValue = maxval.ToString();

                    // dock the panel at the top of the dockpanel and add it to the children so it can be shown
                    DockPanel.SetDock(dmp, Dock.Top);
                    DockPanelMainPanel.Children.Add(dmp);
                }
                else if (name.Contains($"Core{num}Thread1"))
                {
                    dmp.Name = name;
                    dmp.SensorName = $"Core #{num} Thread #1";
                    dmp.SensorValue = curval.ToString() + "%"; if (curval < 70) { dmp.lblSensorValue.Foreground = Brushes.MediumSpringGreen; } else { dmp.lblSensorValue.Foreground = Brushes.Orange; }
                    if (maxval < curval)
                    {
                        maxval = curval;
                        dmp.MaxValue = curval.ToString();
                    }
                    dmp.MaxValue = maxval.ToString();
                    DockPanel.SetDock(dmp, Dock.Top);
                    DockPanelMainPanel.Children.Add(dmp);
                    if (!dmpObjectsNormal.ContainsKey(name))
                    {
                        dmpObjectsNormal.Add(name, dmp);
                    }
                }
                else if (name.Contains($"Core{num}Thread2"))
                {
                    dmp.Name = name;
                    dmp.SensorName = $"Core #{num} Thread #2";
                    dmp.SensorValue = curval.ToString() + "%"; if (curval < 70) { dmp.lblSensorValue.Foreground = Brushes.MediumSpringGreen; } else { dmp.lblSensorValue.Foreground = Brushes.Orange; }
                    if (maxval < curval)
                    {
                        maxval = curval;
                        dmp.MaxValue = curval.ToString();
                    }
                    dmp.MaxValue = maxval.ToString();
                    DockPanel.SetDock(dmp, Dock.Top);
                    DockPanelMainPanel.Children.Add(dmp);
                    if (!dmpObjectsNormal.ContainsKey(name))
                    {
                        dmpObjectsNormal.Add(name, dmp);
                    }
                }
                else if (name.Contains("Clock"))
                {
                    dmpco.SensorName = $"Core {num} Clock Speed";
                    dmpco.SensorValue = curval.ToString() + "Mhz";
                    dmpco.Name = name;
                    DockPanel.SetDock(dmpco, Dock.Top);
                    DockPanelMainPanel.Children.Add(dmpco);
                    dmpObjectsCurrentOnly.Add(name, dmpco);
                }
                
                if (name.Contains("Core") && !name.Contains("Thread") && !name.Contains("Clock"))
                {
                    dmp.SensorValue = curval.ToString(); if (curval < 70) { dmp.lblSensorValue.Foreground = Brushes.MediumSpringGreen; } else { dmp.lblSensorValue.Foreground = Brushes.Orange; }
                }

                if (IsLightDevMonPanel == true)
                {
                    dmp.BackColor = IsLightDevMonPanel;
                    IsLightDevMonPanel = false;
                }
                else if (IsLightDevMonPanel == false)
                {
                    dmp.BackColor = IsLightDevMonPanel;
                    IsLightDevMonPanel = true;
                }
            }

            

        }

        private void AddHeadingPanel(string Text, bool isLight)
        {
            DevMonPanelHeading dmph = new DevMonPanelHeading();
            dmph.HeadingText = Text;
            dmph.BackColor = isLight;
            DockPanel.SetDock(dmph, Dock.Top);
            DockPanelMainPanel.Children.Add(dmph);
        }



        private Tuple<ISensor, ISensor, ISensor, ISensor> initUpdateCpuData(List<ISensor> sensorsList, int numCores, int numThreads)
        {
            ISensor? PackTemp = null;
            ISensor? TotalLoad = null;
            ISensor? PackPower = null;
            ISensor? BusSpeed = null;

            foreach (ISensor sensor in sensorsList)
            {
                if (sensor != null)
                {
                    // get the package temperature then store it in the list and return in the tuple
                    if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Package"))
                    {
                        CoresAndThreads.Add(sensor.Name, sensor.Value.Value);
                        PackTemp = sensor;
                    }

                    //
                    if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("Total"))
                    {
                        CoresAndThreads.Add(sensor.Name, sensor.Value.Value);
                        TotalLoad = sensor;
                    }

                    //
                    if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("Package"))
                    {
                        CoresAndThreads.Add(sensor.Name + "1", sensor.Value.Value);
                        PackPower = sensor;
                    }

                    if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("Bus"))
                    {
                        CoresAndThreads.Add(sensor.Name, sensor.Value.Value);
                        BusSpeed = sensor;
                    }

                    // 
                    for (int i = 1; i < numCores; i++)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains($"CPU Core #{i}") && !sensor.Name.Contains("Tj"))
                        {
                            CoresAndThreads.Add(sensor.Name, sensor.Value.Value);
                        }

                    }

                    // 
                    for (int i = 1; i < numCores; i++)
                    {

                        if (sensor.SensorType == SensorType.Load)
                        {
                            if (sensor.Name.Contains($"Core #{i} Thread #1") || sensor.Name.Contains($"Core #{i} Thread #2"))
                            {
                                CoresAndThreads.Add(sensor.Name, sensor.Value.Value);
                            }
                        }
                    }
                }
            }
            PackageOrAverage.Add(PackTemp);
            PackageOrAverage.Add(TotalLoad);
            PackageOrAverage.Add(PackPower);
            PackageOrAverage.Add(BusSpeed);

            return Tuple.Create(PackTemp, TotalLoad, PackPower, BusSpeed);
        }

        // update the current thread data with a new value from the dictionary passed in
        private void UpdateThreadData(string name, int coreNum, int threadNum, Dictionary<string,float?> newData, Dictionary<string,DevMonPanel> inputObjects)
        {
            for (int i = 0; i < newData.Count; i++)
            {
                if (name.Contains($"Core{coreNum}Thread{threadNum}"))
                {
                    dmpObjectsNormal[name].SensorValue = newData.ToString();
                }
            }
        }
        

        //private void UpdateClocks(string dmpType, string dmpName, int num, Dictionary<string, ISensor> inSensor, Dictionary<string, DevMonPanel> dmpDict, Dictionary<string, DevMonPanelCurrentOnly> dmpcoDict, Dictionary<string, DevMonPanelHeading> dmphDict)
        private void UpdateClocks(string dmpType, string dmpName, int num, List<ISensor> inSensor, Dictionary<string, DevMonPanel> dmpDict, Dictionary<string, DevMonPanelCurrentOnly> dmpcoDict, Dictionary<string, DevMonPanelHeading> dmphDict)
        {
            if (dmpcoDict.ContainsKey($"Core #{num} Clock"))
            {
                for (int i = 0; i < CPU_Cores; i++)
                {
                    dmpcoDict["DevMonPanelClock" + i].SensorValue = inSensor[i].ToString();
                }
                // WRONGGGG
                //dmpcoDict["DevMonPanelClock" + num].SensorValue = inSensor["DevMonPanelClock" + num].Value.Value.ToString();
            }
        }
        private void UpdateVoltage(string dmpType, string dmpName, Dictionary<string, DevMonPanel> dmpDict, Dictionary<string, DevMonPanelCurrentOnly> dmpcoDict, Dictionary<string, DevMonPanelHeading> dmphDict)
        {
            var inputSensorDict = mainWindow.Sensors_CPU;
            if (inputSensorDict.ContainsKey("Clock"))
            {
                
            }
        }
        private void UpdateGPUNV(string dmpType, string dmpName, Dictionary<string, DevMonPanel> dmpDict, Dictionary<string, DevMonPanelCurrentOnly> dmpcoDict, Dictionary<string, DevMonPanelHeading> dmphDict)
        {
            var inputSensorDict = mainWindow.Sensors_CPU;
            if (inputSensorDict.ContainsKey("Clock"))
            {

            }
        }
        private void UpdateGPUAMD(string dmpType, string dmpName, Dictionary<string, DevMonPanel> dmpDict, Dictionary<string, DevMonPanelCurrentOnly> dmpcoDict, Dictionary<string, DevMonPanelHeading> dmphDict)
        {
            var inputSensorDict = mainWindow.Sensors_CPU;
            if (inputSensorDict.ContainsKey("Clock"))
            {

            }
        }
        private void UpdateGPUIntel(string dmpType, string dmpName, Dictionary<string, DevMonPanel> dmpDict, Dictionary<string, DevMonPanelCurrentOnly> dmpcoDict, Dictionary<string, DevMonPanelHeading> dmphDict)
        {
            var inputSensorDict = mainWindow.Sensors_CPU;
            if (inputSensorDict.ContainsKey("Clock"))
            {

            }
        }

        private bool CheckPanelExists(string PanelType, int num,string PanelName,Dictionary<string,DevMonPanelCurrentOnly>PanelDictionary)
        {
            if (PanelType == "Normal" && PanelDictionary.ContainsKey($"Core #{num} Clock"))
            {
                
                return true;

            }
            else
            {
                return false;
            }
        }


        // pass in the dmpName which is what we want to call the sensor
        // pass in a dictionary from the main window that we want to look in for the sensor
        // if we find the sensor name we return true and call the "updatePanel" function
        // if we do not find the sensor we add the sensor and then return false and call the "updatePanel" function
        private bool CheckForDevMonPanel(string dmpType,string dmpName, Dictionary<string, DevMonPanel> dmpDict, Dictionary<string, DevMonPanelCurrentOnly> dmpcoDict, Dictionary<string, DevMonPanelHeading> dmphDict)
        {
            if (!dmpDict.ContainsKey(dmpName) && dmpType == "Normal")
            {
                // sensor not found : create a panel since we cannot find the panel name in the sensor dictionary and then call the updatePanel function
                DevMonPanel dmp = new DevMonPanel // create the panel
                {
                    Name = dmpName // give the panel a name
                    
                };

                DockPanel.SetDock(dmp, Dock.Top); // dock the panel at the top of the underlying DockPanel
                dmpDict.Add(dmpName,dmp);
                DockPanelMainPanel.Children.Add(dmp);// add the panel to the dictionary of panels we have created
                if (IsLightDevMonPanel == true)
                {
                    dmp.BackColor = IsLightDevMonPanel;
                    IsLightDevMonPanel = false;
                }
                else if (IsLightDevMonPanel == false)
                {
                    dmp.BackColor = IsLightDevMonPanel;
                    IsLightDevMonPanel = true;
                }
                return true;
            }
            else if (!dmpcoDict.ContainsKey(dmpName) && dmpType == "CurrentOnly")
            {
                // sensor not found : create a panel since we cannot find the panel name in the sensor dictionary and then call the updatePanel function
                DevMonPanelCurrentOnly dmpco = new DevMonPanelCurrentOnly // create the panel
                {
                    Name = dmpName // give the panel a name
                };

                DockPanel.SetDock(dmpco, Dock.Top); // dock the panel at the top of the underlying DockPanel
                dmpcoDict.Add(dmpName, dmpco); // add the panel to the dictionary of panels we have created
                DockPanelMainPanel.Children.Add(dmpco);

                dmpco.SensorName = dmpName;
                dmpco.SensorValue = mainWindow.test_clock_sensors[0].Value.ToString();

                // check if the panel should have a light background
                if (IsLightDevMonPanel == true) 
                {
                    dmpco.BackColor = IsLightDevMonPanel;
                    IsLightDevMonPanel = false;
                }
                else if (IsLightDevMonPanel == false)
                {
                    dmpco.BackColor = IsLightDevMonPanel;
                    IsLightDevMonPanel = true;
                }
                return true;
            }
            else if (!dmphDict.ContainsKey(dmpName) && dmpType == "Heading")
            {
                // sensor not found : create a panel since we cannot find the panel name in the sensor dictionary and then call the updatePanel function
                DevMonPanelHeading dmph = new DevMonPanelHeading  // create the panel
                {
                    Name = dmpName // give the panel a name
                };
                DockPanel.SetDock(dmph, Dock.Top); // dock the panel at the top of the underlying DockPanel
                dmphDict.Add(dmpName, dmph); // add the panel to the dictionary of panels we have created
                DockPanelMainPanel.Children.Add(dmph);

                // check if the panel should have a dark background
                if (IsLightDevMonPanel == true) 
                {
                    dmph.BackColor = IsLightDevMonPanel;
                    IsLightDevMonPanel = false;
                }
                else if (IsLightDevMonPanel == false) 
                {
                    dmph.BackColor = IsLightDevMonPanel;
                    IsLightDevMonPanel = true;
                }
                return true;
            }
            return false; // return false is we do no find the panel
        }
       
        // update the panels pertaining to the cpu
        private void updatePanelCPU(string dmpType,string dmpName, Dictionary<string,ISensor> dataDict, Dictionary<string, DevMonPanel> dmpDict, Dictionary<string, DevMonPanelCurrentOnly> dmpcoDict, Dictionary<string, DevMonPanelHeading> dmphDict)
        {
            if (dmpType == "Normal")
            {
                if (dmpDict.ContainsKey(dmpName)) 
                {
                    dmpDict[dmpName].SensorValue = dataDict[dmpName].Value.ToString();
                    if (dataDict[dmpName].Value > float.Parse(dmpDict[dmpName].SensorValue))
                    {
                        dmpDict[dmpName].MaxValue = dataDict[dmpName].Value.ToString();
                    }
                }
            }
            else if (dmpType == "CurrentOnly")
            {
                if (dmpcoDict.ContainsKey(dmpName))
                {
                    dmpcoDict[dmpName].SensorValue = dataDict[dmpName].Value.ToString();
                }
            }
            else if (dmpType == "Heading")
            {
                if (dmpcoDict.ContainsKey(dmpName))
                {
                    dmphDict[dmpName].HeadingText = dmpName;
                }
            }
        }

        // 
        private void updateCpuThreads(ISensor inSensor, Dictionary<string, DevMonPanel> DictDevMonPanels)
        {
            DevMonPanelCurrentOnly dmpco = new DevMonPanelCurrentOnly();
            for (int i = 1; i < CPU_Threads; i++)
            {
                //if (!DictDevMonPanels.ContainsKey($"dmpco{i}_CurrentLoad"))
            }
        }
        
        // update the panels pertaining to the nvidia gpu
        private void updatePanelGPUNV(string dmpType, string dmpName, Dictionary<string, ISensor> dataDict, Dictionary<string, DevMonPanel> dmpDict, Dictionary<string, DevMonPanelCurrentOnly> dmpcoDict, Dictionary<string, DevMonPanelHeading> dmphDict)
        {
            if (dmpType == "Normal")
            {
                if (dmpDict.ContainsKey(dmpName))
                {
                    dmpDict[dmpName].SensorValue = dataDict[dmpName].Value.ToString();
                }
            }
            else if (dmpType == "CurrentOnly")
            {
                if (dmpcoDict.ContainsKey(dmpName))
                {
                    dmpcoDict[dmpName].SensorValue = dataDict[dmpName].Value.ToString();
                }
            }
            else if (dmpType == "Heading")
            {

            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            devmonTimer.Stop();
        }



        //private void updateCpuTemps(Dictionary<string,ISensor> inSensor, Dictionary<string, DevMonPanel> DictDevMonPanels)
        //{
        //    
        //    for (int i = 1; i < CPU_Cores; i++)
        //    {
        //        if (DictDevMonPanels.ContainsKey($"dmp_Core{i}_CurrentTemp"))
        //        {
        //            DictDevMonPanels[$"dmp_Core{i}_CurrentTemp"].SensorValue = inSensor[$"dmp_Core{i}_CurrentTemp"].Value.ToString();
        //            var maxTemp = float.Parse(DictDevMonPanels[$"dmp_Core{i}_CurrentTemp"].MaxValue);
        //            if (maxTemp < inSensor[$"dmp_Core{i}_CurrentTemp"].Value)
        //            {
        //                DictDevMonPanels[$"dmp_Core{i}_CurrentTemp"].MaxValue = inSensor[$"dmp_Core{i}_CurrentTemp"].Value.ToString();
        //            }
        //        }
        //
        //        if (!DictDevMonPanels.ContainsKey($"dmp_Core{i}_CurrentTemp"))
        //        {
        //            DevMonPanel dmp = new DevMonPanel();
        //            if (inSensor.SensorType == SensorType.Temperature && inSensor.Name.Contains("Core #"))
        //            {
        //                dmp.Name = $"dmp_Core{i}_CurrentTemp"; // object name
        //                dmp.SensorName = $"Core #{i} Temperature"; // display name
        //                dmp.SensorValue = inSensor.Value.ToString(); // display value
        //                if (float.Parse(dmp.MaxValue) < inSensor.Value)
        //                {
        //                    dmp.MaxValue = inSensor.Value.ToString();
        //                }
        //                dmpObjectsCoreTemps.Add(dmp.Name, dmp);
        //                DockPanel.SetDock(dmp, Dock.Top);
        //                DockPanelMainPanel.Children.Add(dmp);
        //            }
        //        }
        //    }
        //}

    }
}
