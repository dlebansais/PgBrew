﻿namespace PgBrew
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            IsChanged = false;
        }

        public static bool IsChanged { get; set; }

        public Alcoholx4 BasicLager { get; private set; } = new Alcoholx4("Basic Lager", 
            new List<string>()
            {
                "Red Apple",
                "Grapes",
                "Orange",
                "Strawberry",
            });

        public Alcoholx4x4 PaleAle { get; private set; } = new Alcoholx4x4("Pale Ale",
            new List<string>()
            {
                "Red Apple",
                "Grapes",
                "Orange",
                "Strawberry",
            },
            new List<string>()
            {
                "Parasol Mushroom Flakes",
                "Mycena Mushroom Flakes",
                "Boletus Mushroom Flakes",
                "Field Mushroom Flakes",
            });

        public Alcoholx4x4x2 Marzen { get; private set; } = new Alcoholx4x4x2("Marzen",
            new List<string>()
            {
                "Red Apple",
                "Grapes",
                "Orange",
                "Strawberry",
            },
            new List<string>()
            {
                "Parasol Mushroom Flakes",
                "Mycena Mushroom Flakes",
                "Boletus Mushroom Flakes",
                "Field Mushroom Flakes",
            },
            new List<string>()
            {
                "Oregano",
                "Mandrake Root",
            });

        public Alcoholx3x3x4x3 GoblinAle { get; private set; } = new Alcoholx3x3x4x3("Goblin Ale",
            new List<string>()
            {
                "Red Apple",
                "Grapes",
                "Orange",
            },
            new List<string>()
            {
                "Guava",
                "Banana",
                "Lemon",
            },
            new List<string>()
            {
                "Parasol Mushroom Flakes",
                "Mycena Mushroom Flakes",
                "Boletus Mushroom Flakes",
                "Field Mushroom Flakes",
            },
            new List<string>()
            {
                "Oregano",
                "Mandrake Root",
                "Peppercorns",
            });

        public Alcoholx4x3x4x3 OrcishBock { get; private set; } = new Alcoholx4x3x4x3("Orcish Bock",
            new List<string>()
            {
                "Beet",
                "Squash",
                "Broccoli",
                "Carrot",
            },
            new List<string>()
            {
                "Guava",
                "Banana",
                "Lemon",
            },
            new List<string>()
            {
                "Field Mushroom Flakes",
                "Blusher Mushroom Flakes",
                "Milk Cap Mushroom Powder",
                "Blood Mushroom Powder",
            },
            new List<string>()
            {
                "Oregano",
                "Mandrake Root",
                "Peppercorns",
            });

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
        }

        private void SaveAll()
        {
            BasicLager.Save();
            PaleAle.Save();
            Marzen.Save();
        }
    }
}
