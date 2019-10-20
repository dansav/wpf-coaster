using System;
using System.Runtime.InteropServices;

namespace DanielsWpfCoaster.Windows
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Rect
    {
        public readonly int Left;
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;

        public Win32Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        
        public int Width => Right - Left;

        public int Height => Bottom - Top;

        public bool HasArea => Width > 0 && Height > 0;
    }
}