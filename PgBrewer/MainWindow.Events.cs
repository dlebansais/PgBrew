﻿namespace PgBrewer;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using WpfLayout;

/// <summary>
/// Main window implementation.
/// </summary>
public partial class MainWindow
{
    public override void OnClosing(object sender, CancelEventArgs e)
    {
        if (IsChanged)
        {
            MessageBoxResult Answer = MessageBox.Show("Save changes before exit?", "Closing", MessageBoxButton.YesNoCancel);

            switch (Answer)
            {
                case MessageBoxResult.Yes:
                    SaveAll();
                    break;

                case MessageBoxResult.No:
                    break;

                default:
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }
    }

    public override void OnSave(object sender, ExecutedRoutedEventArgs e)
    {
        SaveAll();
        SetIsChanged(false);
    }

    private void SaveAll()
    {
        PageBeers.BasicLager.Save();
        PageBeers.PaleAle.Save();
        PageBeers.Marzen.Save();
        PageBeers.GoblinAle.Save();
        PageBeers.OrcishBock.Save();
        PageBeers.BrownAle.Save();
        PageBeers.HegemonyLager.Save();
        PageBeers.DwarvenStout.Save();
        PageLiquors.PotatoVodka.Save();
        PageLiquors.Applejack.Save();
        PageLiquors.BeetVodka.Save();
        PageLiquors.PaleRum.Save();
        PageLiquors.Whisky.Save();
        PageLiquors.Tequila.Save();
        PageLiquors.DryGin.Save();
        PageLiquors.Bourbon.Save();

        SaveAssociations();
    }

    private void SaveAssociations()
    {
        foreach (ComponentAssociationCollection AssociationList in AssociationTable)
        {
            List<int> IndexList = new List<int>();
            for (int i = 0; i < AssociationList.Count; i++)
                IndexList.Add(AssociationList[i].AssociationIndex);

            DataArchive.SetIndexList($"{AssociationSettingName}{AssociationList.Name}", IndexList);
        }
    }

    public override void OnDeleteLine(object sender, ExecutedRoutedEventArgs e)
    {
        AlcoholLine Line = (AlcoholLine)((Button)e.OriginalSource).DataContext;
        Line.EffectIndex = -1;
    }

    public override void OnDelete(object sender, ExecutedRoutedEventArgs e)
    {
        ComponentAssociation Association = (ComponentAssociation)((Button)e.OriginalSource).DataContext;
        Association.AssociationIndex = -1;
    }

    public override void OnExport(object sender, ExecutedRoutedEventArgs e)
    {
        FileDialogResult Result = (FileDialogResult)e.Parameter;

        if (Result.FilePath.Length == 0)
            Result.FilePath = "Brewing.csv";

        OnExport(Result.FilePath);
    }

    private void OnExport(string fileName)
    {
        try
        {
            using (FileStream Stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter Writer = new StreamWriter(Stream, Encoding.UTF8))
                {
                    OnExport(Writer);
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }

    private void OnExport(StreamWriter writer)
    {
        ExportVersionNumber(writer);

        ExportAssociations(writer);

        PageBeers.BasicLager.Export(writer);
        PageBeers.PaleAle.Export(writer);
        PageBeers.Marzen.Export(writer);
        PageBeers.GoblinAle.Export(writer);
        PageBeers.OrcishBock.Export(writer);
        PageBeers.BrownAle.Export(writer);
        PageBeers.HegemonyLager.Export(writer);
        PageBeers.DwarvenStout.Export(writer);
        PageLiquors.PotatoVodka.Export(writer);
        PageLiquors.Applejack.Export(writer);
        PageLiquors.BeetVodka.Export(writer);
        PageLiquors.PaleRum.Export(writer);
        PageLiquors.Whisky.Export(writer);
        PageLiquors.Tequila.Export(writer);
        PageLiquors.DryGin.Export(writer);
        PageLiquors.Bourbon.Export(writer);
    }

    private void ExportVersionNumber(StreamWriter writer)
    {
        if (SystemTools.GetVersion() is string Version)
            writer.WriteLine($"{VersionProlog}{Version}");
    }

    private void ExportAssociations(StreamWriter writer)
    {
        writer.WriteLine("Associations");
        writer.WriteLine();

        foreach (ComponentAssociationCollection AssociationList in AssociationTable)
        {
            writer.WriteLine(AssociationList.Name);

            foreach (ComponentAssociation Association in AssociationList)
            {
                string AssociationName = Association.Component.Name;

                if (Association.AssociationIndex >= 0)
                    writer.WriteLine($"{AssociationName};{Association.ChoiceList[Association.AssociationIndex]}");
                else
                    writer.WriteLine($"{AssociationName};");
            }

            writer.WriteLine();
        }

        writer.WriteLine();
    }

    public override void OnImport(object sender, ExecutedRoutedEventArgs e)
    {
        FileDialogResult Result = (FileDialogResult)e.Parameter;
        OnImport(Result.FilePath);
    }

    private void OnImport(string fileName)
    {
        try
        {
            using (FileStream Stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader Reader = new StreamReader(Stream, Encoding.UTF8))
                {
                    int ChangeCount = 0;
                    if (OnImport(Reader, ref ChangeCount))
                        if (ChangeCount == 0)
                            MessageBox.Show("The imported file contains the same data as the software.\r\n\r\nNo change made.", "Import", MessageBoxButton.OK, MessageBoxImage.Warning);
                        else
                            MessageBox.Show("File content imported.", "Import", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }

    private bool OnImport(StreamReader reader, ref int changeCount)
    {
        if (!ImportVersionNumber(reader))
            return false;

        if (OnImportConfirmed(reader, ref changeCount))
            return true;
        else
        {
            MessageBox.Show("Invalid format, not all of the file content was imported.", "Import", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private bool ImportVersionNumber(StreamReader reader)
    {
        if (SystemTools.GetVersion() is not string Version)
            return false;

        if (reader.ReadLine() is not string Line)
            return false;

        if (!Line.StartsWith(VersionProlog))
            return false;

        Line = Line.Substring(VersionProlog.Length);

        if (Line != Version)
        {
            MessageBoxResult Answer = MessageBox.Show($"This file was exported from version {Line}. The current version is {Version} and is probably not compatible. Continue?", "Import", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Answer != MessageBoxResult.Yes)
                return false;
        }

        return true;
    }

    private bool OnImportConfirmed(StreamReader reader, ref int changeCount)
    {
        if (!ImportAssociations(reader, ref changeCount))
            return false;

        if (!PageBeers.BasicLager.Import(reader, ref changeCount))
            return false;

        if (!PageBeers.PaleAle.Import(reader, ref changeCount))
            return false;

        if (!PageBeers.Marzen.Import(reader, ref changeCount))
            return false;

        if (!PageBeers.GoblinAle.Import(reader, ref changeCount))
            return false;

        if (!PageBeers.OrcishBock.Import(reader, ref changeCount))
            return false;

        if (!PageBeers.BrownAle.Import(reader, ref changeCount))
            return false;

        if (!PageBeers.HegemonyLager.Import(reader, ref changeCount))
            return false;

        if (!PageBeers.DwarvenStout.Import(reader, ref changeCount))
            return false;

        if (!PageLiquors.PotatoVodka.Import(reader, ref changeCount))
            return false;

        if (!PageLiquors.Applejack.Import(reader, ref changeCount))
            return false;

        if (!PageLiquors.BeetVodka.Import(reader, ref changeCount))
            return false;

        if (!PageLiquors.PaleRum.Import(reader, ref changeCount))
            return false;

        if (!PageLiquors.Whisky.Import(reader, ref changeCount))
            return false;

        if (!PageLiquors.Tequila.Import(reader, ref changeCount))
            return false;

        if (!PageLiquors.DryGin.Import(reader, ref changeCount))
            return false;

        if (!PageLiquors.Bourbon.Import(reader, ref changeCount))
            return false;

        return true;
    }

    private bool ImportAssociations(StreamReader reader, ref int changeCount)
    {
        if (reader.ReadLine() != "Associations")
            return false;

        reader.ReadLine();

        foreach (ComponentAssociationCollection AssociationList in AssociationTable)
        {
            if (AssociationList.Name != reader.ReadLine())
                return false;

            foreach (ComponentAssociation Association in AssociationList)
            {
                string AssociationString = reader.ReadLine()!;
                string AssociationName = Association.Component.Name;
                AssociationName += ";";

                if (!AssociationString.StartsWith(AssociationName))
                    return false;

                AssociationString = AssociationString.Substring(AssociationName.Length);
                if (AssociationString.Length > 0)
                {
                    int SelectedIndex = -1;
                    for (int i = 0; i < Association.ChoiceList.Count; i++)
                    {
                        Component Choice = Association.ChoiceList[i];
                        if (Choice.ToString() == AssociationString)
                        {
                            SelectedIndex = i;
                            break;
                        }
                    }

                    if (SelectedIndex < 0)
                        return false;

                    if (Association.AssociationIndex != SelectedIndex)
                    {
                        Association.AssociationIndex = SelectedIndex;
                        changeCount++;
                    }
                }
                else if (Association.AssociationIndex >= 0)
                {
                    Association.AssociationIndex = -1;
                    changeCount++;
                }
            }

            reader.ReadLine();
        }

        reader.ReadLine();

        return true;
    }

    public override void OnBack(object sender, ExecutedRoutedEventArgs e)
    {
        ChangeLine(-1);
    }

    public override void OnForward(object sender, ExecutedRoutedEventArgs e)
    {
        ChangeLine(+1);
    }

    private void ChangeLine(int offset)
    {
        PgBrewerPage SelectedPage = PageList[SelectedPageIndex];
        if (SelectedPage is IAlcoholPage AsAlcoholPage)
        {
            int SelectedAlcoholIndex = AsAlcoholPage.SelectedAlcoholIndex;
            IList<Alcohol> AlcoholList = AsAlcoholPage.AlcoholList;

            if (SelectedAlcoholIndex >= 0 && SelectedAlcoholIndex < AlcoholList.Count)
            {
                Alcohol SelectedAlcohol = AlcoholList[SelectedAlcoholIndex];
                int SelectedLine = SelectedAlcohol.SelectedLine;
                IList<AlcoholLine> LineList = SelectedAlcohol.Lines;

                if (SelectedLine >= 0 && SelectedLine < LineList.Count)
                {
                    AlcoholLine Line = LineList[SelectedLine];
                    Alcohol Owner = SelectedAlcohol;

                    int NewLine = -1;
                    if (offset < 0)
                    {
                        IFourComponentsAlcohol Next = (IFourComponentsAlcohol)Owner;
                        IFourComponentsAlcohol Previous = (IFourComponentsAlcohol)Owner.Previous;
                        List<ComponentAssociationCollection> PreviousToNext = ((Alcohol)Previous).PreviousToNext;

                        int NextLineIndex = Owner.Lines.IndexOf(Line);
                        GetPreviousLineIndex(Next, Previous, PreviousToNext[0], PreviousToNext[1], PreviousToNext[2], PreviousToNext[3], NextLineIndex, out NewLine);
                    }
                    else
                    {
                        IFourComponentsAlcohol Previous = (IFourComponentsAlcohol)Owner;
                        IFourComponentsAlcohol Next = (IFourComponentsAlcohol)Owner.Next;
                        List<ComponentAssociationCollection> PreviousToNext = ((Alcohol)Previous).PreviousToNext;

                        int PreviousLineIndex = Owner.Lines.IndexOf(Line);
                        GetNextLineIndex(Previous, Next, PreviousToNext[0], PreviousToNext[1], PreviousToNext[2], PreviousToNext[3], PreviousLineIndex, out NewLine);
                    }

                    if (NewLine >= 0)
                    {
                        AsAlcoholPage.SelectedAlcoholIndex = SelectedAlcoholIndex + offset;
                        int TotalLines = Owner.Lines.Count;

                        TaskDispatcher.Dispatch(() => OnLineMove(NewLine, TotalLines));
                    }
                }
            }
        }
    }

    private void OnLineMove(int newLineIndex, int totalLines)
    {
        Debug.Assert(SelectedPageIndex >= 0 && SelectedPageIndex < PageList.Count);

        PgBrewerPage SelectedPage = PageList[SelectedPageIndex];
        if (SelectedPage is IAlcoholPage AsAlcoholPage)
        {
            int SelectedAlcoholIndex = AsAlcoholPage.SelectedAlcoholIndex;
            IList<Alcohol> AlcoholList = AsAlcoholPage.AlcoholList;

            if (SelectedAlcoholIndex >= 0 && SelectedAlcoholIndex < AlcoholList.Count)
            {
                Alcohol SelectedAlcohol = AlcoholList[SelectedAlcoholIndex];
                IList<AlcoholLine> LineList = SelectedAlcohol.Lines;

                if (newLineIndex >= 0 && newLineIndex < LineList.Count)
                    SelectedAlcohol.SelectedLine = newLineIndex;
            }
        }
    }
}
