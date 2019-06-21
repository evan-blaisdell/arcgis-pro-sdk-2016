using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.DragDrop;
using System.Windows;

namespace ProDragDrop
{
  internal class DropHandler1 : DropHandlerBase
  {
    public override void OnDragOver(DropInfo dropInfo)
    {
      //default is to accept our data types
      dropInfo.Effects = DragDropEffects.All;
    }
    public override void OnDrop(DropInfo dropInfo)
    {
      //eg, if you are accessing a dropped file
      //string filePath = dropInfo.Items[0].Data.ToString();
      string sPrompt = "On drop:" + System.Environment.NewLine;
      foreach (var item in dropInfo.Items)
      {
        sPrompt += item.Data as string + Environment.NewLine;
      }
      MessageBox.Show(sPrompt);
      //set to true if you handled the drop
      //dropInfo.Handled = true;
    }
  }
}
