using FoolGame.Deck;
using FoolGame.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoolGame
{
    public partial class TableShell : Form
    {
        readonly Sever Server;

        public TableShell()
        {
            InitializeComponent();

            InitHint();
            InitSettings();

            Server = new Sever(
                    (message) => PrintLogs(message),
                    (textBox, cards) => PrintCardsTextBox(textBox, cards),
                    new List<RichTextBox>
                    {
                        Trump,
                        User1,
                        User2,
                        Deck,
                        Droping,
                        Table
                    }
                );

            StrokeIndex.Text = "0";

        }

        /// <summary>
        /// Вывод карт в текстовое поле
        /// </summary>
        void PrintCardsTextBox(RichTextBox tb, List<PlayingCard> cards)
        {
            tb.Clear();
            int n = 0;
            foreach (var card in cards)
            {
                tb.Text += $"{card}\t({n})\r\n";
                if (tb == Table && (n + 1) % 2 == 0)
                {
                    tb.Text += "\r\n";
                }
                n++;
            }
            tb.Text += $"{cards.Count}";
        }


        /// <summary>
        /// Отправить сообщение в Log
        /// </summary>
        private void PrintLogs(string message, bool isClear = false)
        {
            if (isClear)
            {
                Logs.Clear();
            }
            Logs.Text += DateTime.Now.ToString("HH:mm:ss") + "\t" + message + "\r\n";

            Logs.SelectionStart = Logs.Text.Length;
            Logs.ScrollToCaret();
        }

        /// <summary>
        /// Инициализация настроек
        /// </summary>
        private void InitSettings()
        {
            // настройки редактирования полей
            User1.ReadOnly = true;
            User2.ReadOnly = true;
            Table.ReadOnly = true;
            Deck.ReadOnly = true;
            Droping.ReadOnly = true;
            Trump.ReadOnly = true;
            Logs.ReadOnly = true;
        }

        /// <summary>
        /// Инициализация подсказок для объектов
        /// </summary>
        private void InitHint()
        {
            Hint.SetToolTip(User1, "Рука Player 1");
            Hint.SetToolTip(User2, "Рука Player 2");
            Hint.SetToolTip(Table, "Игровой стол");
            Hint.SetToolTip(Deck, "Колода карт");
            Hint.SetToolTip(Droping, "Отбой");
            Hint.SetToolTip(Trump, "Козырь");
            Hint.SetToolTip(Logs, "Поле логов");
            Hint.SetToolTip(StrokeIndex, "Поле: указать индекс карты для хода");

            Hint.SetToolTip(button1, "Сделать ход");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Server.MakeMove(StrokeIndex.Text.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Server.MakeMove("-1");
        }
    }
}
