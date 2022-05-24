﻿namespace PgBrewer;

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

public class Alcohol : INotifyPropertyChanged
{
    #region Init
    public static Alcohol None { get; } = new Alcohol(string.Empty);

    public Alcohol(string name)
    {
        Name = name;
        EffectList = DataArchive.ReadEffectList(name);
        Previous = None;
        Next = None;
    }
    #endregion

    #region Properties
    public string Name { get; }
    public List<Effect> EffectList { get; } = new List<Effect>();
    public AlcoholLineCollection Lines { get; } = new AlcoholLineCollection();
    public Alcohol Previous { get; private set; }
    public Alcohol Next { get; private set; }
    public List<ComponentAssociationCollection> PreviousToNext { get; private set; } = new List<ComponentAssociationCollection>();

    public int MismatchCount
    {
        get => MismatchCountInternal;
        set
        {
            if (MismatchCountInternal != value)
            {
                MismatchCountInternal = value;
                NotifyThisPropertyChanged();
            }
        }
    }

    private int MismatchCountInternal;

    public bool IsSelected { get; private set; }
    #endregion

    #region Client Interface
    public void SetSelected(bool isSelected)
    {
        IsSelected = isSelected;
        NotifyPropertyChanged(nameof(IsSelected));
    }

    public virtual void Save()
    {
        List<int> Indexes = new List<int>();

        for (int i = 0; i < Lines.Count; i++)
            Indexes.Add(Lines[i].EffectIndex);

        DataArchive.SetIndexList(Name, Indexes);
    }

    public virtual void ClearCalculateIndexes()
    {
        foreach (AlcoholLine Line in Lines)
            Line.ClearCalculateIndex();
    }

    public virtual void RecalculateMismatchCount()
    {
        int NewCount = 0;

        foreach (AlcoholLine Line in Lines)
            if (!Line.IsMatching)
                NewCount++;

        MismatchCount = NewCount;
    }

    public virtual void Export(StreamWriter writer)
    {
        writer.WriteLine(Name);

        foreach (AlcoholLine Line in Lines)
        {
            string ExportedComponents = Line.GetExportedComponents();
            int EffectIndex = Line.EffectIndex;

            if (EffectIndex >= 0)
                writer.WriteLine($"{ExportedComponents};{EffectList[EffectIndex].Text}");
            else
                writer.WriteLine($"{ExportedComponents};");
        }

        writer.WriteLine();
    }

    public virtual bool Import(StreamReader reader, ref int changeCount)
    {
        if (Name != reader.ReadLine())
            return false;

        for (int i = 0; i < Lines.Count; i++)
        {
            AlcoholLine Line = Lines[i];
            string LineString = reader.ReadLine()!;

            string ExportedComponents = Line.GetExportedComponents();
            ExportedComponents += ";";

            if (!LineString.StartsWith(ExportedComponents))
                return false;

            LineString = LineString.Substring(ExportedComponents.Length);
            if (LineString.Length > 0)
            {
                int SelectedLineIndex = -1;
                for (int j = 0; j < EffectList.Count; j++)
                {
                    if (LineString == EffectList[j].Text)
                    {
                        SelectedLineIndex = j;
                        break;
                    }
                }

                if (SelectedLineIndex < 0)
                    return false;

                if (Line.EffectIndex != SelectedLineIndex)
                {
                    Line.EffectIndex = SelectedLineIndex;
                    changeCount++;
                }
            }
            else if (Line.EffectIndex >= 0)
            {
                Line.EffectIndex = -1;
                changeCount++;
            }
        }

        reader.ReadLine();

        return true;
    }

    public static void Chain(Alcohol previous, Alcohol next, List<ComponentAssociationCollection> previousToNext)
    {
        previous.Next = next;
        next.Previous = previous;

        previous.PreviousToNext = previousToNext;
    }
    #endregion

    #region Implementation of INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region Debugging
    public override string ToString()
    {
        return Name;
    }
    #endregion
}
