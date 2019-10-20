using System;
using System.Runtime.InteropServices;

namespace DanielsWpfCoaster.Windows
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement
    {
        public int Length;
        public int Flags;
        public ShowWindowMode ShowMode;
        public Win32Point MinPosition;
        public Win32Point MaxPosition;
        public Win32Rect NormalBounds;
    }
}