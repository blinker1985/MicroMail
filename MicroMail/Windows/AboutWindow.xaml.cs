namespace MicroMail.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : ISingularWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        public string SingularId {
            get { return "AboutWindow"; }
        }
    }
}
