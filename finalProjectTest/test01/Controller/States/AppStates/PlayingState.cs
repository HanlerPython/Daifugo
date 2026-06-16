using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test01.View;
using test01.View.UserControls;

namespace test01.Controller.States.AppStates
{
    public class PlayingState : IAppState
    {
        private PlayingControl _view;
        private GameManager _gm;

        public void Enter(AppManager manager)
        {
            _view = new PlayingControl();
            _gm = new GameManager();

            //初始化手牌顯示元件以及GM
            _view.Hand.Initialize(_gm);
            _gm.Initialize();

            //呼叫app manager的渲染功能
            manager.ShowView(_view);
        }

        public void Exit(AppManager manager)
        {
            //離開時釋放資源
            _view?.Dispose();
            _gm = null;
        }
    }
}
