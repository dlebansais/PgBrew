﻿namespace PgBrewer;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

/// <summary>
/// Main window implementation.
/// </summary>
public partial class MainWindow
{
    public static BackForward BackForward { get; } = new();
    public static PgBrewerPageBeers PageBeers { get; } = new PgBrewerPageBeers(BackForward);
    public static PgBrewerPageLiquors PageLiquors { get; } = new PgBrewerPageLiquors(BackForward);
    public static PgBrewerPageSettings PageSettings { get; } = new PgBrewerPageSettings(BackForward);

    public ImageSource? IconBeer { get; private set; }
    public ImageSource? IconLiquor { get; private set; }
    public ImageSource? IconSettings { get; private set; }
    public override ObservableCollection<PgBrewerPage> PageList { get; } = new()
    {
        PageBeers,
        PageLiquors,
        PageSettings,
    };

    public override int SelectedPageIndex
    {
        get
        {
            return SelectedPageIndexInternal;
        }
        set
        {
            if (SelectedPageIndexInternal != value)
            {
                SelectedPageIndexInternal = value;

                for (int i = 0; i < PageList.Count; i++)
                    PageList[i].SetSelected(i == SelectedPageIndexInternal);
            }
        }
    }

    private int SelectedPageIndexInternal = 0;

    public bool IsChanged
    {
        get => IsChangedInternal;
        private set
        {
            if (IsChangedInternal != value)
            {
                IsChangedInternal = value;
                NotifyThisPropertyChanged();
            }
        }
    }

    private bool IsChangedInternal;

    public List<ComponentAssociationCollection> AssociationTable { get; } = new List<ComponentAssociationCollection>();
}
