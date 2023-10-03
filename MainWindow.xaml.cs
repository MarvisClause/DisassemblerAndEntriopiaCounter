using Microsoft.Win32;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using DisEn.ViewModels;

namespace DisEn
{
    public partial class MainWindow : Window
    {
        private DisassemblerManager _disassemblerManager;

        private DisassemblerComparator _disassemblerComparator;


        public MainWindow()
        {
            InitializeComponent();
            // Initialize disassembler
            _disassemblerManager = new DisassemblerManager();
            _disassemblerComparator = new DisassemblerComparator();
            // Hide additional interface
            //SetCastDataVisibility(false);



        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr wParam = new IntPtr(2); // Convert 2 to IntPtr
            IntPtr lParam = IntPtr.Zero;   // Use IntPtr.Zero for the third parameter
            SendMessage(helper.Handle, 161, wParam, lParam);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else this.WindowState = WindowState.Normal;
        }
    }
}