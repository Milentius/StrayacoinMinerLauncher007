using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Cpu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

namespace StrayacoinMinerLauncher007
{
    public partial class MainWindow : Window
    {
        // create a computer for getting device info
        Computer computer1 = new Computer();

        // create a timer to keep updating the device info
        private DispatcherTimer DevMonUpdateTimer = new DispatcherTimer();

        // CPU Dictionaries
        public Dictionary<string, ISensor> Sensors_CPU_Temp = new Dictionary<string, ISensor>();
        public Dictionary<string, float?> Sensors_CPU_Load = new Dictionary<string, float?>();
        public Dictionary<string, ISensor> Sensors_CPU_Clock = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_CPU_Power = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_CPU_Voltage = new Dictionary<string, ISensor>();

        public List<ISensor> test_clock_sensors = new List<ISensor>();


        // GPU Nvidia Dictionaries
        public Dictionary<string, ISensor> Sensors_GPU_Nvidia_Temp = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Nvidia_Load = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Nvidia_Clock = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Nvidia_SmallData = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Nvidia_Power = new Dictionary<string, ISensor>();

        //GPU AMD Dictionaries
        public Dictionary<string, ISensor> Sensors_GPU_AMD_Temp = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_AMD_Load = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_AMD_Clock = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_AMD_SmallData = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_AMD_Power = new Dictionary<string, ISensor>();

        // GPU Intel Dictionaries
        public Dictionary<string, ISensor> Sensors_GPU_Intel_Temp = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Intel_Load = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Intel_Clock = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Intel_SmallData = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Intel_Power = new Dictionary<string, ISensor>();

        // Controller and Network Dictionaries
        public Dictionary<string, ISensor> Sensors_Controller = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_Network = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_Motherboard = new Dictionary<string, ISensor>();
        

        // old being replaced Dictionaries
        public Dictionary<string, ISensor> Sensors_CPU = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Nvidia = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_AMD = new Dictionary<string, ISensor>();
        public Dictionary<string, ISensor> Sensors_GPU_Intel = new Dictionary<string, ISensor>();
        

        // create a list to store the device CPU info to be used on modules
        public List<ISensor> CPUSensors = new();
        public string? CpuBrand;

        // 
        public List<ISensor> MoBoSensors = new();
        
        // create a list to store the device GPU info to be used on modules
        public List<ISensor> GPUSensors = new();
        public string? GpuBrand1;
        public string? GpuBrand2;

        // create a list to store the device Motherboard info to be used on modules

        public MainWindow()
        {
            InitializeComponent();
            FrameLeftSidebar.NavigationService.Navigate(new Uri("Pages\\Sidebar_DeviceMonitor.xaml", UriKind.Relative));
            FrameBottomCenterbox.NavigationService.Navigate(new Uri("Pages\\MinerControlPanel.xaml", UriKind.Relative));
            GetDeviceInfo(true, false, false, false, false, false, false);
            DevMonUpdateTimer.Interval = TimeSpan.FromSeconds(5);
            DevMonUpdateTimer.Tick += DevMonUpdateTimer_Tick;
            DevMonUpdateTimer.Start();
        }

        private void DevMonUpdateTimer_Tick(object? sender, EventArgs e)
        {
            GetDeviceInfo(true, false, false, false, false, false, false);
        }

        // allow user to move the window by draging it
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        // shutdown the application
        private void btnAppClose_Click(object sender, RoutedEventArgs e)
        {
            DevMonUpdateTimer?.Stop();
            computer1.Close();
            Application.Current.Shutdown();
        }

        // minimize the application
        private void btnAppMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        // Function for getting device info
        private void GetDeviceInfo(bool getCpu, bool getMobo, bool getGpuNvidia, bool getGpuAmd, bool getGpuIntel, bool getController, bool getNetwork)
        {
            bool updateCPU = getCpu;
            bool updateMotherboard = getMobo;
            bool updateGPUNVidia = getGpuNvidia;
            bool updateGPUAMD = getGpuAmd;
            bool updateGPUIntel = getGpuIntel;
            bool updateController = getController;
            bool updateNetwork = getNetwork;

            computer1.Open();
            
            //computer1.IsMotherboardEnabled = true;
            
            


            //
            //    foreach (IHardware hardwareItemCPU in computer1.Hardware)
            //    {
            //        
            //            hardwareItemCPU.Update();
            //
            //            foreach (ISensor sensor in hardwareItemCPU.Sensors)
            //            {
            //                rtb1.AppendText(hardwareItemCPU.Name + "\t\t|\t\t" + sensor.Name + "\t\t|\t\t" + sensor.SensorType + "\t\t|\t\t" + sensor.Value + "\n");
            //            }
            //        
            //    }
            //

            // check if we need to get the cpu sensors
            if (updateCPU == true)
            {
                // enable the cpu sensors so we can get data on the cpu
                computer1.IsCpuEnabled = true;

                foreach (IHardware hardwareItemCPU in computer1.Hardware)
                {
                    if (hardwareItemCPU.HardwareType == HardwareType.Cpu)
                    {
                        hardwareItemCPU.Update();

                        foreach (ISensor sensor in hardwareItemCPU.Sensors)
                        {
                            if (sensor != null && sensor.SensorType == SensorType.Clock)
                            {
                                test_clock_sensors.Add(sensor);
                            }

                            if (!Sensors_CPU_Temp.ContainsKey(sensor.Name) && sensor.SensorType == SensorType.Temperature)
                            {
                                Sensors_CPU_Temp.Add(sensor.Name, sensor);
                            }
                            else if(!Sensors_CPU_Load.ContainsKey(sensor.Name) && sensor.SensorType == SensorType.Load)
                            {
                                Sensors_CPU_Load.Add(sensor.Name, sensor.Value.Value);

                            }
                            else if (!Sensors_CPU_Clock.ContainsKey(sensor.Name) && sensor.SensorType == SensorType.Clock)
                            {
                                Sensors_CPU_Clock.Add(sensor.Name, sensor);
                            }
                            else if (!Sensors_CPU_Power.ContainsKey(sensor.Name) && sensor.SensorType == SensorType.Power)
                            {
                                Sensors_CPU_Power.Add(sensor.Name, sensor);
                            }
                            else if (!Sensors_CPU_Voltage.ContainsKey(sensor.Name) && sensor.SensorType == SensorType.Voltage)
                            {
                                Sensors_CPU_Voltage.Add(sensor.Name, sensor);
                            }



                            rtb1.AppendText(sensor.Name + "\t\t|\t\t" + sensor.Value.ToString() + "\t\t|\t\t" + sensor.SensorType + "\n");
                            if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Core #"))

                            if (sensor != null)
                            {
                                    if (!Sensors_CPU.ContainsKey(sensor.Name)) { Sensors_CPU.Add(sensor.Name, sensor); }

                            }
                            else{Sensors_CPU[sensor.Name] = sensor;}CPUSensors.Add(sensor);}
                        computer1.IsCpuEnabled = false;
                    }
                }
            }

            // check if we need to get the motherboard sensors
            if (updateMotherboard == true)
            {
                computer1.IsMotherboardEnabled = true;
                foreach (IHardware hardwareItemMoBo in computer1.Hardware)
                {
                    if (hardwareItemMoBo.HardwareType == HardwareType.Motherboard)
                    {
                        hardwareItemMoBo.Update();

                        foreach (ISensor sensor in hardwareItemMoBo.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Power)
                            {
                                Sensors_Motherboard.Add(sensor.Name, sensor);
                            }




                            
                            rtb1.AppendText(sensor.Name + "\t\t|\t\t" + sensor.Value.ToString() + "\t\t|\t\t" + sensor.SensorType + "\n");
                            if (sensor != null)
                            {

                                MoBoSensors.Add(sensor);
                                Sensors_Motherboard.Add(sensor.Name, sensor);
                            }
                            else
                            {
                                Sensors_Motherboard[sensor.Name] = sensor;
                            }
                        }
                    }
                }
            }

            // check if we need to get the nvidia gpu sensors
            if (updateGPUNVidia == true)
            {
                foreach (IHardware hardwareItemGPUNV in computer1.Hardware)
                {
                    computer1.IsGpuEnabled = true;
                    // store the sensors for the GPU in the GPU sensor list
                    if (hardwareItemGPUNV.HardwareType == HardwareType.GpuNvidia)
                    {
                        hardwareItemGPUNV.Update();

                        foreach (ISensor sensor in hardwareItemGPUNV.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                Sensors_GPU_Nvidia_Temp.Add(sensor.Name,sensor);
                            }
                            else if (sensor.SensorType == SensorType.Load)
                            {
                                Sensors_GPU_Nvidia_Load.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.Clock)
                            {
                                Sensors_GPU_Nvidia_Clock.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.SmallData)
                            {
                                Sensors_GPU_Nvidia_SmallData.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.Power)
                            {
                                Sensors_GPU_Nvidia_Power.Add(sensor.Name, sensor);
                            }



                            rtb1.AppendText(sensor.Name + "\t\t|\t\t" + sensor.Value.ToString() + "\t\t|\t\t" + sensor.SensorType + "\n");
                            if (sensor != null && !Sensors_GPU_Nvidia.ContainsKey(sensor.Name))
                            {
                                Sensors_GPU_Nvidia.Add(sensor.Name, sensor);
                                GPUSensors.Add(sensor);
                            }
                            else
                            {
                                Sensors_GPU_Nvidia[sensor.Name] = sensor;
                                GPUSensors.Add(sensor);
                            }
                        }
                    }
                }
            }

            // check if we need to get the amd gpu sensors
            if (updateGPUAMD == true)
            {
                computer1.IsGpuEnabled = true;
                foreach (IHardware hardwareItemGPUAMD in computer1.Hardware)
                {
                    if (hardwareItemGPUAMD.HardwareType == HardwareType.GpuAmd)
                    {
                        hardwareItemGPUAMD.Update();

                        foreach (ISensor sensor in hardwareItemGPUAMD.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                Sensors_GPU_AMD_Temp.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.Load)
                            {
                                Sensors_GPU_AMD_Load.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.Clock)
                            {
                                Sensors_GPU_AMD_Clock.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.SmallData)
                            {
                                Sensors_GPU_AMD_SmallData.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.Power)
                            {
                                Sensors_GPU_AMD_Power.Add(sensor.Name, sensor);
                            }



                            rtb1.AppendText(sensor.Name + "\t\t|\t\t" + sensor.Value.ToString() + "\t\t|\t\t" + sensor.SensorType + "\n");
                            if (sensor != null && !Sensors_GPU_AMD.ContainsKey(sensor.Name))
                            {
                                Sensors_GPU_AMD.Add(sensor.Name, sensor);
                                GPUSensors.Add(sensor);
                            }
                            else
                            {
                                Sensors_GPU_AMD[sensor.Name] = sensor;
                                GPUSensors.Add(sensor);
                            }
                        }
                    }
                }
            }

                // temp load clock small power
            // check if we need to get the intel gpu sensors
            if (updateGPUIntel == true)
            {
                computer1.IsGpuEnabled = true;
                foreach (IHardware hardwareItemGPUIn in computer1.Hardware)
                {
                    if (hardwareItemGPUIn.HardwareType == HardwareType.GpuIntel)
                    {
                        hardwareItemGPUIn.Update();

                        foreach (ISensor sensor in hardwareItemGPUIn.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                Sensors_GPU_Intel_Temp.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.Load)
                            {
                                Sensors_GPU_Intel_Load.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.Clock)
                            {
                                Sensors_GPU_Intel_Clock.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.SmallData)
                            {
                                Sensors_GPU_Intel_SmallData.Add(sensor.Name, sensor);
                            }
                            else if (sensor.SensorType == SensorType.Power)
                            {
                                Sensors_GPU_Intel_Power.Add(sensor.Name, sensor);
                            }



                            rtb1.AppendText(sensor.Name + "\t\t|\t\t" + sensor.Value.ToString() + "\t\t|\t\t" + sensor.SensorType + "\n");
                            if (sensor != null && !Sensors_GPU_Intel.ContainsKey(sensor.Name))
                            {

                                Sensors_GPU_Intel.Add(sensor.Name, sensor);
                                GPUSensors.Add(sensor);
                            }
                            else
                            {
                                Sensors_GPU_Intel[sensor.Name] = sensor;
                            }
                        }
                    }
                }
            }

            // check if we need to get the controller sensors
            if (updateController == true)
            {
                computer1.IsControllerEnabled = true;
                foreach (IHardware hardwareItemController in computer1.Hardware)
                {
                    if (hardwareItemController.HardwareType == HardwareType.EmbeddedController)
                    {
                        hardwareItemController.Update();

                        foreach (ISensor sensor in hardwareItemController.Sensors)
                        {
                            rtb1.AppendText(sensor.Name + "\t\t|\t\t" + sensor.Value.ToString() + "\t\t|\t\t" + sensor.SensorType + "\n");
                            if (sensor != null && !Sensors_Controller.ContainsKey(sensor.Name))
                            {
                                Sensors_Controller.Add(sensor.Name, sensor);
                            }
                            else
                            {
                                Sensors_Controller[sensor.Name] = sensor;
                            }
                        }
                    }
                }
            }

            // check if we need to get the network sensors
            if (updateNetwork == true)
            {
                computer1.IsNetworkEnabled = true;
                foreach (IHardware hardwareItemNetwork in computer1.Hardware)
                {
                    if (hardwareItemNetwork.HardwareType == HardwareType.EmbeddedController)
                    {
                        hardwareItemNetwork.Update();

                        foreach (ISensor sensor in hardwareItemNetwork.Sensors)
                        {
                            rtb1.AppendText(sensor.Name + "\t\t|\t\t" + sensor.Value.ToString() + "\t\t|\t\t" + sensor.SensorType + "\n");
                            if (sensor != null && !Sensors_Network.ContainsKey(sensor.Name))
                            {
                                Sensors_Network.Add(sensor.Name, sensor);
                            }
                            else
                            {
                                Sensors_Controller[sensor.Name] = sensor;
                            }
                        }
                    }
                }
            }
        }

        //private ISensor UpdatePanel(string dmpSensorName, string dmpSensorType)
        //{
        //    ISensor sensorX;
        //    if (dmpSensorType == "Temperature")
        //    {
        //
        //    }
        //    return sensorX;
        //}
    }
}
