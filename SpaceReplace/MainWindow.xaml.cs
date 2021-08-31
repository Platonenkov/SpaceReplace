using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Space = SpaseType.Underline;
        }

        enum SpaseType
        {
            CamelCase,
            Underline,
            Hyphen
        }

        private SpaseType Space;

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is not RadioButton { } radio)
                return;
            switch ((string)radio.Content)
            {
                case "CamelCase": Space = SpaseType.CamelCase;
                    break;
                case "_": Space = SpaseType.Underline;
                    break;
                default: Space = SpaseType.Hyphen;
                    break;
            }
            DoWork(Input?.Text);
        }

        private void Input_OnTextChanged(object Sender, TextChangedEventArgs E)
        {
            if (Sender is not TextBox { } input)
                return;
            DoWork(input.Text);
        }

        private void DoWork(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                if(Output is not null)
                    Output.Text = string.Empty;
                if(Status is not null)
                    Status.Text = string.Empty;
                return;
            } 
            var new_text = string.Empty;
            switch (Space)
            {
                case SpaseType.CamelCase:
                {
                    Output.Text = ReplaceText(text, ' ', '_');
                    for (var i = 0; i < text.Length; i++)
                    {
                        var current = text[i];
                        if (current == ' ')
                            continue;
                        if (i == 0 || i > 0 && text[i - 1] == ' ')
                        {
                            new_text += $"{current}".ToUpper();
                            continue;
                        }

                        new_text += current;
                    }


                    break;
                }
                case SpaseType.Underline:
                {
                    new_text = ReplaceText(text, ' ', '_');
                    break;
                }
                case SpaseType.Hyphen:
                {
                    new_text = ReplaceText(text, ' ', '-');
                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }
            Output.Text = new_text;
            try
            {
                Clipboard.SetText(new_text);
                Status.Text = Clipboard.GetText();
            }
            catch (Exception)
            {
                Status.Text = "Ошибка копирования текста в буфер";
            }
        }

        private string ReplaceText(string input, char target, char change) => input.Replace(target, change);
    }
}
