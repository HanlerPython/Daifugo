using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test01.Model;

namespace test01.Controller.States.GameStates
{
    public interface IGameState
    {
        void Enter(GameManager gm);
        //嘗試出牌或換牌等等UI選牌操作
        bool PlayCard(GameManager gm, IEnumerable<Card> cards);
        //嘗試跳過(Pass)
        bool Pass(GameManager gm);
    }
}