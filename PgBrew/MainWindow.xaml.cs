﻿namespace PgBrew
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static readonly string AssociationSettingName = "Association";
        private static readonly string GuiSettingName = "GUI";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadAssociations();
            LoadGUI();
            _IsChanged = false;
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

        public bool IsChanged
        {
            get { return _IsChanged; }
            private set
            {
                if (_IsChanged != value)
                {
                    _IsChanged = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private bool _IsChanged;

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

        public static Component Strawberry { get; } = new Component("Strawberry");
        public static Component GreenPepper { get; } = new Component("Green Pepper");
        public static Component RedPepper { get; } = new Component("Red Pepper");
        public static Component Molasses { get; } = new Component("Molasses");
        public static Component Corn { get; } = new Component("Corn");

        private static List<Component> FruitTier1Three = new List<Component>() { RedApple, Grapes, Orange };
        private static List<Component> FruitTier1Four = new List<Component>() { RedApple, Grapes, Orange, Strawberry };
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

        private void OnSave(object sender, RoutedEventArgs e)
        {
            SaveAll();
            IsChanged = false;
        }

        public void Recalculate()
        {
            Recalculate(OrcishBock, BrownAle);
            Recalculate(BrownAle, HegemonyLager);
            Recalculate(HegemonyLager, DwarvenStout);
        }

        public void Recalculate(Alcoholx4x3x4x3 previous, Alcoholx4x3x4x3 next)
        {
            foreach (Alcoholx4x3x4x3Line Line in previous.Lines)
                if (Line.EffectIndex >= 0)
                {
                    Component PreviousComponent1 = previous.ComponentList1[Line.Index1];
                    Component PreviousComponent2 = previous.ComponentList2[Line.Index2];
                    Component PreviousComponent3 = previous.ComponentList3[Line.Index3];
                    Component PreviousComponent4 = previous.ComponentList4[Line.Index4];

                    int NextIndex1 = -1;
                    int NextIndex2 = -1;
                    int NextIndex3 = -1;
                    int NextIndex4 = -1;

                    foreach (ComponentAssociationCollection AssociationList in AssociationTable)
                        foreach (ComponentAssociation Association in AssociationList)
                        {
                            if (Association.Component == PreviousComponent1 && Association.ChoiceList == next.ComponentList1 && Association.AssociationIndex >= 0)
                                NextIndex1 = Association.AssociationIndex;
                            if (Association.Component == PreviousComponent2 && Association.ChoiceList == next.ComponentList2 && Association.AssociationIndex >= 0)
                                NextIndex2 = Association.AssociationIndex;
                            if (Association.Component == PreviousComponent3 && Association.ChoiceList == next.ComponentList3 && Association.AssociationIndex >= 0)
                                NextIndex3 = Association.AssociationIndex;
                            if (Association.Component == PreviousComponent4 && Association.ChoiceList == next.ComponentList4 && Association.AssociationIndex >= 0)
                                NextIndex4 = Association.AssociationIndex;
                        }

                    if (NextIndex1 >= 0 && NextIndex2 >= 0 && NextIndex3 >= 0 && NextIndex4 >= 0)
                    {
                        int NextLineIndex = (NextIndex1 * 3 * 4 * 3) + (NextIndex2 * 4 * 3) + (NextIndex3 * 3) + NextIndex4;
                        next.Lines[NextLineIndex].CalculatedIndex = Line.EffectIndex;
                    }
                }

            foreach (Alcoholx4x3x4x3Line Line in next.Lines)
                if (Line.EffectIndex >= 0)
                {
                    int NextIndex1 = Line.Index1;
                    int NextIndex2 = Line.Index2;
                    int NextIndex3 = Line.Index3;
                    int NextIndex4 = Line.Index4;

                    int PreviousIndex1 = -1;
                    int PreviousIndex2 = -1;
                    int PreviousIndex3 = -1;
                    int PreviousIndex4 = -1;

                    foreach (ComponentAssociationCollection AssociationList in AssociationTable)
                        foreach (ComponentAssociation Association in AssociationList)
                        {
                            if (Association.ChoiceList == next.ComponentList1 && Association.AssociationIndex == NextIndex1)
                                PreviousIndex1 = previous.ComponentList1.IndexOf(Association.Component);
                            if (Association.ChoiceList == next.ComponentList2 && Association.AssociationIndex == NextIndex2)
                                PreviousIndex2 = previous.ComponentList2.IndexOf(Association.Component);
                            if (Association.ChoiceList == next.ComponentList3 && Association.AssociationIndex == NextIndex3)
                                PreviousIndex3 = previous.ComponentList3.IndexOf(Association.Component);
                            if (Association.ChoiceList == next.ComponentList4 && Association.AssociationIndex == NextIndex4)
                                PreviousIndex4 = previous.ComponentList4.IndexOf(Association.Component);
                        }

                    if (PreviousIndex1 >= 0 && PreviousIndex2 >= 0 && PreviousIndex3 >= 0 && PreviousIndex4 >= 0)
                    {
                        int PreviousLineIndex = (PreviousIndex1 * 3 * 4 * 3) + (PreviousIndex2 * 4 * 3) + (PreviousIndex3 * 3) + PreviousIndex4;
                        previous.Lines[PreviousLineIndex].CalculatedIndex = Line.EffectIndex;
                    }
                }
        }

        public static void SetChanged()
        {
            MainWindow Window = App.Current.MainWindow as MainWindow;
            Window.IsChanged = true;
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void NotifyThisPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        #endregion

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            ComponentAssociation Association = (sender as Button).DataContext as ComponentAssociation;
            Association.AssociationIndex = -1;
        }
    }
}
