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

namespace test01
{
    public partial class MainForm : Form
    {
        public static readonly int WindowWidth = 800;
        public static readonly int WindowHeight = 600;

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
                BackColor = Color.FromArgb(40, 44, 52)
            };
            this.Controls.Add(_mainPanel);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(WindowWidth, WindowHeight);

            ResourceManager.initialize();

            //將容器託管給AppManager
            _appManager = new AppManager(_mainPanel);
            // 啟動狀態機。目前為開發測試階段，可直接進入 PlayingState
            // 未來完成主選單後，改為 _appManager.ChangeState(new StartMenuState());
            _appManager.ChangeState(new PlayingState());
        }
    }
}