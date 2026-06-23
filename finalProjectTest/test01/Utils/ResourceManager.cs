using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Model;
using static test01.Model.Card;

namespace test01.Utils
{
    public static class ResourceManager
    {
        private static readonly Dictionary<(Suit Suit, Rank Rank), Image> _cardFaceImages =
            new();
        private static Image _cardBackImage;
        private static SoundPlayer _currentBgmPlayer;
        private static readonly Dictionary<string, SoundPlayer> _bgm = new();
        private static readonly Image[] _buttonImage = new Image[3];
        private static Image _titlleImage;
        private static Image _creditsBackImage;
        private static readonly Image[] _playerFrameImage = new Image[4];
        private static readonly Image[] _playerAvatarImage = new Image[4];

        public static void Initialize()
        {
            string baseDir = Path.Combine(Application.StartupPath, "Assets/img");

            //卡背
            string backPath = Path.Combine(baseDir, "card_back.png");
            if (File.Exists(backPath))
            {
                _cardBackImage = Image.FromFile(backPath);
            }

            //卡面(除了鬼牌)
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                if (suit == Suit.JOKER)
                    continue;

                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    if (rank == Rank.BLACK || rank == Rank.RED)
                        continue;

                    string key = $"{suit}_{rank}";
                    string fullPath = Path.Combine(baseDir, $"{key}.png");
                    if (File.Exists(fullPath))
                    {
                        _cardFaceImages[(suit, rank)] = Image.FromFile(fullPath);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(fullPath + " loaded error");
                    }
                }
            }

            //大小王
            string path = Path.Combine(baseDir, "JOKER_BLACK.png");
            _cardFaceImages[(Suit.JOKER, Rank.BLACK)] = Image.FromFile(path);
            path = Path.Combine(baseDir, "JOKER_RED.png");
            _cardFaceImages[(Suit.JOKER, Rank.RED)] = Image.FromFile(path);

            //按鈕圖片
            path = Path.Combine(baseDir, "btn_default.png");
            _buttonImage[0] = Image.FromFile(path);

            path = Path.Combine(baseDir, "btn_hover.png");
            _buttonImage[1] = Image.FromFile(path);

            path = Path.Combine(baseDir, "btn_pressed.png");
            _buttonImage[2] = Image.FromFile(path);

            //玩家頭像與邊框
            for(int i = 0; i < 4; i++)
            {
                path = Path.Combine(baseDir, $"player_frame_{i}.png");
                _playerFrameImage[i] = Image.FromFile(path);
                path = Path.Combine(baseDir, $"player_avatar_{i}.png");
                _playerAvatarImage[i] = Image.FromFile(path);
            }

            //標題圖片
            path = Path.Combine(baseDir, "title.png");
            _titlleImage = Image.FromFile(path);

            //製作人員名單背景圖片
            path = Path.Combine(baseDir, "credits_back.png");
            _creditsBackImage = Image.FromFile(path);

            //音訊檔案
            baseDir = Path.Combine(Application.StartupPath, "Assets/audio");
            path = Path.Combine(baseDir, "bgm.wav");
            _bgm["Playing"] = new SoundPlayer(path);
            _bgm["Playing"].LoadAsync();
        }
        public static Image GetCardFaceImage(Suit suit, Rank rank)
        {
            if (_cardFaceImages.TryGetValue((suit, rank), out Image img))
            {
                return img;
            }

            return null;
        }
        public static Image GetCardBackImage()
        {
            return _cardBackImage;
        }
        public static Image GetButtonImage(int state)
        {
            if (state >= 0 && state < _buttonImage.Length)
            {
                return _buttonImage[state];
            }
            return null;
        }
        public static Image GetTitleImage()
        {
            return _titlleImage;
        }
        public static Image GetCreditsBackImage()
        {
            return _creditsBackImage;
        }
        public static Image GetPlayerFrameImage(int index)
        {
            if (index >= 0 && index < 4)
            {
                return _playerFrameImage[index];
            }
            return null;
        }
        public static Image GetPlayerAvatarImage(int index)
        {
            if (index >= 0 && index < 4)
            {
                return _playerAvatarImage[index];
            }
            return null;
        }
        public static void PlayBgm(string bgmKey)
        {
            _currentBgmPlayer = _bgm[bgmKey];
            _currentBgmPlayer.PlayLooping();
        }
        public static void StopBgm()
        {
            _currentBgmPlayer?.Stop();
        }
    }
}
