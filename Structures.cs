using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

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
        public string Build(string name)
        {
            if(!Utils.ValidateFileName(name, out string corrName))
            {
                if (string.IsNullOrWhiteSpace(corrName))
                {
                    name = "AutoCJM";
                }
                name = corrName;
            }
            FileStart:
            try
            {
                StreamWriter file = new StreamWriter($"{name}.csv", false, encoding: Encoding.UTF8);
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
            catch
            {
                // Ошибка записи, даём fallback имя
                if(name != "AutoCJM")
                    name = "AutoCJM";
                else
                {
                    // Fallback уже был использован, сообщаем об ошибке
                    throw new InvalidOperationException();
                }
                goto FileStart;
            }
            return name;
        }
    }

    public static class Utils
    {
        public static bool ValidateFileName(string name)
        {
            try
            {
                FileStream fs = File.Open(name, FileMode.Open);
                if (fs != null) fs.Close();
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (FileNotFoundException)
            {
                return true;
            }
            catch (IOException)
            {
                return true;
            }
            return true;
        }
 
        public static bool ValidateFileName(string name, out string corrected_name)
        {
            try
            {
                FileStream fs = File.Open(name, FileMode.Open);
                if (fs != null) fs.Close();
            }
            catch (ArgumentException)
            {
                char[] banned = Path.GetInvalidFileNameChars();
                StringBuilder sb = new StringBuilder();

                foreach (char c in name)
                {
                    if (banned.Contains(c)) sb.Append('_');
                    else sb.Append(c);
                }

                if (ValidateFileName(sb.ToString()) != false)
                {
                    corrected_name = sb.ToString();
                }
                else corrected_name = "";
                return false;
            }
            catch (FileNotFoundException)
            {
                corrected_name = "";
                return true;
            }
            catch (IOException)
            {
                corrected_name = "";
                return true;
            }
            corrected_name = "";
            return true;
        }
    }
}