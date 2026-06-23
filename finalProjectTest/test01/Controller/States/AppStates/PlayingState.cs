using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test01.View;
using test01.View.Playing;
using static test01.Model.Interfaces.GreedyAIStrategy;

namespace test01.Controller.States.AppStates
{
    public class PlayingState : IAppState
    {
        private PlayingControl _view;
        private GameManager _gameManager;
        private readonly Difficulty _difficulty;

        public PlayingState(Difficulty difficulty)
        {
            _difficulty = difficulty;
        }
        public void Enter(AppManager manager)
        {
            _view = new PlayingControl();
            _gameManager = new GameManager(_difficulty);

            //初始化顯示元件以及GM
            _view.Initialize(_gameManager);
            _gameManager.Initialize();

            //訂閱pass按鈕事件
            _view.OnPlayerPass += HandlePlayerPass;

            //呼叫app manager的渲染功能
            manager.ShowView(_view);
        }
        public void Exit(AppManager manager)
        {
            //離開時釋放資源
            _view?.Dispose();
            _gameManager = null;
        }
        private void HandlePlayerPass(object sender, EventArgs e)
        {
            _gameManager.TryPass();
        }
    }
}
