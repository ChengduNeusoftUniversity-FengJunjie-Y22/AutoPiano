using FastHotKeyForWPF;
using System.Windows;
using System.Windows.Input;

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
            BinaryObject.CheckDataFloder();
            AudioBasic.UpdateAudioByType(InstrumentTypes.FWPiano);
            TxtAnalizeVisual.InstrumentType = InstrumentTypes.FWPiano;

            HotKeySet.LoadPage();

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