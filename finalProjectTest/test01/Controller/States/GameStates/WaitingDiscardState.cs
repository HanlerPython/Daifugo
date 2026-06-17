using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test01.Model;

namespace test01.Controller.States.GameStates
{
    public class WaitingDiscardState : IGameState
    {
        private int _requiredCount;

        public WaitingDiscardState(int requiredCount)
        {
            _requiredCount = requiredCount;
        }

        public void Enter(GameManager gm)
        {
            // 可在此觸發事件通知 UI 顯示「請選擇 X 張牌丟棄」的提示
        }

        //鎖定一般出牌與Pass
        public bool PlayCard(GameManager gm, IEnumerable<Card> cards) => false;
        public bool Pass(GameManager gm) => false;
        public bool SubmitSpecialAction(GameManager gm, IEnumerable<Card> cards)
        {
            if (cards.Count() > _requiredCount || !cards.Any())
                return false;

            // 執行捨棄邏輯
            // ... 從玩家手牌移除該牌並丟入廢牌堆 ...

            // 動作完成，解除鎖定，將發球權輪轉給下一位
            gm.ChangeState(new PlayerTurnState()); // 需配合輪轉指標的推移
            return true;
        }
    }
}
