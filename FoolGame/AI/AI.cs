using FoolGame.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoolGame.AI
{
    abstract class AI
    {
        /// <summary>
        /// Вызов "действия" у объекта
        /// </summary>
        /// <param name="hand">Карты на руке у объекта</param>
        /// <param name="table">Карты на столе</param>
        /// <param name="countDroping">Количество оставшихся карт в колоде</param>
        /// <returns>Новое состояние входных параметров</returns>
        public abstract (List<PlayingCard> hand, List<PlayingCard> table) MakeMove(List<PlayingCard> hand, List<PlayingCard> table, int countDroping);

        /// <summary>
        /// Проверка: равны ли карты по значению (вес и масть отброшены)
        /// </summary>
        /// <param name="card1">Карта для сравнения 1</param>
        /// <param name="card2">Карта для сравнения 2</param>
        /// <returns></returns>
        public bool CheckingCardValues(PlayingCard card1, PlayingCard card2)
        {
            return card1.value == card2.value;
        }

        /// <summary>
        /// Перенести карту из одного списка в другой
        /// </summary>
        /// <param name="fromList">Список источник</param>
        /// <param name="toList">Список получатель</param>
        /// <param name="index">Индекс переходной карты</param>
        public void Moving(List<PlayingCard> fromList, List<PlayingCard> toList, int index)
        {
            toList.Add(fromList[index]);
            fromList.RemoveAt(index);
        }

        /// <summary>
        /// Перенести все карты из одного списка в другой
        /// </summary>
        /// <param name="fromList">Список источник</param>
        /// <param name="toList">Список получатель</param>
        public void Moving(List<PlayingCard> fromList, List<PlayingCard> toList)
        {
            toList.AddRange(fromList);
            fromList.Clear();
        }
    }
}
