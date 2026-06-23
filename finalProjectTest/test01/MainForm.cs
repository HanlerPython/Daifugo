using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Model;
using test01.View;
using test01.Controller;
using test01.Controller.States.AppStates;
using test01.Utils;

namespace test01
{
    public partial class MainForm : Form
    {
        public static readonly int WindowWidth = 1080;
        public static readonly int WindowHeight = 720;

        private AppManager _appManager;
        private Panel _mainPanel; //作為畫面切換的根容器
 
        public MainForm()
        {
            InitializeComponent();
            InitializeMainPanel();
        }
        private void InitializeMainPanel()
        {
            _mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
            };
            this.Controls.Add(_mainPanel);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(WindowWidth, WindowHeight);

            ResourceManager.Initialize();
            ResourceManager.PlayBgm("Playing");

            //將容器託管給AppManager
            _appManager = new AppManager(_mainPanel);
            //從標題頁面開始
            _appManager.ChangeState(new MainMenuState());
        }
    }
}