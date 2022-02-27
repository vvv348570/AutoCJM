using System.Collections.Generic;
using System.IO;

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
                // �� ����������� ����� ��������� ��������...
                name = $"CJM-{retries++}.csv";
                goto FileOpen;
            }
            return name;
        }
    }
}