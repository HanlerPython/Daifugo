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
        //嘗試出牌
        bool PlayCard(GameManager gm, IEnumerable<Card> cards);
        //嘗試跳過(Pass)
        bool Pass(GameManager gm);
        //繳交特殊效果卡牌 (應對 7 或 10 的捨棄動作)
        bool SubmitSpecialAction(GameManager gm, IEnumerable<Card> cards);
    }
}