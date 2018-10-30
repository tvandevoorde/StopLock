using System;
using System.Runtime.InteropServices;
using WindowsInput;

namespace StopLock
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            InputSimulator sim = new InputSimulator();
            uint envTicks = 0;
            uint idleTicks = 0;
            uint idleTime = 0;
            uint lastInputTick = 0;

            var handle = GetConsoleWindow();
            ShowWindow(handle, 0);

            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            do
            {
                envTicks = (uint)Environment.TickCount;
                if (GetLastInputInfo(ref lastInputInfo))
                {
                    lastInputTick = lastInputInfo.dwTime;
                    idleTicks = envTicks - lastInputTick;
                    idleTime = ((idleTicks > 0) ? (idleTicks / 1000) : 0);
                }

                if (idleTime > 100)
                {
                    sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F24);
                }
                System.Threading.Thread.Sleep(10000);
            } while (true);

        }

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public uint cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
    }
}
