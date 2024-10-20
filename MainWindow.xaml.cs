using FastHotKeyForWPF;
using Microsoft.Expression.Shapes;
using MinimalisticWPF;
using OpenCvSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        protected override void OnSourceInitialized(EventArgs e)
        {
            GlobalHotKey.Awake();

            FileTool.CheckDataFloder();
            AudioBasic.UpdateAudioByType(InstrumentTypes.FWPiano);
            TxtAnalizeVisual.InstrumentType = InstrumentTypes.FWPiano;

            TxtAnalizeVisual.Instance?.LoadFixedHotKey();

            TempInfos.LoadTempInfo();
            TempInfos.UseTempInfo();

            base.OnSourceInitialized(e);

            DynamicVisionGroup.SetPopupScreen();

            GlobalHotKey.Add(ModelKeys.CTRL, NormalKeys.F4, (sender, e) =>
            {
                DynamicVisionGroup.UpdateArea();
            });
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.F4);

            GlobalHotKey.Add(ModelKeys.CTRL | ModelKeys.ALT, NormalKeys.F1, async (sender, e) =>
            {
                await DynamicVisionGroup.Start();
            });
            GlobalHotKey.Add(ModelKeys.CTRL | ModelKeys.ALT, NormalKeys.F2, (sender, e) =>
            {
                DynamicVisionGroup.Parse(TxtAnalizeVisual.CurrentSong);
            });

        }

        protected override void OnClosed(EventArgs e)
        {
            GlobalHotKey.Destroy();

            TempInfos.Update();
            TempInfos.SaveTempInfo();

            DynamicVisionGroup.Popup.IsOpen = false;

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