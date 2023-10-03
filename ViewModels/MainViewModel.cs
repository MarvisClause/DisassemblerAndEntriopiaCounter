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

        public MainViewModel()
        {
            // Default view
            ExecuteShowDashboardViewCommand(null); 

            //Initialize commands
            ShowDashboardViewCommand = new ViewModelCommand(ExecuteShowDashboardViewCommand);
            ShowResearchViewCommand = new ViewModelCommand(ExecuteShowResearchViewCommand);

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
    }
}