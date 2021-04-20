using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Compiler
{
    public partial class Form1 : Form
    {
        List<DocPage> Pages;
        LocalisationController localisation;
        CodeHandler codeHandler;
        private string CopyBuffer;

        public Form1()
        {
            InitializeComponent();
        }
        private void SaveFile(int pageNumber)
        {
            Pages[pageNumber].Save();
            PagesTab.TabPages[pageNumber].Text = Pages[pageNumber].Title;
        }
        private void SaveAsFile(int pageNumber)
        {
            Pages[pageNumber].SaveAs();
            PagesTab.TabPages[pageNumber].Text = Pages[pageNumber].Title;
        }
        private void UpdateText(object sender, EventArgs e)
        {
            Pages[PagesTab.SelectedIndex].Text = CodeField.Text;
            UpdateInterface();
        }
        private void CreateClick(object sender, EventArgs e)
        {
            Pages.Add(new DocPage());
            PagesTab.TabPages.Add(new TabPage(Pages[Pages.Count - 1].Title));
            PagesTab.SelectedIndex = Pages.Count - 1;
            UpdateInterface();
        }
        private void OpenClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.Cancel)
                return;
            try
            {
                Pages.Add(DocPage.OpenFromFile(openFileDialog.FileName));
                PagesTab.TabPages.Add(new TabPage(Pages[Pages.Count - 1].Title));
            }
            catch (Exception err)
            {
                ResultField.Text = err.Message;
            }
            PagesTab.SelectedIndex = Pages.Count - 1;
            UpdateInterface();
        }

        private void SaveClick(object sender, EventArgs e)
        {
            SaveFile(PagesTab.SelectedIndex);
        }

        private void SaveAsClick(object sender, EventArgs e)
        {
            SaveAsFile(PagesTab.SelectedIndex);
        }

        private void ExitClick(object sender, EventArgs e)
        {
            Close();
        }
        private void UpdateInterface()
        {
            if (PagesTab.SelectedIndex == -1)
            {
                RepeatButton.Enabled = false;
                BackButton.Enabled = false;
                RowsNumbers.Text = "";
                return;
            }

            RepeatButton.Enabled = Pages[PagesTab.SelectedIndex].CanRepeat;
            BackButton.Enabled = Pages[PagesTab.SelectedIndex].CanCancel;

            string numbers = "";
            int start = CodeField.GetLineFromCharIndex(CodeField.GetCharIndexFromPosition(new Point(0, 0)));
            int end = CodeField.GetLineFromCharIndex(CodeField.GetCharIndexFromPosition(new Point(CodeField.Size.Width, CodeField.Size.Height)));
            for (int i = start; i <= end; i++)
                numbers += (i+1) + ":\n";

            RowsNumbers.Text = numbers;

            codeHandler.HandleText();
        }
        private void TabChanged(object sender, EventArgs e)
        {
            if (PagesTab.SelectedIndex == -1)
            {
                CodeField.Text = "";
                ResultField.Text = "";
                UpdateInterface();
                return;
            }
            CodeField.Text = Pages[PagesTab.SelectedIndex].Text;
            ResultField.Text = Pages[PagesTab.SelectedIndex].ResultText;
            UpdateInterface();
        }

        private void CloseForm(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < Pages.Count; i++)
                if (!Pages[i].Close())
                {
                    e.Cancel = true;
                    return;
                }
        }
        private void CancelClick(object sender, EventArgs e)
        {
            Pages[PagesTab.SelectedIndex].CancelState();
            int index = CodeField.SelectionStart;
            CodeField.Text = Pages[PagesTab.SelectedIndex].Text;
            UpdateInterface();
        }

        private void RepeatClick(object sender, EventArgs e)
        {
            Pages[PagesTab.SelectedIndex].RepeatState();
            CodeField.Text = Pages[PagesTab.SelectedIndex].Text;
            UpdateInterface();
        }

        private void CutClick(object sender, EventArgs e)
        {
            if (CodeField.SelectionLength == 0)
                return;
            int SelectionStart = CodeField.SelectionStart;
            CopyBuffer = CodeField.SelectedText;
            CodeField.Text = CodeField.Text.Remove(CodeField.SelectionStart, CodeField.SelectionLength);
            CodeField.SelectionStart = SelectionStart;
        }

        private void CopyClick(object sender, EventArgs e)
        {
            if (CodeField.SelectionLength == 0)
                return;
            CopyBuffer = CodeField.SelectedText;
        }

        private void PasteClick(object sender, EventArgs e)
        {
            if (CopyBuffer == null)
                return;
            if (CopyBuffer == "")
                return;
            int SelectionStart;
            if(CodeField.SelectionLength != 0)
            {
                SelectionStart = CodeField.SelectionStart;
                CodeField.Text = CodeField.Text.Remove(CodeField.SelectionStart, CodeField.SelectionLength);
                CodeField.SelectionStart = SelectionStart;
            }
            SelectionStart = CodeField.SelectionStart + CopyBuffer.Length;
            CodeField.Text = CodeField.Text.Insert(CodeField.SelectionStart, CopyBuffer);
            CodeField.SelectionStart = SelectionStart;
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            if (CodeField.SelectionLength == 0)
                return;
            int SelectionStart = CodeField.SelectionStart;
            CodeField.Text = CodeField.Text.Remove(CodeField.SelectionStart, CodeField.SelectionLength);
            CodeField.SelectionStart = SelectionStart;
        }

        private void CodeFontUp(object sender, EventArgs e)
        {
            CodeField.Font = new Font(CodeField.Font.FontFamily, Math.Min(CodeField.Font.Size + 1, 20));
        }

        private void CodeFontDown(object sender, EventArgs e)
        {
            CodeField.Font = new Font(CodeField.Font.FontFamily, Math.Max(CodeField.Font.Size - 1, 10));
        }

        private void OutFontUp(object sender, EventArgs e)
        {
            ResultField.Font = new Font(ResultField.Font.FontFamily, Math.Min(ResultField.Font.Size + 1, 20));
        }

        private void OutFontDown(object sender, EventArgs e)
        {
            ResultField.Font = new Font(ResultField.Font.FontFamily, Math.Min(ResultField.Font.Size - 1, 20));
        }

        private void Drop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var fileNames = data as string[];
                for (int i = 0; i < fileNames.Length; i++)
                {
                    try
                    {
                        Pages.Add(DocPage.OpenFromFile(fileNames[i]));
                        PagesTab.TabPages.Add(new TabPage(Pages[Pages.Count - 1].Title));
                    }
                    catch (Exception err)
                    {
                        ResultField.Text = err.Message;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CodeField.AllowDrop = true;
            CodeField.DragDrop += Drop;

            localisation = new LocalisationController("Localisations/Localisations.txt");
            SetLocalisations();
            Locale();

            Pages = new List<DocPage>();
            Pages.Add(new DocPage());
            PagesTab.TabPages.Add(new TabPage(Pages[0].Title));

            BackButton.Enabled = false;
            RepeatButton.Enabled = false;

            RowsNumbers.Font = CodeField.Font;

            codeHandler = new CodeHandler(CodeField);

            UpdateInterface();
        }
        private void SetLocalisations()
        {
            List<string> loclist = localisation.Localisations;
            foreach (string str in loclist)
            {
                ToolStripMenuItem tmp = new ToolStripMenuItem();
                tmp.Name = str + "LangStrip";
                tmp.Click += SetLocalisation;
                tmp.Text = str;
                LocalisationStrip.DropDownItems.Add(tmp);
            }
        }
        private void SetLocalisation(object sender, EventArgs e)
        {
            localisation.CurrentLocalisation = (sender as ToolStripMenuItem).Text;
            Locale();
        }

        private void Locale()
        {
            FileStrip.Text = localisation["Файл"];
            EditStrip.Text = localisation["Правка"];
            TextStrip.Text = localisation["Текст"];
            PlayStrip.Text = localisation["Пуск"];
            toolStripButton1.Text = localisation["Пуск"];
            InfoStrip.Text = localisation["Справка"];
            ViewStrip.Text = localisation["Вид"];
            CreateStrip.Text = localisation["Создать"];
            CreateButton.Text = localisation["Создать"];
            OpenStrip.Text = localisation["Открыть"];
            OpenButton.Text = localisation["Открыть"];
            SaveStrip.Text = localisation["Сохранить"];
            SaveButton.Text = localisation["Сохранить"];
            SaveAsStrip.Text = localisation["Сохранить как"];
            ExitStrip.Text = localisation["Выход"];
            CancelStrip.Text = localisation["Отменить"];
            BackButton.Text = localisation["Отменить"];
            RepeatStrip.Text = localisation["Повторить"];
            RepeatButton.Text = localisation["Повторить"];
            CutStrip.Text = localisation["Вырезать"];
            CutButton.Text = localisation["Вырезать"];
            CopyStrip.Text = localisation["Копировать"];
            CopyButton.Text = localisation["Копировать"];
            PasteStrip.Text = localisation["Вставить"];
            PasteButton.Text = localisation["Вставить"];
            DeleteStrip.Text = localisation["Удалить"];
            SelectAllStrip.Text = localisation["Выделить все"];
            T1Strip.Text = localisation["Постановка задачи"];
            T2Strip.Text = localisation["Грамматика"];
            T3Strip.Text = localisation["Классификация грамматики"];
            T4Strip.Text = localisation["Метод анализа"];
            T5Strip.Text = localisation["Диагностика и нейтрализация ошибок"];
            T6Strip.Text = localisation["Тестовый пример"];
            T7Strip.Text = localisation["Список литературы"];
            T8Strip.Text = localisation["Исходный код программы"];
            CallInfoStrip.Text = localisation["Вызов справки"];
            toolStripButton3.Text = localisation["Вызов справки"];
            toolStripButton2.Text = localisation["О программе"];
            AboutStrip.Text = localisation["О программе"];
            TextSizeStrip.Text = localisation["Размер текста"];
            CodeFieldStrip.Text = localisation["Окно кода"];
            ResultFieldStrip.Text = localisation["Окно вывода"];
            CodeFontUpStrip.Text = localisation["Увеличить шрифт"];
            CodeFontDownStrip.Text = localisation["Уменьшить шрифт"];
            ResultFontUpStrip.Text = localisation["Увеличить шрифт"];
            ResultFontDownStrip.Text = localisation["Уменьшить шрифт"];
            LocalisationStrip.Text = localisation["Локализация"];
            заданиеToolStripMenuItem.Text = localisation["Регулярные выражения"];
            задание12ToolStripMenuItem.Text = localisation["Задание 12"];
            задание15ToolStripMenuItem.Text = localisation["Задание 15"];
            задание19ToolStripMenuItem.Text = localisation["Задание 19"];
            справкаПоЗаданиюToolStripMenuItem.Text = localisation["Информация о заданиях"];

            DocPage.DefaultTitle = localisation["Новый документ"];
        }
        private void About(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\about.html");
        }
        private void Help(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\help.html");
        }
        private void Task(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\main.html");
        }

        private void Grammar(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\grammar.html");
        }

        private void Classification(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\class.html");
        }

        private void Graph(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\graph.html");
        }

        private void Neutralisation(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\neutralisation.html");
        }

        private void Example(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\test.html");
        }

        private void Literature(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\literature.html");
        }

        private void Listing(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\code.html");
        }
        private void SelectAllClick(object sender, EventArgs e)
        {
            CodeField.SelectionStart = 0;
            CodeField.SelectionLength = CodeField.Text.Length;
        }

        private void PagesTabPressDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                int index = PagesTab.SelectedIndex;
                Pages[index].Close();
                Pages.Remove(Pages[index]);
                PagesTab.TabPages.Remove(PagesTab.TabPages[index]);
            }
        }

        private void RTBFontChanged(object sender, EventArgs e)
        {
            RowsNumbers.Font = CodeField.Font;
        }

        private void RTBScroll(object sender, EventArgs e)
        {
            UpdateInterface();
        }

        private void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                SaveButton.PerformClick();
            }
            if (e.KeyCode == Keys.Z && e.Control)
            {
                BackButton.PerformClick();
            }
        }

        String parsedText = null;

        private void CodeField_Click(object sender, EventArgs e)
        {
            if (parsedText != null)
            {
                CodeField.Text = "";
                CodeField.Text = parsedText;
                parsedText = null;
            }          
        }

        private void задание12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResultField.Text = "";
            FA fA = new FA();
            string input = CodeField.Text;
            List<Substring> substrings = fA.Find(input);
            
            foreach(Substring s in substrings)
            {
                ResultField.Text += "Найдена подстрока: " + s.GetStr() + ".\r\n";
                ResultField.Text += "Позиция начала данной подстроки: " + s.GetIdx() + ".\r\n";
            }
            if (substrings.Count == 0)
            {
                ResultField.Text += "Не найдено ни одной подстроки.";
            }
        }

        private void задание15ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find("(00)*(11)*1");
        }

        private void Find(string pattern)
        {
            ResultField.Text = "";
            
            Regex regex = new Regex(pattern);

            string input = CodeField.Text;

            Match match = regex.Match(input);

            if (!match.Success)
            {
                ResultField.Text += "Не найдено ни одной подстроки";
            }

            while(match.Success)
            {
                ResultField.Text += "Была найдена подстрока: " + match.Value + ".\r\n";
                ResultField.Text += "Позиция начала подстроки: " + match.Index + ".\r\n";

                match = regex.Match(input, match.Index + 1);
            }
        }

        private void задание19ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find("(ab*)(1*2)*");
        }

        private void справкаПоЗаданиюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\regexp.html");
        }
    }
}
