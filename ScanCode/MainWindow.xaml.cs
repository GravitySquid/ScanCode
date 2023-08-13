using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Shell;
using System.Diagnostics;
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Emit;

namespace ScanCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _tempPath;
        public MainWindow()
        {
            InitializeComponent();
            _tempPath = System.IO.Path.GetTempPath();
            foldername.Focus();
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            //output.Document.Blocks.Clear();
            //output.AppendText("Searching ...");
            Process proc = new Process();
            proc.StartInfo.FileName = createSearchBatFile();
            proc.StartInfo.WorkingDirectory = _tempPath;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();

            output.Document.Blocks.Clear();
            try
            {
                string outputFilename = _tempPath + "\\CodeScan_Output.txt";
                FlowDocument flowDocument = new FlowDocument();
                using (StreamReader sr = new StreamReader(outputFilename))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.Contains(':'))
                        {
                            int pos = line.IndexOf(':');
                            if (pos != -1)
                            {
                                Paragraph para = new Paragraph();
                                Run run1 = new Run(line.Substring(0, pos + 1));
                                run1.FontWeight = FontWeights.Bold;
                                run1.FontStyle = FontStyles.Italic;
                                run1.Foreground = Brushes.DarkGreen;
                                para.Inlines.Add(run1);
                                line = line.Substring(pos + 1);
                                int nextPos = line.IndexOf(':');
                                Run run2;
                                if (pos != -1)
                                {
                                    run2 = new Run(line.Substring(0, nextPos + 1) + Environment.NewLine);
                                    //run2.FontWeight = FontWeights.Bold;
                                    run2.FontStyle = FontStyles.Italic;
                                    run2.Foreground = Brushes.Indigo;
                                    para.Inlines.Add(run2);
                                    line = line.Substring(nextPos + 1);
                                    para.Inlines.Add(new Run(line));
                                }
                                else
                                {
                                    run2 = new Run(line + Environment.NewLine);
                                    para.Inlines.Add(run2);
                                }
                                flowDocument.Blocks.Add(para);
                            }
                        }
                        else
                        {
                            Paragraph para = new Paragraph();
                            para.Inlines.Add(new Run(line));
                            flowDocument.Blocks.Add(para);
                        }
                    }
                }
                output.Document = flowDocument;
            }
            catch { output.AppendText("\nGot an Error!"); }
        }

        private string createSearchBatFile()
        {
            string batFilename = _tempPath + "\\CodeScan.bat";
            string outputFilename = _tempPath + "\\CodeScan_Output.txt";
            //string target =  foldername.Text + "\"" + filenamePattern.Text;
            if (File.Exists(batFilename)) File.Delete(batFilename);
            if (File.Exists(outputFilename)) File.Delete(outputFilename);

            string parameters = " " + parms.Text.Trim() + " ";
            using (StreamWriter sw = new StreamWriter(batFilename))
            {
                //sw.WriteLine("@echo on");
                sw.WriteLine("cd " + foldername.Text);
                sw.WriteLine("findstr" + parameters + "/C:\"" + searchString.Text + "\" " + filenamePattern.Text + " > " + outputFilename);
                //sw.WriteLine("wait 5");
                sw.Close();
            }
            return batFilename;
        }

        private void folderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            foldername.Text = folderBrowserDialog.SelectedPath;
        }


    }
}
