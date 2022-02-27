using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AutoCJM
{
    public partial class MainForm : Form
    {
        public static readonly string botName = "CJ";
        public static string filename = "";
        public static int currentStep = -2;
        public static int CJMStep = 1;
        private Map CJMMap = new Map();
        public static string stepText = "";
        public static string reasonText = "";
        public static int rating;

        public MainForm()
        {
            InitializeComponent();
            TheoryGroupBox.Dock = DockStyle.Fill;
            praticeGroupBox.Dock = DockStyle.Fill;
            textPanel.Location = new Point(((Size.Width - 165) / 2) + 165 / 2, Size.Height / 2 - 50);
            chat.AppendText("Добро пожаловать в тестовую систему AutoCJM!", Color.Blue);
            chat.AppendText(Environment.NewLine);
            chat.Bot("Для начала работы напиши любое сообщение через поле ввода ниже");
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
         * что-то вводит. */
        private void ExecuteStep(string input)
        {
            if (currentStep != 2)
            {
                chat.AppendText(Environment.NewLine);
            }
            bool moveNext = true;
            if (currentStep == -2)
            {
                chat.Bot("Отлично! Вы уже успели ознакомиться с теорией?");
                chat.Bot("Я - Си Джи, помогу вам в составлении простого CJM");
                currentStep = -1;
            }
            if (currentStep == -1)
            {
                chat.Bot("Для начала определимся с названием");
                chat.AppendText($"Как называется ваш CJM? Рекомендуется назвать его короткой фразой из 2-5 слов", Color.Green);
            }
            // Шаг
            else if (currentStep == 0)
            {
                if (input.Length > 64)
                {
                    chat.AppendText("Имя файла слишком большое! Оно было сокращено до первых 64 символов.", Color.Red);
                    filename = $"{input[0..^64]}";
                    nameLabel.Text = $"Название CJM:\n{input[0..^64]}";
                }
                else
                {
                    filename = $"{input}";
                    nameLabel.Text = $"Название CJM:\n{input}";

                }
                chat.Bot($"Опиши, каким был текущий шаг путешествия:");
            }
            // Оценка шага
            else if (currentStep == 1)
            {
                stepText = input.Trim();
                chat.Bot($"Каким было твоё настроение от 1 до 5 на этом шаге?\nГде 1 - Ужасно, а 5 - Отлично");
            }
            else if (currentStep == 2)
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
                currentStep++;
            }
            if (currentStep == 3)
            {
                chat.AppendText(Environment.NewLine);
                chat.Bot("Теперь можешь описать причину, что вызвало у тебя такое настроение?");
            }
            else if (currentStep == 4)
            {
                reasonText = input;
                chat.Bot("Взгляни на диалоговое окно. Всё указано верно?");
                DialogResult result = MessageBox.Show($"[ Вот наш сгенерированный шаг: ]\n\nШаг: {stepText}\nНастроение на шаге: {rating}/5 [{StrRating(rating)}]\nПричина настроения: {reasonText}\nДобавляем этот шаг в CJM?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    CJMMap.AddStep(stepText, reasonText, rating);
                    chat.User("Да - Добавить этот шаг");
                    chat.Bot($"Отлично! Я добавил шаг #{CJMStep++} в наш CJM");
                }
                else
                {
                    chat.User("Нет - Переписать этот шаг");
                    chat.Bot($"Хорошо, заполним шаг #{CJMStep} сначала?");
                }
                currentStep = 0;

                if (CJMStep == 3)
                {
                    chat.AppendText(Environment.NewLine);
                    chat.Bot($"Приступаем к следующему шагу.");
                    chat.AppendText("Минимальное количество шагов записано!\nЕсли ты желаешь сохранить CJM, в любой момент ты можешь нажать на кнопку \"Завершить и сохранить\" справа", Color.Green);
                    saveButton.Enabled = true;
                }
                if (currentStep != -2)
                {
                    chat.AppendText(Environment.NewLine);
                    chat.Bot($"Опиши, каким был текущий шаг путешествия:");
                }
            }
            if (moveNext)
            {
                currentStep++;
                stepsLabel.Text = $"Количество шагов: {CJMStep - 1}";
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

        private void saveButton_Click(object sender, EventArgs e)
        {
            string askString = $"Вы хотите сохранить текущий CJM \"{filename}\"?";
            if (currentStep > 1)
            {
                askString += $"\nТекущий недозаполненный шаг \"{stepText}\" будет потерян!";
            }
            DialogResult result = MessageBox.Show(askString, "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                try
                {
                    string RedactedName = CJMMap.Build(filename);
                    inputBox.Text = "";
                    filename = "";
                    currentStep = -1;
                    CJMStep = 1;
                    stepsLabel.Text = "Количество шагов: 0";
                    chat.AppendText(Environment.NewLine);
                    if (RedactedName.StartsWith("AutoCJM"))
                    {
                        chat.AppendText("Похоже что нам не удалось сохранить файл под указанным названием и я дал ему автоматическое имя", Color.Red);
                    }
                    chat.AppendText($"CJM был сохранён в файл \"{RedactedName}.csv\" в папке с программой", Color.Green);
                    chat.AppendText($"Чтобы составить ещё один CJM, напишите что-нибудь в чат!", Color.Green);
                    saveButton.Enabled = false;
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Возникла ошибка записи в файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    public static class RichTextBoxExtensions
    {
        /// <summary>
        /// Добавляет текст в поле чата с определённым цветом
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
