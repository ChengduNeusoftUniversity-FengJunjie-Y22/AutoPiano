using FastHotKeyForWPF;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;

/// <summary>
///  【MainWindow】当前页面类型
/// </summary>
public enum PageTypes
{
    TxtAnalize,
    NMNAnalize,
    HotKeySet,
    GameInside
}

namespace AutoPiano
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance;

        public static Song AutoTarget = new Song();

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
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