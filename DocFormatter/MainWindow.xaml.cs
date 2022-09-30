using System;
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
            Doc.Text = "summary";
            SplitSymbol.Text = ".";
        }

        private void Input_OnTextChanged(object Sender, TextChangedEventArgs E)
        {
            if (Sender is not TextBox { })
                return;
            if (autoBox.IsChecked==true)
                if (Input.Text.Contains("```ts"))
                {

                    SplitSymbol.Text = ",";
                    addBr.IsChecked = false;
                    Doc.Text = "code";
                    StartWith.Text = "\\t";
                }
                else
                {
                    SplitSymbol.Text = ".";
                    addBr.IsChecked = true;
                    Doc.Text = "summary";
                    StartWith.Text = string.Empty;

                }
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
                    default:
                        new_text = new_text.Replace(symbol, string.Empty);
                        continue;
                }
            }

            new_text = new_text.Trim();
            var split = SplitSymbol.Text;
            var doc_type = Doc.Text;
            var start_with = StartWith.Text;
            if (start_with.Contains("\\t"))
                start_with = start_with.Replace("\\t", "\t");

            string result = string.Empty;
            var last_symbol_in_row = this.addBr.IsChecked == true ? @"<br/>" : string.Empty;
            if (!string.IsNullOrWhiteSpace(split))
            {
                var rows = new_text.Split(split).Select(c => c.Trim()).Where(c => !string.IsNullOrWhiteSpace(c));
                result = $@"/// <{doc_type}>{Environment.NewLine}";
                result = rows.Aggregate(result, (current, row) => current + $"/// {start_with}{row.Trim()}{split}{last_symbol_in_row}{Environment.NewLine}");

                var sub_length = split.Length + last_symbol_in_row.Length + (addBr.IsChecked == true ? 1 : 2);

                result = result.Substring(0, result.Length - sub_length);
                result += $"{Environment.NewLine}/// </{doc_type}>\n";
                //Output.Text = result;


            }
            else
            {
                result = $@"/// <{doc_type}>{Environment.NewLine}";
                result += $"/// {start_with}{new_text}{split}{last_symbol_in_row}{Environment.NewLine}";

                result += $"{Environment.NewLine}/// </{doc_type}>";
                //Output.Text = result;

            }
            if (doc_type == "code")
            {
                string sub = string.Empty;
                var symb = "{";
                var arr = result.Split(symb);

                result = arr.Aggregate(sub, (current, row) => current + $"{row.Trim()}{symb}{Environment.NewLine}");
                result = result.Substring(0, result.Length - 3);
                symb = "}";
                arr = result.Split(symb);
                result = arr.Aggregate(sub, (current, row) => current + $"{row.Trim()}{symb}");
                result = result.Substring(0, result.Length - 1);

                arr = result.Split(Environment.NewLine);

                for (var i = 0; i < arr.Length; i++)
                {
                    if (arr[i].Contains("```ts"))
                    {
                        arr[i] = arr[i].Replace('\t', ' ');
                    }

                    if (arr[i].EndsWith("```"))
                        arr[i] = arr[i].Replace("```", $"{Environment.NewLine}/// ```");
                    if (arr[i].StartsWith($"///"))
                        continue;
                    arr[i] = arr[i].Insert(0, $"/// {start_with}");
                }

                result = arr.Aggregate(sub, (current, row) => current + $"{row}{Environment.NewLine}");

            }
            Output.Text = result;

            try
            {
                Clipboard.SetText(result);
                Status.Text = Clipboard.GetText();
            }
            catch (Exception)
            {
                Status.Text = "Error to copy to clipboard";
            }
        }

        private void AddBr_OnClick(object Sender, RoutedEventArgs E) { DoWork(Input.Text); }
    }
}
