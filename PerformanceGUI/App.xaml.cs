using System;
using System.Windows;

namespace PerformanceGUI
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += (s, e) =>
            {
                MessageBox.Show(e.Exception.ToString(), "DispatcherUnhandledException");
                e.Handled = true; 
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                MessageBox.Show(e.ExceptionObject.ToString(), "AppDomain UnhandledException");
            };
        }
    }
}