namespace MusicPlayer.Windows
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void BaseWindow_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
