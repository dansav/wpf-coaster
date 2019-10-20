using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DanielsWpfCoaster.Windows
{
    public class WindowWithPositionPersistence : Window
    {
        private readonly IWindowBoundsRepository _repo;

        public WindowWithPositionPersistence(IWindowBoundsRepository repo)
        {
            _repo = repo;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            LoadWindowStateAndPosition();
        }

        // WARNING - Not fired when Application.SessionEnding is fired
        protected override void OnClosing(CancelEventArgs e)
        {
            SaveWindowStateAndPosition();
            base.OnClosing(e);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void LoadWindowStateAndPosition()
        {
            try
            {
                WindowPlacement wp = _repo.LoadBoundsOrDefault();
                wp.Length = Marshal.SizeOf(typeof(WindowPlacement));
                wp.Flags = 0;
                wp.ShowMode = wp.ShowMode == ShowWindowMode.Minimized ? ShowWindowMode.Normal : wp.ShowMode;
                var windowHandle = new WindowInteropHelper(this).Handle;

                if (wp.ShowMode == ShowWindowMode.Normal && !wp.NormalBounds.HasArea)
                {
                    // set default position
                    var workAreaWidth = SystemParameters.WorkArea.Width;
                    var workAreaHeight = SystemParameters.WorkArea.Height;
                    Width = workAreaWidth * 0.5;
                    Height = workAreaHeight * 0.5;
                    Left = (workAreaWidth - Width) * 0.5;
                    Top = (workAreaHeight - Height) * 0.5;
                }
                else
                {
                    NativeMethods.SetWindowPlacement(windowHandle, in wp);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void SaveWindowStateAndPosition()
        {
            var windowHandle = new WindowInteropHelper(this).Handle;
            NativeMethods.GetWindowPlacement(windowHandle, out var windowPlacement);
            _repo.StoreBounds(windowPlacement);
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool SetWindowPlacement(IntPtr hWnd, in WindowPlacement windowPlacement);

            [DllImport("user32.dll")]
            public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement windowPlacement);
        }
    }
}
