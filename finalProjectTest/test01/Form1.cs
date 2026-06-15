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

namespace test01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static readonly int WindowWidth = 800;
        public static readonly int WindowHeight = 600;
        HandView hand;
        GameManager gm;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(WindowWidth, WindowHeight);
            ResourceManager.initialize();
            hand = new HandView();
            gm = new GameManager();
            this.Controls.Add(hand); //提前加入以確保位置不會跑掉
            hand.Initialize(gm);
            gm.Initialize();

            this.Click += Background_Click;
            hand.Location = new Point(0, 400);
        }
        private void Background_Click(object sender, EventArgs e)
        {
            //當點擊 Form1 空白處時，取消選取所有元件
            hand.Unselect();
        }
    }
}