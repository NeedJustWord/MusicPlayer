namespace MusicPlayer.ViewModel
{
    public class ViewModelLocator
    {
        private MainWindowViewModel _mainWindowVm;
        public MainWindowViewModel MainWindowVm => _mainWindowVm ?? (_mainWindowVm = new MainWindowViewModel());
    }
}
