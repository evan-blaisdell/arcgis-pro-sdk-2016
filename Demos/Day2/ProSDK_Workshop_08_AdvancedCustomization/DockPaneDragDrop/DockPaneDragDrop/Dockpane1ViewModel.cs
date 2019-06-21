using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.DragDrop;
using System.Collections.ObjectModel;

namespace DockPaneDragDrop
{
  internal class Dockpane1ViewModel : DockPane
  {
    private const string _dockPaneID = "DockPaneDragDrop_Dockpane1";

    protected Dockpane1ViewModel() { }

    /// <summary>
    /// Collection of dropped items
    /// </summary>
    public ObservableCollection<string> dropItems = new ObservableCollection<string>();
    public ObservableCollection<string> DropItems
    {
      get { return dropItems; }
      set
      {
        SetProperty(ref dropItems, value, () => DropItems);
      }
    }

    /// <summary>
    /// Show the DockPane.
    /// </summary>
    internal static void Show()
    {
      DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
      if (pane == null)
        return;

      pane.Activate();
    }
    
    /// <summary>
    /// Text shown near the top of the DockPane.
    /// </summary>
    private string _heading = "Drag & Drop DockPane";
    public string Heading
    {
      get { return _heading; }
      set
      {
        SetProperty(ref _heading, value, () => Heading);
      }
    }

    /// <summary>
    /// Text shown near the top of the DockPane.
    /// </summary>
    private string _dropHandled = "N/A";
    public string DropHandled
    {
      get { return _dropHandled; }
      set
      {
        SetProperty(ref _dropHandled, value, () => DropHandled);
      }
    }

    /// <summary>
    /// called by PRO UIFramework when item is dragged over pane
    /// </summary>
    /// <param name="dropInfo"></param>
    public override void OnDragOver(DropInfo dropInfo)
    {
        // Choose drop target adorner to Highlight or Insert
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

        // Choose drag drop effects to Copy, Move, Link or Scroll
        dropInfo.Effects = System.Windows.DragDropEffects.All;
    }

    /// <summary>
    /// called by PRO UIFramework when item is dropped onto pane
    /// </summary>
    /// <param name="dropInfo"></param>
    public override void OnDrop(DropInfo dropInfo)
    {
      DropItems.Clear();
      foreach (var item in dropInfo.Items)
      {
        if (System.IO.Path.GetExtension(item.Data as string) != ".xls")
          DropItems.Add(item.Data as string);
        item.Handled = true;
      }
      DropHandled = $@"Dropped {dropInfo.Items.Count} onto pane";
    }
    
  }

  /// <summary>
  /// Button implementation to show the DockPane.
  /// </summary>
	internal class Dockpane1_ShowButton : Button
  {
    protected override void OnClick()
    {
      Dockpane1ViewModel.Show();
    }
  }
}
