using ArcGIS.Desktop.Framework;
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

namespace MapAndLayers
{
  /// <summary>
  /// Interaction logic for CreateMapDialog.xaml
  /// </summary>
  public partial class CreateMapDialog : Window
  {
    public CreateMapDialog()
    {
      InitializeComponent();
      this.Owner = FrameworkApplication.Current.MainWindow;
    }

    private void btnDialogOk_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      mapName.SelectAll();
      mapName.Focus();
    }

    public string MapName
    {
      get { return mapName.Text; }
    }
  }
}
