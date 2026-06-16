using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test01.Controller.States.AppStates
{
    public interface IAppState
    {
        //進入該狀態時觸發
        void Enter(AppManager manager);

        //離開該狀態時觸發
        void Exit(AppManager manager);
    }
}
