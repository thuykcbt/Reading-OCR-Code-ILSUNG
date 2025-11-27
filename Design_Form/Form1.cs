using Design_Form.Job_Model;
using DevExpress.Utils.CommonDialogs;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors.Mask.Design;
using HalconDotNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Design_Form.PLC_Communication;
using System.Diagnostics;
using DevExpress.XtraPrinting;
using Design_Form.User_PLC;
using DevExpress.ClipboardSource.SpreadsheetML;
using System.Windows.Media.Media3D;
using Design_Form.Monitor_Product_Error;
using DevExpress.Utils.Filtering.Internal;
using System.Net.Sockets;
//using LModbus;
namespace Design_Form
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        HalconDotNet.HSmartWindowControl HSmartWindowControl1 = new HSmartWindowControl();
        public Form1()
        {
            InitializeComponent();
            Insert_camera();
            inital_dislapHalcon();
            inital_config_machine();
        }
        private void inital_dislapHalcon()
        {
           panel_Cam1.Controls.Add(HSmartWindowControl1);
            HSmartWindowControl1.Show();
        }
        public void inital_config_machine()
        {
            string debugFolder = AppDomain.CurrentDomain.BaseDirectory;
            string name_file = "Machine_Config.cam";
            string file_path = Path.Combine(debugFolder, name_file);
            if (!File.Exists(name_file))
            {
                wirte_config(file_path);
            }
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            string json = File.ReadAllText(name_file);
          
            Job_Model.Statatic_Model.config_machine = JsonConvert.DeserializeObject<Config_Machine>(json, settings);
        }
        public void wirte_config(string file_path)
        {
          Job_Model.Config_Machine config_Machine = new Job_Model.Config_Machine();
        
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(config_Machine, settings);
            File.WriteAllText(file_path, json);
        }
        public void wirte_data_Result(Config_Machine config_Machine)
        {
            string debugFolder = AppDomain.CurrentDomain.BaseDirectory;
            string name_file = "Machine_Config.cam";
            string file_path = Path.Combine(debugFolder, name_file);

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(config_Machine, settings);
            File.WriteAllText(file_path, json);
        }
        private void Insert_camera()
        {
            string debugFolder = AppDomain.CurrentDomain.BaseDirectory;
            string name_file = "Cam_Config.cam";
            string file_path = Path.Combine(debugFolder, name_file);
            if(!File.Exists(name_file))
            {
                wirte_camera();
            }
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            string json = File.ReadAllText(name_file);
            List<config_cam> cams = new List<config_cam>();
            cams = JsonConvert.DeserializeObject<List<config_cam>>(json, settings);
            Job_Model.Statatic_Model.model_run.total_camera = cams.Count;
            for(int i = 0;i<cams.Count;i++)
            {
                Job_Model.VisionHalcon cam1 = new Job_Model.VisionHalcon();
                cam1.Device = cams[i].device;
                cam1.name = cams[i].name;
                cam1.TriggerMode = cams[i].TriggerMode;
                cam1.Open_connect_Gige();
                Job_Model.Statatic_Model.Dino_lites.Add(cam1);
            }
        }
        private void wirte_camera()
        {
            List <config_cam> cams = new List<config_cam>();
            Job_Model.config_cam cam1 = new Job_Model.config_cam();
            cam1.device = "000cdf0a2ded_JAICorporation_GO5101MPGE";
            cam1.name = "GigEVision2";
            cam1.TriggerMode = "Off";
            Job_Model.config_cam cam2 = new Job_Model.config_cam();
            cam2.name = "USB3Vision";
            cam2.device = "CAM0";
            cams.Add(cam1);
            cams.Add(cam2);
            string debugFolder = AppDomain.CurrentDomain.BaseDirectory;
            string name_file = "Cam_Config.cam";
            string file_path = Path.Combine(debugFolder, name_file);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(cams, settings);
            File.WriteAllText(file_path, json);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
         
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            wirte_data_Result(Statatic_Model.config_machine);
        }
        private void status_inital_Click(object sender, EventArgs e)
        {
            ModelView.ReadCodeModel readCodeModel = new ModelView.ReadCodeModel();
            readCodeModel.Code_Datatrix = label33.Text;
            readCodeModel.Code_QRCode =  label35.Text;
            readCodeModel.Dpt_Time =  label37.Text;
            readCodeModel.W_K_Time =  label39.Text;
            readCodeModel.Arr_Time = label41.Text;
            readCodeModel.Main_PO = label42.Text;
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(readCodeModel, settings);
            Job_Model.Statatic_Model.wirtelog_CODE.Log(json);
        }
        private void status_auto_Click(object sender, EventArgs e)
        {
            HObject InputIMG;
            InputIMG = Job_Model.Statatic_Model.Dino_lites[0].capture_halcom();
            HOperatorSet.ClearWindow(HSmartWindowControl1.HalconWindow);
            HOperatorSet.DispObj(InputIMG, HSmartWindowControl1.HalconWindow);
            Job_Model.Statatic_Model.model_run.Cameras[0].Jobs[0].ExecuteAllImge(HSmartWindowControl1.HalconWindow, InputIMG);
        }
    }
}
