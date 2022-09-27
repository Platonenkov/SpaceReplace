using System;
using System.Collections.Generic;
using System.Linq;
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
            Symbols.Text = @"/** /* **/ */ * \n\r \n \r";
        }

        private void Input_OnTextChanged(object Sender, TextChangedEventArgs E)
        {
            if (Sender is not TextBox { })
                return;

            DoWork(Input.Text);
        }

        private void DoWork(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                if (Output is not null)
                    Output.Text = string.Empty;
                if (Status is not null)
                    Status.Text = string.Empty;
                return;
            }

            var xchange = Symbols.Text;
            if (string.IsNullOrWhiteSpace(xchange))
                return;

            var symbols = xchange.Split(' ').Select(c => c.Trim()).Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
            symbols.Add("  ");
            symbols.Add("   ");
            symbols.Add("    ");
            symbols.Add("     ");

            var new_text = text;
            foreach (var symbol in symbols)
            {
                switch (symbol)
                {
                    case "\\n":
                        new_text = new_text.Replace('\n', ' ');
                        continue;
                    case "\\r":
                        new_text = new_text.Replace('\r', ' ');
                        continue;
                    case "  " or "   " or "    " or "     ":
                        new_text = new_text.Replace(symbol, " ");
                        continue;
                    default: new_text = new_text.Replace(symbol, string.Empty);
                        continue;
                }
            }

            new_text = new_text.Trim();
            Output.Text = new_text;
            try
            {
                Clipboard.SetText(new_text);
                Status.Text = Clipboard.GetText();
            }
            catch (Exception)
            {
                Status.Text = "Error to copy to clipboard";
            }
        }
    }
}
