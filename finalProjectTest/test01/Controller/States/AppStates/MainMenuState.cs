using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.View.Playing;
using static test01.Model.Interfaces.GreedyAIStrategy;

namespace test01.Controller.States.AppStates
{

    public class MainMenuState : IAppState
    {
        private MainMenuControl _view;
        private AppManager _appManager;

        public void Enter(AppManager manager)
        {
            _view = new MainMenuControl();
            _appManager = manager;

            _view.OnStartGame += HandleStartGame;
            _appManager.ShowView(_view);
        }

        public void Exit(AppManager manager)
        {
            _view?.Dispose();
            _appManager = null;
        }
        private void HandleStartGame(object sender, EventArgs e)
        {
            Difficulty difficulty = (Difficulty)_view.SelectedDifficulty;
            _appManager.ChangeState(new PlayingState(difficulty));
        }
    }
}
