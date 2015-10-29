using System;
using System.Collections.Generic;
using System.Linq;
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

namespace NaiveBayesAlgo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var processor = new NaiveBayes.Processor<Person, int>();
                processor.Prepare();
                var data = new Processor().HandleData(@"pima-indians-diabetes.data.txt");
                processor.Process(data);

                //var processor = new Processor();
                //processor.Process(data);
            }
            catch (Exception exception)
            {
                lbl.Content = string.Format("Error occured: {0}\nStack trace: {1}", exception.Message, exception.StackTrace);
            }
        }
    }
}
