﻿namespace PgBrew
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Alcoholx4x4x2Line : INotifyPropertyChanged
    {
        public Alcoholx4x4x2Line(Alcoholx4x4x2 owner, int index1, int index2, int index3, int effectIndex)
        {
            Owner = owner;
            Index1 = index1;
            Index2 = index2;
            Index3 = index3;
            _EffectIndex = effectIndex;
        }

        public Alcoholx4x4x2 Owner { get; }
        public int Index1 { get; }
        public int Index2 { get; }
        public int Index3 { get; }
        public int EffectIndex
        {
            get { return _EffectIndex; }
            set
            {
                if (_EffectIndex != value)
                {
                    _EffectIndex = value;
                    MainWindow.IsChanged = true;

                    NotifyThisPropertyChanged();
                }
            }
        }
        private int _EffectIndex;

        public string Component1 { get { return Owner.ComponentList1[Index1].Name; } }
        public string Component2 { get { return Owner.ComponentList2[Index2].Name; } }
        public string Component3 { get { return Owner.ComponentList3[Index3].Name; } }
        public string Effect { get { return EffectIndex >= 0 && EffectIndex < Owner.EffectList.Count ? Owner.EffectList[EffectIndex] : null; } }

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
    }
}
