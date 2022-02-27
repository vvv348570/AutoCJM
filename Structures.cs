using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AutoCJM
{
    /// <summary>
    /// ������ "�����" CJM
    /// </summary>
    internal class Map
    {
        /// <summary>
        /// �������� � ���� ���������� �������
        /// </summary>
        private List<List<string>> plainMap = new List<List<string>>()
        {
            { new List<string>() { "������: 5 - �������" } },
            { new List<string>() { "������: 4 - ������" } },
            { new List<string>() { "������: 3 - ������" } },
            { new List<string>() { "������: 2 - �����" } },
            { new List<string>() { "������: 1 - ������" } },
            { new List<string>() { " " } }, // ��� ������� ������������ ��� �������� �����, ������� ��� ������
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
        /// ���������� CSV ����� �� ������ CJM � ���������� ��� � ����
        /// </summary>
        /// <param name="name">�������� �����</param>
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
                // ������ ������, ��� fallback ���
                if(name != "AutoCJM")
                    name = "AutoCJM";
                else
                {
                    // Fallback ��� ��� �����������, �������� �� ������
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