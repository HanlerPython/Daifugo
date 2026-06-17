using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test01.Controller.States.AppStates
{
    public class AppManager
    {
        private IAppState _currentState;

        //用來放置UserControl的容器，即當前畫面
        private readonly Control _mainContainer;

        public AppManager(Control mainContainer)
        {
            _mainContainer = mainContainer;
        }

        //狀態切換
        public void ChangeState(IAppState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }

        public void ShowView(UserControl viewControl)
        {
            _mainContainer.Controls.Clear();
            //讓新畫面填滿容器
            viewControl.Dock = DockStyle.Fill; 
            _mainContainer.Controls.Add(viewControl);
        }
    }
}
