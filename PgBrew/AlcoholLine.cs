﻿namespace PgBrew
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class AlcoholLine : INotifyPropertyChanged
    {
        public AlcoholLine(Alcohol owner, int effectIndex)
        {
            Owner = owner;
            _EffectIndex = effectIndex;
            _CalculatedIndex = -1;
        }

        public Alcohol Owner { get; }

        public int EffectIndex
        {
            get { return _EffectIndex; }
            set
            {
                if (_EffectIndex != value)
                {
                    _EffectIndex = value;
                    MainWindow.SetChanged();

                    NotifyThisPropertyChanged();

                    MainWindow Window = App.Current.MainWindow as MainWindow;
                    Window.Recalculate();
                }
            }
        }
        private int _EffectIndex;

        public Effect Effect { get { return EffectIndex >= 0 && EffectIndex < Owner.EffectList.Count ? Owner.EffectList[EffectIndex] : null; } }

        public int CalculatedIndex
        {
            get { return _CalculatedIndex; }
            set
            {
                if (_CalculatedIndex != value)
                {
                    _CalculatedIndex = value;
                    NotifyPropertyChanged(nameof(CalculatedEffect));
                }
            }
        }
        private int _CalculatedIndex;

        public Effect CalculatedEffect
        {
            get
            {
                if (EffectIndex >= 0)
                    return null;

                return _CalculatedIndex >= 0 && _CalculatedIndex < Owner.EffectList.Count ? Owner.EffectList[_CalculatedIndex] : null;
            }
        }

        public int BestIndex { get { return _EffectIndex >= 0 ? _EffectIndex : _CalculatedIndex >= 0 ? _CalculatedIndex : -1; } }

        public void ClearCalculateIndex()
        {
            CalculatedIndex = -1;
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
    }
}
