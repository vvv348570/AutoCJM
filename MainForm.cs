using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoCJM
{
    public partial class MainForm : Form
    {
        public static readonly string botName = "CJ";
        // cs = Current Step
        public static int cs = -2;
        public static int step = 1;
        private Map cjm = new Map();
        public static string upText = "";
        public static string downText = "";
        public static int rating;

        public MainForm()
        {
            InitializeComponent();
            TheoryGroupBox.Dock = DockStyle.Fill;
            praticeGroupBox.Dock = DockStyle.Fill;
            textPanel.Location = new Point(334, 190);
            chat.AppendText("Добро пожаловать в тестовую систему AutoCJM!", Color.Blue);
            chat.AppendText(Environment.NewLine);
            chat.Bot("Используй поле ввода ниже для отправки сообщений!");
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void theoryButton_Click(object sender, EventArgs e)
        {
            TheoryGroupBox.Show();
            praticeGroupBox.Hide();
            textPanel.Hide();
        }

        private void practiceButton_Click(object sender, EventArgs e)
        {
            TheoryGroupBox.Hide();
            praticeGroupBox.Show();
            textPanel.Hide();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (textPanel.Visible)
            {
                textPanel.Location = new Point(((Size.Width - 165) / 2) + 165 / 2, Size.Height / 2 - 50);
            }
        }

        // ######################### ГЛАВНАЯ ЧАСТЬ КОДА ############################
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift && !string.IsNullOrWhiteSpace(inputBox.Text))
            {
                chat.User(inputBox.Text);
                ExecuteStep(inputBox.Text);
                inputBox.Text = string.Empty;
            }
            // Предотвращаем создание новой строки
            if (e.KeyCode == Keys.Enter && (string.IsNullOrWhiteSpace(inputBox.Text) || e.Shift))
            {
                e.SuppressKeyPress = true;
            }
        }

        /* Выполнение кода разделено на этапи, и так как
         * WinForms не умеет в асинхронность (и я тоже)
         * я просто запускаю шаги как только пользователь
         * что-то вводит.
         */
        private void ExecuteStep(string input)
        {
            if (cs != 2)
            {
                chat.AppendText(Environment.NewLine);
            }
            bool moveNext = true;
            if (cs == -2)
            {
                chat.Bot("Отлично! Вы уже успели ознакомиться с теорией?");
                chat.Bot("Я - Си Джи, помогу вам в составлении простого CJM");
                cs = -1;
            }
            if (cs == -1)
            {
                chat.Bot("Для начала определимся с названием");
                chat.AppendText($"Как называется ваш CJM?", Color.Green);
            }
            // Шаг
            else if (cs == 0)
            {
                chat.Bot($"Опиши, каким был текущий шаг путешествия\nСейчас описывается шаг #{step}");
            }
            // Рейтинг
            else if (cs == 1)
            {
                upText = input.Trim();
                chat.Bot($"Каким было твоё настроение от 1 до 5 на этом шаге?\nГде 1 - Ужасно, а 5 - Отлично");
            }
            else if (cs == 2)
            {
                try
                {
                    rating = int.Parse(input);
                }
                catch (FormatException)
                {
                    chat.AppendText("! ", Color.Red, false);
                    chat.AppendText("Пожалуйста, введите только число, например, \"4\"", Color.YellowGreen);
                    moveNext = false;
                    return;
                }
                if (rating < 1 || rating > 5)
                {
                    chat.AppendText("! ", Color.Red, false);
                    chat.AppendText("Это число не лежит в диапазоне [1 - 5]!", Color.YellowGreen);
                    moveNext = false;
                    return;
                }
                cs++;
            }
            if (cs == 3)
            {
                chat.AppendText(Environment.NewLine);
                chat.Bot("Теперь можешь описать причину, что вызвало у тебя такое настроение?");
            }
            else if (cs == 4)
            {
                downText = input;
                chat.Bot("Взгляни на диалоговое окно. Всё указано верно?");
                DialogResult result = MessageBox.Show($"[ Вот наш сгенерированный шаг: ]\n\nШаг: {upText}\nНастроение на шаге: {rating}/5 [{StrRating(rating)}]\nПричина настроения: {downText}\nДобавляем этот шаг в CJM?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    cjm.AddStep(upText, downText, rating);
                    chat.User("Да - Добавить этот шаг");
                    chat.Bot($"Отлично! Я добавил шаг #{step++} в наш CJM");
                }
                else
                {
                    chat.User("Нет - Переписать этот шаг");
                    chat.Bot($"Хорошо, заполним шаг #{step} сначала?");
                }
                cs = 0;

                if (step > 3)
                {
                    chat.AppendText(Environment.NewLine);
                    chat.Bot($"Приступаем к следующему шагу?");
                    DialogResult result2;
                    string showString = $"В CJM сейчас есть {step - 1} шагов\n [ Добавляем ещё один шаг? ]";
                    if (step - 1 < 5)
                    {
                        showString = $"В CJM сейчас есть {step - 1} шага\n [ Добавляем ещё один шаг? ]";
                    }

                    result2 = MessageBox.Show(showString, "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result2 != DialogResult.Yes)
                    {
                        chat.User("Нет, CJM нужно сохранить.");
                        string filename = cjm.Build();
                        chat.AppendText($"Файл был сохранён под названием {filename} в папке с программой", Color.Green);
                        chat.AppendText($"Если вы хотите составить ещё один CJM, напишите что-нибудь в чат", Color.Green);
                        cs = -2;
                    }
                    else
                    {
                        chat.User("Да, добавляем ещё один шаг");
                    }
                }
                if (cs != -2)
                {
                    chat.AppendText(Environment.NewLine);
                    chat.Bot($"Опиши, каким был текущий шаг путешествия\nСейчас описывается шаг #{step}");
                }
            }
            if (moveNext)
            {
                cs++;
            }
        }
        // ################################ КОНЕЦ ######################################

        /// <summary>
        /// Бесполезная функция для преобразования числа в строку
        /// </summary>
        public static string StrRating(int rating)
        {
            // В теории можно было сделать словать таких значений, но такое будет нечитабельно
            if (rating == 1)
            {
                return "Ужасно";
            }

            if (rating == 2)
            {
                return "Плохо";
            }

            if (rating == 3)
            {
                return "Нормально";
            }

            if (rating == 4)
            {
                return "Хорошо";
            }

            if (rating == 5)
            {
                return "Отлично";
            }

            return "";
        }

    }

    public static class RichTextBoxExtensions
    {
        /// <summary>
        /// Доавляет текст в поле чата с определённым цветом
        /// </summary>
        public static void AppendText(this RichTextBox box, string text, Color color, bool addNewLine = true)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(addNewLine
    ? $"{text}{Environment.NewLine}"
    : text);

            box.SelectionColor = box.ForeColor;
            box.ScrollToCaret();
        }

        /// <summary>
        /// Доавляет текст в поле чата от имени бота со стандартным цветом
        /// </summary>
        public static void Bot(this RichTextBox box, string text)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = Color.ForestGreen;
            box.AppendText($"{MainForm.botName}: ");

            box.SelectionColor = box.ForeColor;
            box.AppendText($"{text}{Environment.NewLine}");
            box.ScrollToCaret();
        }

        /// <summary>
        /// Доавляет текст в поле чата от имени пользователя со стандартным цветом
        /// </summary>
        public static void User(this RichTextBox box, string text)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = Color.DeepSkyBlue;
            box.AppendText("Вы: ");

            box.SelectionColor = box.ForeColor;
            box.AppendText($"{text}{Environment.NewLine}");
            box.ScrollToCaret();
        }
    }
}
