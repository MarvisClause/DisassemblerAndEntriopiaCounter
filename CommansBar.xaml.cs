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
using System.Windows.Shapes;

namespace DisEn
{
    /// <summary>
    /// Interaction logic for CommansBar.xaml
    /// </summary>
    public partial class CommansBar : Window
    {
        public CommansBar()
        {
            InitializeComponent();
        } 
         //проверка на отмеченные команды и запись их в txt файл
        public void CommandsCheck(object sender, ExecutedRoutedEventArgs e)
        {
            if(incCheckBox.IsChecked== true) { }
        }
    }
}
