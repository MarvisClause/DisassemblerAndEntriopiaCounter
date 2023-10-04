using FontAwesome.Sharp;
using System.Windows.Input;

namespace DisEn.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ViewModelBase _currentChildView;
        public string _caption;
        public IconChar _icon;

        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        // Commands
        public ICommand ShowDashboardViewCommand { get; }
        public ICommand ShowResearchViewCommand { get; }
        public ICommand ShowCastViewCommand { get; }
        public ICommand ShowFAQViewCommand { get; }
        public ICommand ShowSettingsViewCommand { get; }
        public ICommand ShowReportsViewCommand { get; }

        public MainViewModel()
        {
            // Default view
            ExecuteShowDashboardViewCommand(null);

            //Initialize commands
            ShowDashboardViewCommand = new ViewModelCommand(ExecuteShowDashboardViewCommand);
            ShowResearchViewCommand = new ViewModelCommand(ExecuteShowResearchViewCommand);
            ShowCastViewCommand = new ViewModelCommand(ExecuteShowCastViewCommand);
            ShowFAQViewCommand = new ViewModelCommand(ExecuteShowFAQViewCommand);
            ShowSettingsViewCommand = new ViewModelCommand(ExecuteShowSettingsViewCommand);
            ShowReportsViewCommand = new ViewModelCommand(ExecuteShowReportsViewCommand);

        }

        public void ExecuteShowDashboardViewCommand(object obj)
        {
            CurrentChildView = new DashboardViewModel();
            Caption = "Dashboard";
            Icon = IconChar.ChartSimple;
        }

        public void ExecuteShowResearchViewCommand(object obj)
        {
            CurrentChildView = new ResearchViewModel();
            Caption = "Research";
            Icon = IconChar.Flask;
        }

        public void ExecuteShowCastViewCommand(object obj)
        {
            CurrentChildView = new CastsViewModel();
            Caption = "Casts";
            Icon = IconChar.Clone;
        }

        public void ExecuteShowFAQViewCommand(object obj)
        {
            CurrentChildView = new FAQViewModel();
            Caption = "F.A.Q";
            Icon = IconChar.ClipboardQuestion;
        }
        public void ExecuteShowSettingsViewCommand(object obj)
        {
            CurrentChildView = new SettingsViewModel();
            Caption = "Settings";
            Icon = IconChar.Gear;
        }
        public void ExecuteShowReportsViewCommand(object obj)
        {
            CurrentChildView = new ReportsViewModel();
            Caption = "Reports";
            Icon = IconChar.Flag;
        }
    }
}