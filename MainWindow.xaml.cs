using FastHotKeyForWPF;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AutoPiano
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            GlobalHotKey.Awake();

            AudioBasic.CheckAudioFolder();
            StringProcessing.CheckTxtFloder();
            XmlObject.CheckDataFloder();
            //AudioBasic.UpdateAudioByType(InstrumentTypes.FWPiano);
            AudioBasic.UpdateAudioByType(InstrumentTypes.JHPiano);

            base.OnSourceInitialized(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            GlobalHotKey.Destroy();
            base.OnClosed(e);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Viewbox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Sidebar.Instance != null && SuperMenu.IsSideBarOpen)
            {
                Sidebar.Instance.UnExpandTAB();
            }
        }
    }
}