using System.Collections.Generic;
using System.IO;

namespace AutoCJM
{
    /// <summary>
    /// Объект "карты" CJM
    /// </summary>
    internal class Map
    {
        /// <summary>
        /// Табличка в виде двумерного массива
        /// </summary>
        private List<List<string>> plainMap = new List<List<string>>()
        {
            { new List<string>() { "Эмоции: 5 - Отлично" } },
            { new List<string>() { "Эмоции: 4 - Хорошо" } },
            { new List<string>() { "Эмоции: 3 - Средне" } },
            { new List<string>() { "Эмоции: 2 - Плохо" } },
            { new List<string>() { "Эмоции: 1 - Ужасно" } },
            { new List<string>() { " " } }, // Эта строчка используется для описания шагов, поэтому без эмоции
        };

        public void AddStep(string upText, string downText, int rating)
        {
            for (int i = 0; i <= 4; i++)
            {
                if (i == 5 - rating)
                {
                    plainMap[5 - rating].Add(upText);
                }
                else
                {
                    plainMap[i].Add("");
                }
            }
            plainMap[5].Add(downText);
        }

        /// <summary>
        /// Генерирует CSV карту на основе CJM и записывает это в файл
        /// </summary>
        /// <param name="name">Название файла</param>
        public string Build(string name = "CJM.csv")
        {
            int retries = 1;
        FileOpen:
            try
            {
                if (File.Exists(name))
                {
                    throw new IOException();
                }

                StreamWriter file = new StreamWriter(name, false, encoding: System.Text.Encoding.UTF8);
                for (int i = 0; i < plainMap.Count; i++)
                {
                    for (int j = 0; j < plainMap[i].Count; j++)
                    {
                        file.Write(plainMap[i][j]);
                        file.Write(";");
                    }
                    file.Write("\n");
                }
                file.Flush();
            }
            catch (IOException)
            {
                // Мы обязательно найдём свободное название...
                name = $"CJM-{retries++}.csv";
                goto FileOpen;
            }
            return name;
        }
    }
}