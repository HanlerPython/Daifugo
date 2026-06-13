using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Model;
using static test01.Model.Card;

namespace test01.View
{
    public static class ResourceManager
    {
        private static Dictionary<(Card.Suit Suit, Card.Rank Rank), Image> cardFaceImages =
            new Dictionary<(Card.Suit, Card.Rank), Image>();
        private static Image cardBackImage;

        public static void initialize()
        {
            string baseDir = Path.Combine(Application.StartupPath, "Assets/img");

            //卡背
            string backPath = Path.Combine(baseDir, "card_back.png");
            if (File.Exists(backPath))
            {
                cardBackImage = Image.FromFile(backPath);
            }

            //卡面(除了鬼牌)
            foreach (Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
            {
                if (suit == Card.Suit.JOKER)
                    continue;

                foreach (Card.Rank rank in Enum.GetValues(typeof(Card.Rank)))
                {
                    if (rank == Card.Rank.BLACK || rank == Card.Rank.RED)
                        continue;

                    string key = $"{suit}_{rank}";
                    string fullPath = Path.Combine(baseDir, $"{key}.png");
                    if (File.Exists(fullPath))
                    {
                        cardFaceImages[(suit, rank)] = Image.FromFile(fullPath);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(fullPath + " loaded error");
                    }
                }
            }

            //大小王
            string path = Path.Combine(baseDir, "JOKER_BLACK.png");
            cardFaceImages[(Suit.JOKER, Rank.BLACK)] = Image.FromFile(path);
            path = Path.Combine(baseDir, "JOKER_RED.png");
            cardFaceImages[(Suit.JOKER, Rank.RED)] = Image.FromFile(path);
        }
        public static Image getCardFaceImage(Card.Suit suit, Card.Rank rank)
        {
            if (cardFaceImages.TryGetValue((suit, rank), out Image img))
            {
                return img;
            }

            return null;
        }
        public static Image getCardBackImage()
        {
            return cardBackImage;
        }
    }
}
