using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleApplication
{
    class MainWindowViewModel
    {
        public State State
        {
            get { return _state; }
            set { _state = value; RaisePropertyChanged(); }
        }
        private State _state;

        public ObservableCollection<State> States { get; set; }

        public ObservableCollection<string> Locales { get; set; }
        public string CurrentLocale
        {
            get { return WPFLocales.Localization.CurrentLocale; }
            set { WPFLocales.Localization.CurrentLocale = value; RaisePropertyChanged(); }
        }

        public ICommand ChangeStateCommand { get; set; }

        public MainWindowViewModel()
        {
            if (WPFLocales.Localization.AvailableLocales != null)
            {
                Locales = new ObservableCollection<string>(WPFLocales.Localization.AvailableLocales);
            }
            States = new ObservableCollection<State>(new[] { State.Error, State.Ok });
            ChangeStateCommand = new DelegateCommand(() =>
            {
                State = State == State.Error ? State.Ok : State.Error;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    enum State
    {
        Ok,
        Error
    }
}
