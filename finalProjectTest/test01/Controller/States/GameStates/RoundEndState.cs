using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Model;

namespace test01.Controller.States.GameStates
{
    public class RoundEndState : IGameState
    {
        public async void Enter(GameManager gm)
        {
            //結算與排序名次
            var finalRanking = new List<Player>();
            finalRanking.AddRange(gm.Winners);
            finalRanking.AddRange(gm.Losers.AsEnumerable().Reverse().ToList());
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("本局結算名次：\n");
            string[] rankTitles = { "大富豪", "富豪", "貧民", "大貧民" };
            for (int i = 0; i < finalRanking.Count; i++)
            {
                finalRanking[i].Rank = i + 1;
                string title = (i < rankTitles.Length) ? rankTitles[i] : $"第{i + 1}名";
                string name = finalRanking[i].Name;
                string playerName = finalRanking[i].Id == 0 ? $"{name} (你)" : $"{name} (人機)";
                sb.AppendLine($"{title} : {playerName}");
            }
            MessageBox.Show(sb.ToString(), "回合結束", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //清空手牌與狀態
            foreach (var player in gm.Players)
            {
                player.ClearHand();
                player.StateType = Player.State.Active;
            }
            gm.NotifyPlayerHandChanged();

            await Task.Delay(1500);
            //進入下一局
            gm.ChangeState(new RoundInitState());
        }

        public bool PlayCard(GameManager gm, IEnumerable<Card> cards) => false;

        public bool Pass(GameManager gm) => false;
    }
}
