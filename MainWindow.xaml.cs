using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;

namespace TestSTC
{
    /// <summary>
    /// Contains all methods for performing basic operations
    /// </summary>
    public partial class MainWindow : Window
    {
        private Reader reader;
        private Writer writer;
        /// <summary>
        /// this Main class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// this evet Btn_Start_Click
        /// </summary>
        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if (tb_SourceFile.Text == "")
            {
                MessageBox.Show("Source File is not defined");
                return;
            }
            if (tb_DestFile.Text == "")
            {
                MessageBox.Show("Destanation File is not defined");
                return;
            }
            try
            {
                Buffer buffer = new Buffer(int.Parse(tb_SizeBuffer.Text));
                reader = new Reader(tb_SourceFile.Text, buffer, int.Parse(tb_SpeedReading.Text));
                writer = new Writer(tb_DestFile.Text, buffer, int.Parse(tb_SpeedWriting.Text));

                reader.Start();
                lb_StatusRead.Content = "Reader thread is running";
                lb_StatusRead.Foreground = Foreground = System.Windows.Media.Brushes.Green;
                writer.Start();
                lb_StatusWrite.Content = "Writer thread is running";
                lb_StatusWrite.Foreground = Foreground = System.Windows.Media.Brushes.Green;
                buffer.onChange += updateProgressBar;
                reader.LabelRead += setLabelRead;
                writer.LabelWrite += setLabelWrite;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex.Message);
            }
        }

        private void setLabelWrite(string str)
        {
            //event for set value for Label
            Action action = () => { lb_StatusWrite.Content = str; };
            lb_StatusWrite.Dispatcher.Invoke(action);
        }
        private void setLabelRead(string str)
        {
            //event for set value for Label
            Action action = () => { lb_StatusRead.Content = str; };
            lb_StatusRead.Dispatcher.Invoke(action);
        }
        private void updateProgressBar(int value)
        {
            //event for set value for progressbar (status fulling of buffer)
            Action action = () => { prog_Bar.Value = value; };
            prog_Bar.Dispatcher.Invoke(action);
        }

        private void Btn_StopRead_Click(object sender, RoutedEventArgs e)
        {
            //check that thread is running
            if (reader != null)
            {
                reader.pause();
                btn_StopRead.IsEnabled = false;
                btn_ResumeRead.IsEnabled = true;
                lb_StatusRead.Content = "Reader thread is stopped";
                lb_StatusRead.Foreground = Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                MessageBox.Show("Thread is not running");
            }
        }

        private void Btn_ResumeRead_Click(object sender, RoutedEventArgs e)
        {
            reader.resume();
            btn_ResumeRead.IsEnabled = false;
            btn_StopRead.IsEnabled = true;
            lb_StatusRead.Content = "Reader thread is running";
            lb_StatusRead.Foreground = Foreground = System.Windows.Media.Brushes.Green;

        }

        private void Btn_ResumeWrite_Click(object sender, RoutedEventArgs e)
        {
            writer.resume();
            btn_ResumeWrite.IsEnabled = false;
            btn_StopWrite.IsEnabled = true;
            lb_StatusWrite.Content = "Writer thread is running";
            lb_StatusWrite.Foreground = Foreground = System.Windows.Media.Brushes.Green;
        }
        
        private void Btn_StopWrite_Click(object sender, RoutedEventArgs e)
        {
            //check that thread is running
            if (reader != null)
            {
                writer.pause();
                btn_StopWrite.IsEnabled = false;
                btn_ResumeWrite.IsEnabled = true;
                lb_StatusWrite.Content = "Writer thread is stopped";
                lb_StatusWrite.Foreground = Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                MessageBox.Show("Thread is not running");
            }
        }

        private void Btn_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FilterIndex = 2;

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                tb_SourceFile.Text = dialog.FileName;
            }

            btn_DestFile.IsEnabled = true;
        }

        private void Btn_DestFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FilterIndex = 2;

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                tb_DestFile.Text = dialog.FileName;
            }

            btn_Start.IsEnabled = true;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //check for input only number
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        private void Tb_SpeedReading_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //check for input only number
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        private void Tb_SpeedWriting_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //check for input only number
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }
    }
}
