﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WPFLocales;

namespace SampleApplication
{
    class ViewModel : INotifyPropertyChanged
    {
        public State State
        {
            get { return _state; }
            set { _state = value; RaisePropertyChanged(); }
        }
        private State _state;

        public ObservableCollection<string> Locales { get; set; }
        public string CurrentLocale
        {
            get { return Localization.CurrentLocale; }
            set { Localization.CurrentLocale = value; RaisePropertyChanged(); }
        }


        public ICommand ChangeStateCommand { get; set; }

        public ViewModel()
        {
            Locales = new ObservableCollection<string>(Localization.Locales);
            ChangeStateCommand = new DelegateCommand(() =>
            {
                State = State == State.Error ? State.Ok : State.Error;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    enum State
    {
        Ok,
        Error
    }
}