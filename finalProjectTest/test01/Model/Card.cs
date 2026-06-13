using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test01.Model
{
    public class Card
    {
        public enum Suit
        {
            CLUBS,      //梅花
            DIAMONDS,   //方塊
            HEARTS,     //愛心
            SPADES,     //黑桃
            JOKER       //鬼牌特殊花色(也就是沒花色)
        }
        public enum Rank
        {
            THREE,
            FOUR,
            FIVE,
            SIX,
            SEVEN,
            EIGHT,
            NINE,
            TEN,
            JACK,
            QUEEN,
            KING,
            ACE,
            TWO,
            BLACK, //小王
            RED    //大王
        }

        public Suit SuitType { get; } //花色
        public Rank RankType { get; } //點數
        public int Weight { get; } //對應權重

        public Card(Suit suit, Rank rank)
        {
            this.SuitType = suit;
            this.RankType = rank;
            this.Weight = (int)rank;
        }
    }
}
