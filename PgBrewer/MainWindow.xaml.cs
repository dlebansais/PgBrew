﻿namespace PgBrewer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using Contracts;
    using Microsoft.Win32;

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Constants
        private static readonly string AssociationSettingName = "Association";
        private static readonly string GuiSettingName = "GUI";
        private static readonly string VersionProlog = "Exported from PgBrewing.exe version ";
        #endregion

        #region Init
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadAssociations();
            LoadGUI();
            LoadIcons();
            IsChangedInternal = false;
            CanGoBackInternal = false;
            CanGoForwardInternal = false;

            Loaded += OnLoaded;
        }

        private void LoadAssociations()
        {
            LoadAssociations(AssociationFruit1);
            LoadAssociations(AssociationFruit2);
            LoadAssociations(AssociationVeggie1);
            LoadAssociations(AssociationVeggie2Beer);
            LoadAssociations(AssociationVeggie2Liquor);
            LoadAssociations(AssociationMushroom3);
            LoadAssociations(AssociationParts1);
            LoadAssociations(AssociationParts2);
            LoadAssociations(AssociationFlavor1Beer);
            LoadAssociations(AssociationFlavor1Liquor);
            LoadAssociations(AssociationFlavor2Beer);
            LoadAssociations(AssociationFlavor2Liquor);
        }

        private void LoadAssociations(ComponentAssociationCollection associationList)
        {
            List<int> AssociationIndexes = DataArchive.GetIndexList($"{AssociationSettingName}{associationList.Name}", associationList.Count);
            for (int i = 0; i < associationList.Count; i++)
                associationList[i].AssociationIndex = AssociationIndexes[i];

            AssociationTable.Add(associationList);
        }

        private void LoadGUI()
        {
            List<int> Coordinates = DataArchive.GetIndexList(GuiSettingName, 4);

            if (Coordinates[0] >= 0)
                Left = Coordinates[0];

            if (Coordinates[1] >= 0)
                Top = Coordinates[1];

            if (Coordinates[2] >= 0)
                Width = Coordinates[2];

            if (Coordinates[3] >= 0)
                Height = Coordinates[3];

            if (Coordinates[2] >= 0 && Coordinates[3] >= 0)
                SizeToContent = SizeToContent.Manual;
        }

        private void LoadIcons()
        {
            try
            {
                string UserRootFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string ApplicationFolder = Path.Combine(UserRootFolder, "PgJsonParse");
                string VersionCacheFolder = Path.Combine(ApplicationFolder, "Versions");

                if (!Directory.Exists(VersionCacheFolder))
                    return;

                string? FinalFolder = null;
                string SharedFolder = Path.Combine(ApplicationFolder, "Shared Icons");

                if (Directory.Exists(SharedFolder))
                    FinalFolder = SharedFolder;
                else
                {
                    string[] VersionFolders = Directory.GetDirectories(VersionCacheFolder);
                    int LastVersion = -1;

                    foreach (string Folder in VersionFolders)
                    {
                        if (int.TryParse(Path.GetFileName(Folder), out int FolderVersion))
                            if (LastVersion < FolderVersion)
                                LastVersion = FolderVersion;
                    }

                    if (LastVersion > 0)
                        FinalFolder = Path.Combine(VersionCacheFolder, LastVersion.ToString());
                }

                if (FinalFolder != null)
                {
                    IconBeer = new BitmapImage(new Uri(Path.Combine(FinalFolder, "icon_5744.png")));
                    IconLiquor = new BitmapImage(new Uri(Path.Combine(FinalFolder, "icon_5746.png")));
                    IconSettings = new BitmapImage(new Uri(Path.Combine(FinalFolder, "icon_5476.png")));
                }
            }
            catch
            {
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Alcohol.Chain(OrcishBock, BrownAle, new List<ComponentAssociationCollection>() { AssociationVeggie2Beer, ComponentAssociationCollection.None, ComponentAssociationCollection.None, AssociationFlavor1Beer });
            Alcohol.Chain(BrownAle, HegemonyLager, new List<ComponentAssociationCollection>() { ComponentAssociationCollection.None, AssociationFruit2, ComponentAssociationCollection.None, ComponentAssociationCollection.None });
            Alcohol.Chain(HegemonyLager, DwarvenStout, new List<ComponentAssociationCollection>() { ComponentAssociationCollection.None, ComponentAssociationCollection.None, AssociationMushroom3, AssociationFlavor2Beer });

            Alcohol.Chain(PotatoVodka, Applejack, new List<ComponentAssociationCollection>() { ComponentAssociationCollection.None, AssociationVeggie1, ComponentAssociationCollection.None, ComponentAssociationCollection.None });
            Alcohol.Chain(Applejack, BeetVodka, new List<ComponentAssociationCollection>() { AssociationFruit1, ComponentAssociationCollection.None, ComponentAssociationCollection.None, ComponentAssociationCollection.None });
            Alcohol.Chain(BeetVodka, PaleRum, new List<ComponentAssociationCollection>() { ComponentAssociationCollection.None, ComponentAssociationCollection.None, ComponentAssociationCollection.None, ComponentAssociationCollection.None });
            Alcohol.Chain(PaleRum, Whisky, new List<ComponentAssociationCollection>() { ComponentAssociationCollection.None, ComponentAssociationCollection.None, AssociationParts1, ComponentAssociationCollection.None });
            Alcohol.Chain(Whisky, Tequila, new List<ComponentAssociationCollection>() { ComponentAssociationCollection.None, ComponentAssociationCollection.None, ComponentAssociationCollection.None, AssociationFlavor1Liquor });
            Alcohol.Chain(Tequila, DryGin, new List<ComponentAssociationCollection>() { AssociationFruit2, AssociationMushroom3, ComponentAssociationCollection.None, ComponentAssociationCollection.None });
            Alcohol.Chain(DryGin, Bourbon, new List<ComponentAssociationCollection>() { ComponentAssociationCollection.None, ComponentAssociationCollection.None, AssociationParts2, AssociationFlavor2Liquor });

            Recalculate();
        }
        #endregion

        #region Properties
        public ImageSource? IconBeer { get; private set; }
        public ImageSource? IconLiquor { get; private set; }
        public ImageSource? IconSettings { get; private set; }

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

        public bool CanGoBack
        {
            get => CanGoBackInternal;
            private set
            {
                if (CanGoBackInternal != value)
                {
                    CanGoBackInternal = value;
                    NotifyThisPropertyChanged();
                }
            }
        }

        private bool CanGoBackInternal;

        public bool CanGoForward
        {
            get => CanGoForwardInternal;
            private set
            {
                if (CanGoForwardInternal != value)
                {
                    CanGoForwardInternal = value;
                    NotifyThisPropertyChanged();
                }
            }
        }

        private bool CanGoForwardInternal;
        #endregion

        #region Alcohols
        public static Component RedApple { get; } = new Component("Red Apple");
        public static Component Grapes { get; } = new Component("Grapes");
        public static Component Orange { get; } = new Component("Orange");
        public static Component Guava { get; } = new Component("Guava");
        public static Component Banana { get; } = new Component("Banana");
        public static Component Lemon { get; } = new Component("Lemon");
        public static Component Pear { get; } = new Component("Pear");
        public static Component Peach { get; } = new Component("Peach");
        public static Component GreenApple { get; } = new Component("Green Apple");
        public static Component ParasolMushroomFlakes { get; } = new Component("Parasol Mushroom Flakes");
        public static Component MycenaMushroomFlakes { get; } = new Component("Mycena Mushroom Flakes");
        public static Component BoletusMushroomFlakes { get; } = new Component("Boletus Mushroom Flakes");
        public static Component FieldMushroomFlakes { get; } = new Component("Field Mushroom Flakes");
        public static Component Beet { get; } = new Component("Beet");
        public static Component Squash { get; } = new Component("Squash");
        public static Component Broccoli { get; } = new Component("Broccoli");
        public static Component Carrot { get; } = new Component("Carrot");
        public static Component BlusherMushroomFlakes { get; } = new Component("Blusher Mushroom Flakes");
        public static Component MilkCapMushroomPowder { get; } = new Component("Milk Cap Mushroom Powder");
        public static Component BloodMushroomPowder { get; } = new Component("Blood Mushroom Powder");
        public static Component CoralMushroomPowder { get; } = new Component("Coral Mushroom Powder");
        public static Component GroxmaxPowder { get; } = new Component("Groxmax Powder");
        public static Component PorciniMushroomFlakes { get; } = new Component("Porcini Mushroom Flakes");
        public static Component BlackFootMorelFlakes { get; } = new Component("Black Foot Morel Flakes");
        public static Component BoarTusk { get; } = new Component("Boar Tusk");
        public static Component CatEyeball { get; } = new Component("Cat Eyeball");
        public static Component SnailSinew { get; } = new Component("Snail Sinew");
        public static Component RatTail { get; } = new Component("Rat Tail");
        public static Component BasicFishScale { get; } = new Component("Basic Fish Scale");
        public static Component WolfTeeth { get; } = new Component("Wolf Teeth");
        public static Component PantherTail { get; } = new Component("Panther Tail");
        public static Component DeinonychusClaw { get; } = new Component("Deinonychus Claw");
        public static Component RabbitsFoot { get; } = new Component("Rabbit's Foot");
        public static Component BearGallbladder { get; } = new Component("Bear Gallbladder");
        public static Component CockatriceBeak { get; } = new Component("Cockatrice Beak");
        public static Component WormTooth { get; } = new Component("Worm Tooth");
        public static Component Ectoplasm { get; } = new Component("Ectoplasm");
        public static Component PowderedMammal { get; } = new Component("Powdered Mammal");
        public static Component BarghestFlesh { get; } = new Component("Barghest Flesh");
        public static Component Oregano { get; } = new Component("Oregano");
        public static Component MandrakeRoot { get; } = new Component("Mandrake Root");
        public static Component Peppercorns { get; } = new Component("Peppercorns");
        public static Component Grass { get; } = new Component("Grass");
        public static Component Cinnamon { get; } = new Component("Cinnamon");
        public static Component MuntokPeppercorns { get; } = new Component("Muntok Peppercorns");
        public static Component Seaweed { get; } = new Component("Seaweed");
        public static Component MyconianJelly { get; } = new Component("Myconian Jelly");
        public static Component Mint { get; } = new Component("Mint");
        public static Component Honey { get; } = new Component("Honey");
        public static Component JuniperBerries { get; } = new Component("Juniper Berries");
        public static Component Almonds { get; } = new Component("Almonds");
        public static Component LargeStrawberry { get; } = new Component("Large Strawberry");
        public static Component GreenPepper { get; } = new Component("Green Pepper");
        public static Component RedPepper { get; } = new Component("Red Pepper");
        public static Component Molasses { get; } = new Component("Molasses");
        public static Component Corn { get; } = new Component("Corn");

        private static List<Component> FruitTier1Three = new List<Component>() { RedApple, Grapes, Orange };
        private static List<Component> FruitTier1Four = new List<Component>() { RedApple, Grapes, Orange, LargeStrawberry };
        private static List<Component> FruitTier2 = new List<Component>() { Guava, Banana, Lemon };
        private static List<Component> FruitTier3 = new List<Component>() { Pear, Peach, GreenApple };
        private static List<Component> VeggieTier1 = new List<Component>() { ParasolMushroomFlakes, MycenaMushroomFlakes, BoletusMushroomFlakes, FieldMushroomFlakes };
        private static List<Component> VeggieTier2 = new List<Component>() { Beet, Squash, Broccoli, Carrot };
        private static List<Component> VeggieTier3Beer = new List<Component>() { GreenPepper, RedPepper, Molasses, Corn };
        private static List<Component> MushroomTier3 = new List<Component>() { FieldMushroomFlakes, BlusherMushroomFlakes, MilkCapMushroomPowder, BloodMushroomPowder };
        private static List<Component> MushroomTier4 = new List<Component>() { CoralMushroomPowder, GroxmaxPowder, PorciniMushroomFlakes, BlackFootMorelFlakes };
        private static List<Component> PartsTier1 = new List<Component>() { BoarTusk, CatEyeball, SnailSinew, RatTail, BasicFishScale };
        private static List<Component> PartsTier2 = new List<Component>() { WolfTeeth, PantherTail, DeinonychusClaw, RabbitsFoot, BearGallbladder };
        private static List<Component> PartsTier3 = new List<Component>() { CockatriceBeak, WormTooth, Ectoplasm, PowderedMammal, BarghestFlesh };
        private static List<Component> FlavorTier1Two = new List<Component>() { Oregano, MandrakeRoot };
        private static List<Component> FlavorTier1Three = new List<Component>() { Oregano, MandrakeRoot, Peppercorns };
        private static List<Component> FlavorTier1Four = new List<Component>() { Oregano, MandrakeRoot, Peppercorns, Grass };
        private static List<Component> FlavorTier2Three = new List<Component>() { Cinnamon, MuntokPeppercorns, Seaweed };
        private static List<Component> FlavorTier2Four = new List<Component>() { Cinnamon, MuntokPeppercorns, Seaweed, MyconianJelly };
        private static List<Component> FlavorTier3Three = new List<Component>() { Mint, Honey, JuniperBerries };
        private static List<Component> FlavorTier3Four = new List<Component>() { Mint, Honey, JuniperBerries, Almonds };

        public Alcoholx4 BasicLager { get; private set; } = new Alcoholx4("Basic Lager",
            FruitTier1Four);

        public Alcoholx4x4 PaleAle { get; private set; } = new Alcoholx4x4("Pale Ale",
            FruitTier1Four,
            VeggieTier1);

        public Alcoholx4x4x2 Marzen { get; private set; } = new Alcoholx4x4x2("Marzen",
            FruitTier1Four,
            VeggieTier1,
            FlavorTier1Two);

        public Alcoholx3x3x4x3 GoblinAle { get; private set; } = new Alcoholx3x3x4x3("Goblin Ale",
            FruitTier1Three,
            FruitTier2,
            VeggieTier1,
            FlavorTier1Three);

        public Alcoholx4x3x4x3 OrcishBock { get; private set; } = new Alcoholx4x3x4x3("Orcish Bock",
            VeggieTier2,
            FruitTier2,
            MushroomTier3,
            FlavorTier1Three);

        public Alcoholx4x3x4x3 BrownAle { get; private set; } = new Alcoholx4x3x4x3("Brown Ale",
            VeggieTier3Beer,
            FruitTier2,
            MushroomTier3,
            FlavorTier2Three);

        public Alcoholx4x3x4x3 HegemonyLager { get; private set; } = new Alcoholx4x3x4x3("Hegemony Lager",
            VeggieTier3Beer,
            FruitTier3,
            MushroomTier3,
            FlavorTier2Three);

        public Alcoholx4x3x4x3 DwarvenStout { get; private set; } = new Alcoholx4x3x4x3("Dwarven Stout",
            VeggieTier3Beer,
            FruitTier3,
            MushroomTier4,
            FlavorTier3Three);

        public Alcoholx3x4x5x4 PotatoVodka { get; private set; } = new Alcoholx3x4x5x4("Potato Vodka",
            FruitTier1Three,
            VeggieTier1,
            PartsTier1,
            FlavorTier1Four);

        public Alcoholx3x4x5x4 Applejack { get; private set; } = new Alcoholx3x4x5x4("Applejack",
            FruitTier1Three,
            VeggieTier2,
            PartsTier1,
            FlavorTier1Four);

        public Alcoholx3x4x5x4 BeetVodka { get; private set; } = new Alcoholx3x4x5x4("Beet Vodka",
            FruitTier2,
            VeggieTier1,
            PartsTier1,
            FlavorTier1Four);

        public Alcoholx3x4x5x4 PaleRum { get; private set; } = new Alcoholx3x4x5x4("Pale Rum",
            FruitTier2,
            MushroomTier3,
            PartsTier1,
            FlavorTier1Four);

        public Alcoholx3x4x5x4 Whisky { get; private set; } = new Alcoholx3x4x5x4("Whisky",
            FruitTier2,
            MushroomTier3,
            PartsTier2,
            FlavorTier1Four);

        public Alcoholx3x4x5x4 Tequila { get; private set; } = new Alcoholx3x4x5x4("Tequila",
            FruitTier2,
            MushroomTier3,
            PartsTier2,
            FlavorTier2Four);

        public Alcoholx3x4x5x4 DryGin { get; private set; } = new Alcoholx3x4x5x4("Dry Gin",
            FruitTier3,
            MushroomTier4,
            PartsTier2,
            FlavorTier2Four);

        public Alcoholx3x4x5x4 Bourbon { get; private set; } = new Alcoholx3x4x5x4("Bourbon",
            FruitTier3,
            MushroomTier4,
            PartsTier3,
            FlavorTier3Four);

        public ComponentAssociationCollection AssociationFruit1 { get; } = new ComponentAssociationCollection("Fruit1", new List<ComponentAssociation>()
        {
            new ComponentAssociation(RedApple, FruitTier2),
            new ComponentAssociation(Grapes, FruitTier2),
            new ComponentAssociation(Orange, FruitTier2),
        });

        public ComponentAssociationCollection AssociationFruit2 { get; } = new ComponentAssociationCollection("Fruit2", new List<ComponentAssociation>()
        {
            new ComponentAssociation(Guava, FruitTier3),
            new ComponentAssociation(Banana, FruitTier3),
            new ComponentAssociation(Lemon, FruitTier3),
        });

        public ComponentAssociationCollection AssociationVeggie1 { get; } = new ComponentAssociationCollection("Veggie1", new List<ComponentAssociation>()
        {
            new ComponentAssociation(ParasolMushroomFlakes, VeggieTier2),
            new ComponentAssociation(MycenaMushroomFlakes, VeggieTier2),
            new ComponentAssociation(BoletusMushroomFlakes, VeggieTier2),
            new ComponentAssociation(FieldMushroomFlakes, VeggieTier2),
        });

        public ComponentAssociationCollection AssociationVeggie2Beer { get; } = new ComponentAssociationCollection("Veggie2Beer", new List<ComponentAssociation>()
        {
            new ComponentAssociation(Beet, VeggieTier3Beer),
            new ComponentAssociation(Squash, VeggieTier3Beer),
            new ComponentAssociation(Broccoli, VeggieTier3Beer),
            new ComponentAssociation(Carrot, VeggieTier3Beer),
        });

        public ComponentAssociationCollection AssociationVeggie2Liquor { get; } = new ComponentAssociationCollection("Veggie2Liquor", new List<ComponentAssociation>()
        {
            new ComponentAssociation(Beet, MushroomTier3),
            new ComponentAssociation(Squash, MushroomTier3),
            new ComponentAssociation(Broccoli, MushroomTier3),
            new ComponentAssociation(Carrot, MushroomTier3),
        });

        public ComponentAssociationCollection AssociationMushroom3 { get; } = new ComponentAssociationCollection("Mushroom3", new List<ComponentAssociation>()
        {
            new ComponentAssociation(FieldMushroomFlakes, MushroomTier4),
            new ComponentAssociation(BlusherMushroomFlakes, MushroomTier4),
            new ComponentAssociation(MilkCapMushroomPowder, MushroomTier4),
            new ComponentAssociation(BloodMushroomPowder, MushroomTier4),
        });

        public ComponentAssociationCollection AssociationParts1 { get; } = new ComponentAssociationCollection("Parts1", new List<ComponentAssociation>()
        {
            new ComponentAssociation(BoarTusk, PartsTier2),
            new ComponentAssociation(CatEyeball, PartsTier2),
            new ComponentAssociation(SnailSinew, PartsTier2),
            new ComponentAssociation(RatTail, PartsTier2),
            new ComponentAssociation(BasicFishScale, PartsTier2),
        });

        public ComponentAssociationCollection AssociationParts2 { get; } = new ComponentAssociationCollection("Parts2", new List<ComponentAssociation>()
        {
            new ComponentAssociation(WolfTeeth, PartsTier3),
            new ComponentAssociation(PantherTail, PartsTier3),
            new ComponentAssociation(DeinonychusClaw, PartsTier3),
            new ComponentAssociation(RabbitsFoot, PartsTier3),
            new ComponentAssociation(BearGallbladder, PartsTier3),
        });

        public ComponentAssociationCollection AssociationFlavor1Beer { get; } = new ComponentAssociationCollection("Flavor1Beer", new List<ComponentAssociation>()
        {
            new ComponentAssociation(Oregano, FlavorTier2Three),
            new ComponentAssociation(MandrakeRoot, FlavorTier2Three),
            new ComponentAssociation(Peppercorns, FlavorTier2Three),
        });

        public ComponentAssociationCollection AssociationFlavor1Liquor { get; } = new ComponentAssociationCollection("Flavor1Liquor", new List<ComponentAssociation>()
        {
            new ComponentAssociation(Oregano, FlavorTier2Four),
            new ComponentAssociation(MandrakeRoot, FlavorTier2Four),
            new ComponentAssociation(Peppercorns, FlavorTier2Four),
            new ComponentAssociation(Grass, FlavorTier2Four),
        });

        public ComponentAssociationCollection AssociationFlavor2Beer { get; } = new ComponentAssociationCollection("Flavor2Beer", new List<ComponentAssociation>()
        {
            new ComponentAssociation(Cinnamon, FlavorTier3Three),
            new ComponentAssociation(MuntokPeppercorns, FlavorTier3Three),
            new ComponentAssociation(Seaweed, FlavorTier3Three),
        });

        public ComponentAssociationCollection AssociationFlavor2Liquor { get; } = new ComponentAssociationCollection("Flavor2Liquor", new List<ComponentAssociation>()
        {
            new ComponentAssociation(Cinnamon, FlavorTier3Four),
            new ComponentAssociation(MuntokPeppercorns, FlavorTier3Four),
            new ComponentAssociation(Seaweed, FlavorTier3Four),
            new ComponentAssociation(MyconianJelly, FlavorTier3Four),
        });

        public List<ComponentAssociationCollection> AssociationTable { get; } = new List<ComponentAssociationCollection>();
        #endregion

        #region Events
        private void OnClosing(object sender, CancelEventArgs e)
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

            if (!e.Cancel)
                SaveGUI();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            SaveAll();
            IsChanged = false;
        }

        private void SaveAll()
        {
            BasicLager.Save();
            PaleAle.Save();
            Marzen.Save();
            GoblinAle.Save();
            OrcishBock.Save();
            BrownAle.Save();
            HegemonyLager.Save();
            DwarvenStout.Save();
            PotatoVodka.Save();
            Applejack.Save();
            BeetVodka.Save();
            PaleRum.Save();
            Whisky.Save();
            Tequila.Save();
            DryGin.Save();
            Bourbon.Save();

            SaveAssociations();
            SaveGUI();
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

        private void SaveGUI()
        {
            if (WindowState != WindowState.Normal)
                return;

            List<int> Coordinates = new List<int>();
            Coordinates.Add((int)Left);
            Coordinates.Add((int)Top);
            Coordinates.Add((int)ActualWidth);
            Coordinates.Add((int)ActualHeight);

            DataArchive.SetIndexList(GuiSettingName, Coordinates);
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            ComponentAssociation Association = (ComponentAssociation)((Button)sender).DataContext;
            Association.AssociationIndex = -1;
        }

        private void OnExport(object sender, RoutedEventArgs e)
        {
            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.Filter = "CSV file (*.csv)|*.csv";
            bool? Continue = Dlg.ShowDialog(this);

            if (Continue.HasValue && Continue.Value)
                OnExport(Dlg.FileName);
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

            BasicLager.Export(writer);
            PaleAle.Export(writer);
            Marzen.Export(writer);
            GoblinAle.Export(writer);
            OrcishBock.Export(writer);
            BrownAle.Export(writer);
            HegemonyLager.Export(writer);
            DwarvenStout.Export(writer);
            PotatoVodka.Export(writer);
            Applejack.Export(writer);
            BeetVodka.Export(writer);
            PaleRum.Export(writer);
            Whisky.Export(writer);
            Tequila.Export(writer);
            DryGin.Export(writer);
            Bourbon.Export(writer);
        }

        private void ExportVersionNumber(StreamWriter writer)
        {
            string Version = GetVersion();
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

        private void OnImport(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.Filter = "CSV file (*.csv)|*.csv";
            bool? Continue = Dlg.ShowDialog(this);

            if (Continue.HasValue && Continue.Value)
                OnImport(Dlg.FileName);
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
            string Version = GetVersion();

            string Line = reader.ReadLine()!;
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

            if (!BasicLager.Import(reader, ref changeCount))
                return false;

            if (!PaleAle.Import(reader, ref changeCount))
                return false;

            if (!Marzen.Import(reader, ref changeCount))
                return false;

            if (!GoblinAle.Import(reader, ref changeCount))
                return false;

            if (!OrcishBock.Import(reader, ref changeCount))
                return false;

            if (!BrownAle.Import(reader, ref changeCount))
                return false;

            if (!HegemonyLager.Import(reader, ref changeCount))
                return false;

            if (!DwarvenStout.Import(reader, ref changeCount))
                return false;

            if (!PotatoVodka.Import(reader, ref changeCount))
                return false;

            if (!Applejack.Import(reader, ref changeCount))
                return false;

            if (!BeetVodka.Import(reader, ref changeCount))
                return false;

            if (!PaleRum.Import(reader, ref changeCount))
                return false;

            if (!Whisky.Import(reader, ref changeCount))
                return false;

            if (!Tequila.Import(reader, ref changeCount))
                return false;

            if (!DryGin.Import(reader, ref changeCount))
                return false;

            if (!Bourbon.Import(reader, ref changeCount))
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

        private string GetVersion()
        {
            Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
#pragma warning disable IL3000 // Avoid using accessing Assembly file path when publishing as a single-file
            FileVersionInfo VersionInfo = FileVersionInfo.GetVersionInfo(CurrentAssembly.Location);
#pragma warning restore IL3000 // Avoid using accessing Assembly file path when publishing as a single-file

            return VersionInfo.FileVersion!;
        }

        public void OnGotFocus(ComboBox sender)
        {
            LastFocusedCombo = sender;

            AlcoholLine Line = (AlcoholLine)sender.DataContext;
            Alcohol Owner = Line.Owner;

            CanGoBack = Owner.Previous != null;
            CanGoForward = Owner.Next != null;
        }

        public void OnLostFocus(ComboBox sender)
        {
            // CanGoBack = false;
            // CanGoForward = false;
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            ChangeLine(-1);
        }

        private void OnForward(object sender, RoutedEventArgs e)
        {
            ChangeLine(+1);
        }

        private void ChangeLine(int offset)
        {
            if (LastFocusedCombo != null && FindTabControl(out TabControl CtrlPage))
            {
                Debug.Assert(CtrlPage.SelectedIndex + offset >= 0 && CtrlPage.SelectedIndex + offset < CtrlPage.Items.Count);

                AlcoholLine Line = (AlcoholLine)LastFocusedCombo.DataContext;
                Alcohol Owner = Line.Owner;

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
                    CtrlPage.SelectedIndex = CtrlPage.SelectedIndex + offset;
                    int TotalLines = Owner.Lines.Count;
                    Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new LineMoveHandler(OnLineMove), CtrlPage, NewLine, TotalLines);
                }
            }
        }

        private delegate void LineMoveHandler(TabControl ctrlPage, int newLineIndex, int totalLines);

        private void OnLineMove(TabControl ctrlPage, int newLineIndex, int totalLines)
        {
            FrameworkElement Root = (FrameworkElement)ctrlPage.SelectedContent;
            if (Tools.FindFirstControl(Root, out ScrollViewer FirstScrollViewer))
            {
                FirstScrollViewer.ScrollToVerticalOffset((double)newLineIndex / (double)totalLines);
                if (Tools.FindFirstControl(FirstScrollViewer, out ItemsControl FirstItemsControl))
                {
                    ItemContainerGenerator Generator = FirstItemsControl.ItemContainerGenerator;
                    FrameworkElement LineContent = (FrameworkElement)Generator.ContainerFromIndex(newLineIndex);
                    if (Tools.FindFirstControl(LineContent, out ComboBox FirstComboBox))
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new SetLineFocusHandler(OnSetLineFocus), FirstComboBox);
                    }
                }
            }
        }

        private delegate void SetLineFocusHandler(ComboBox firstComboBox);
        private void OnSetLineFocus(ComboBox firstComboBox)
        {
            firstComboBox.Focus();
        }

        private bool FindTabControl(out TabControl ctrlPage)
        {
            FrameworkElement? Ctrl = LastFocusedCombo;

            while (Ctrl != null)
            {
                if (Ctrl is TabControl AsTabControl)
                {
                    ctrlPage = AsTabControl;
                    return true;
                }

                Ctrl = VisualTreeHelper.GetParent(Ctrl) as FrameworkElement;
            }

            Contract.Unused(out ctrlPage);
            return false;
        }

        private ComboBox? LastFocusedCombo;
        #endregion

        #region Client Interface
        public void Recalculate()
        {
            RecalculateFromBottom(OrcishBock);
            RecalculateFromBottom(PotatoVodka);
        }

        public void RecalculateFromBottom(IFourComponentsAlcohol start)
        {
            IFourComponentsAlcohol Previous = start;
            IFourComponentsAlcohol Next;
            List<ComponentAssociationCollection> AssociationLists;

            ((Alcohol)Previous).ClearCalculateIndexes();

            for (; ;)
            {
                Alcohol NextAlcohol = ((Alcohol)Previous).Next;
                if (NextAlcohol == Alcohol.None)
                    break;

                Next = (IFourComponentsAlcohol)NextAlcohol;

                ((Alcohol)Next).ClearCalculateIndexes();

                AssociationLists = ((Alcohol)Previous).PreviousToNext;
                RecalculateBottomToTop(Previous, Next, AssociationLists[0], AssociationLists[1], AssociationLists[2], AssociationLists[3]);

                Previous = Next;
            }

            Next = Previous;

            for (; ;)
            {
                ((Alcohol)Next).RecalculateMismatchCount();

                Alcohol PreviousAlcohol = ((Alcohol)Next).Previous;
                if (PreviousAlcohol == Alcohol.None)
                    break;

                Previous = (IFourComponentsAlcohol)PreviousAlcohol;

                AssociationLists = ((Alcohol)Previous).PreviousToNext;
                RecalculateTopToBottom(Next, Previous, AssociationLists[0], AssociationLists[1], AssociationLists[2], AssociationLists[3]);

                Next = Previous;
            }
        }

        public void RecalculateBottomToTop(IFourComponentsAlcohol previous, IFourComponentsAlcohol next, ComponentAssociationCollection associationList1, ComponentAssociationCollection associationList2, ComponentAssociationCollection associationList3, ComponentAssociationCollection associationList4)
        {
            Debug.Assert((previous is Alcohol) && (next is Alcohol) && ((Alcohol)previous).Next == next && ((Alcohol)next).Previous == previous);

            int Multiplier1 = previous.Multiplier1;
            int Multiplier2 = previous.Multiplier2;
            int Multiplier3 = previous.Multiplier3;
            Debug.Assert(Multiplier1 == next.Multiplier1);
            Debug.Assert(Multiplier2 == next.Multiplier2);
            Debug.Assert(Multiplier3 == next.Multiplier3);

            for (int PreviousLineIndex = 0; PreviousLineIndex < previous.Lines.Count; PreviousLineIndex++)
                if (GetNextLineIndex(previous, next, associationList1, associationList2, associationList3, associationList4, PreviousLineIndex, out int NextLineIndex))
                    next.Lines[NextLineIndex].CalculatedIndex = previous.Lines[PreviousLineIndex].BestIndex;
        }

        public bool GetNextLineIndex(IFourComponentsAlcohol previous, IFourComponentsAlcohol next, ComponentAssociationCollection associationList1, ComponentAssociationCollection associationList2, ComponentAssociationCollection associationList3, ComponentAssociationCollection associationList4, int previousLineIndex, out int nextLineIndex)
        {
            IFourComponentsAlcoholLine Line = (IFourComponentsAlcoholLine)previous.Lines[previousLineIndex];
            int BestIndex = Line.BestIndex;
            if (BestIndex >= 0)
            {
                int NextIndex1 = GetPreviousToNextIndex(associationList1, Line.Index1);
                int NextIndex2 = GetPreviousToNextIndex(associationList2, Line.Index2);
                int NextIndex3 = GetPreviousToNextIndex(associationList3, Line.Index3);
                int NextIndex4 = GetPreviousToNextIndex(associationList4, Line.Index4);

                if (NextIndex1 >= 0 && NextIndex2 >= 0 && NextIndex3 >= 0 && NextIndex4 >= 0)
                {
                    int Multiplier1 = previous.Multiplier1;
                    int Multiplier2 = previous.Multiplier2;
                    int Multiplier3 = previous.Multiplier3;

                    nextLineIndex = (NextIndex1 * Multiplier1) + (NextIndex2 * Multiplier2) + (NextIndex3 * Multiplier3) + NextIndex4;
                    return true;
                }
            }

            nextLineIndex = -1;
            return false;
        }

        public void RecalculateTopToBottom(IFourComponentsAlcohol next, IFourComponentsAlcohol previous, ComponentAssociationCollection associationList1, ComponentAssociationCollection associationList2, ComponentAssociationCollection associationList3, ComponentAssociationCollection associationList4)
        {
            int Multiplier1 = next.Multiplier1;
            int Multiplier2 = next.Multiplier2;
            int Multiplier3 = next.Multiplier3;
            Debug.Assert(Multiplier1 == previous.Multiplier1);
            Debug.Assert(Multiplier2 == previous.Multiplier2);
            Debug.Assert(Multiplier3 == previous.Multiplier3);

            for (int NextLineIndex = 0; NextLineIndex < next.Lines.Count; NextLineIndex++)
                if (GetPreviousLineIndex(next, previous, associationList1, associationList2, associationList3, associationList4, NextLineIndex, out int PreviousLineIndex))
                    previous.Lines[PreviousLineIndex].CalculatedIndex = next.Lines[NextLineIndex].BestIndex;
        }

        public bool GetPreviousLineIndex(IFourComponentsAlcohol next, IFourComponentsAlcohol previous, ComponentAssociationCollection associationList1, ComponentAssociationCollection associationList2, ComponentAssociationCollection associationList3, ComponentAssociationCollection associationList4, int nextLineIndex, out int previousLineIndex)
        {
            IFourComponentsAlcoholLine Line = (IFourComponentsAlcoholLine)next.Lines[nextLineIndex];
            int BestIndex = Line.BestIndex;
            if (BestIndex >= 0)
            {
                int PreviousIndex1 = GetNextToPreviousIndex(associationList1, Line.Index1);
                int PreviousIndex2 = GetNextToPreviousIndex(associationList2, Line.Index2);
                int PreviousIndex3 = GetNextToPreviousIndex(associationList3, Line.Index3);
                int PreviousIndex4 = GetNextToPreviousIndex(associationList4, Line.Index4);

                if (PreviousIndex1 >= 0 && PreviousIndex2 >= 0 && PreviousIndex3 >= 0 && PreviousIndex4 >= 0)
                {
                    int Multiplier1 = next.Multiplier1;
                    int Multiplier2 = next.Multiplier2;
                    int Multiplier3 = next.Multiplier3;

                    previousLineIndex = (PreviousIndex1 * Multiplier1) + (PreviousIndex2 * Multiplier2) + (PreviousIndex3 * Multiplier3) + PreviousIndex4;
                    return true;
                }
            }

            previousLineIndex = -1;
            return false;
        }

        private int GetPreviousToNextIndex(ComponentAssociationCollection associationList, int previousIndex)
        {
            int NextIndex;

            if (associationList.IsNone)
                NextIndex = previousIndex;
            else if (associationList[previousIndex].AssociationIndex >= 0)
                NextIndex = associationList[previousIndex].AssociationIndex;
            else
                NextIndex = -1;

            return NextIndex;
        }

        private int GetNextToPreviousIndex(ComponentAssociationCollection associationList, int nextIndex)
        {
            int PreviousIndex;

            if (associationList.IsNone)
                PreviousIndex = nextIndex;
            else
            {
                PreviousIndex = -1;

                for (int i = 0; i < associationList.Count; i++)
                    if (associationList[i].AssociationIndex == nextIndex)
                    {
                        PreviousIndex = i;
                        break;
                    }
            }

            return PreviousIndex;
        }

        public static void SetChanged()
        {
            MainWindow Window = (MainWindow)App.Current.MainWindow;
            Window.IsChanged = true;
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
    }
}
