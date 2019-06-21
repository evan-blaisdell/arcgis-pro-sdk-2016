using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.DragDrop;
using System.Collections.ObjectModel;

namespace DockPaneDragDrop
{
  internal class Pane1ViewModel : ViewStatePane
  {
    private const string _viewPaneID = "DockPaneDragDrop_Pane1";

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
    /// Consume the passed in CIMView. Call the base constructor to wire up the CIMView.
    /// </summary>
    public Pane1ViewModel(CIMView view)
      : base(view)
    { }

    /// <summary>
    /// Create a new instance of the pane.
    /// </summary>
    internal static Pane1ViewModel Create()
    {
      var view = new CIMGenericView();
      view.ViewType = _viewPaneID;
      return FrameworkApplication.Panes.Create(_viewPaneID, new object[] { view }) as Pane1ViewModel;
    }

    /// <summary>
    /// called by PRO UIFramework when item is dragged over pane
    /// </summary>
    /// <param name="dropInfo"></param>
    public override void OnDragOver(DropInfo dropInfo)
    {
      System.Diagnostics.Debug.WriteLine(dropInfo.TargetModel?.GetType().ToString());
      System.Diagnostics.Debug.WriteLine(dropInfo.TargetItem?.GetType().ToString());
      {
        // Choose drop target adorner to Highlight or Insert
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

        // Choose drag drop effects to Copy, Move, Link or Scroll
        dropInfo.Effects = System.Windows.DragDropEffects.All;
      }
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

    #region Pane Overrides

    /// <summary>
    /// Must be overridden in child classes used to persist the state of the view to the CIM.
    /// </summary>
    public override CIMView ViewState
    {
      get
      {
        _cimView.InstanceID = (int)InstanceID;
        return _cimView;
      }
    }

    /// <summary>
    /// Called when the pane is initialized.
    /// </summary>
    protected async override Task InitializeAsync()
    {
      await base.InitializeAsync();
    }

    /// <summary>
    /// Called when the pane is uninitialized.
    /// </summary>
    protected async override Task UninitializeAsync()
    {
      await base.UninitializeAsync();
    }

    #endregion Pane Overrides
  }

  /// <summary>
  /// Button implementation to create a new instance of the pane and activate it.
  /// </summary>
  internal class Pane1_OpenButton : Button
  {
    protected override void OnClick()
    {
      Pane1ViewModel.Create();
    }
  }
}
