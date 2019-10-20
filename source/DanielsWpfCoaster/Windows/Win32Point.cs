using System;
using System.Runtime.InteropServices;

namespace DanielsWpfCoaster.Windows
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public readonly int X;
        public readonly int Y;

        public Win32Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}