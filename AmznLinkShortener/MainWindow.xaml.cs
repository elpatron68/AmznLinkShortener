using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace AmznLinkShortener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Properties.Settings.Default.activateClipboardMonitor == true)
            {
                Debug.WriteLine("Activating clipboard monitor");
                ActivateClipboardMonitor();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShortenUrl(txUrl.Text);
        }

        private void ActivateClipboardMonitor()
        {
            cbMonitorClipboard.IsChecked = true;
            // Initialize the clipboard now that we have a window source to use
            ClipboardManager windowClipboardManager = new ClipboardManager(this);
            windowClipboardManager.ClipboardChanged += ClipboardChanged;
        }

        private void ClipboardChanged(object sender, EventArgs e)
        {
            // Handle your clipboard update here, debug logging example:
            System.Threading.Thread.Sleep(100);
            try
            {
                if (Clipboard.ContainsText() && AmznShorten.IsAmazonLongUrl(Clipboard.GetText()))
                {
                    ShortenUrl(Clipboard.GetText());
                }
            }
            catch(Exception)
            {
                Debug.WriteLine("Clipboard not accessible, trying again in 100 ms");
                ClipboardChanged(sender, e);
            }
        }

        private void cbMonitorClipboard_Checked(object sender, RoutedEventArgs e)
        {
            if (cbMonitorClipboard.IsChecked == true)
            {
                ActivateClipboardMonitor();
                Properties.Settings.Default.activateClipboardMonitor = true;
            }
            else
            {
                Properties.Settings.Default.activateClipboardMonitor = false;
            }
        }

        private void ShortenUrl(string url)
        {
            if (AmznShorten.IsAmazonLongUrl(url))
            {
                string shorturl = AmznShorten.ShortenUrl(txUrl.Text);
                txUrl.Text = shorturl;
                Debug.WriteLine("Url shortended to " + shorturl);
                Clipboard.SetText(shorturl);
            }
        }

        private void txUrl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Fixes issue when clicking cut/ copy / paste in context menu
            if (txUrl.SelectionLength == 0)
                txUrl.SelectAll();
        }

        private void txUrl_LostMouseCapture(object sender, MouseEventArgs e)
        {
            // If user highlights some text, don't override it
            if (txUrl.SelectionLength == 0)
                txUrl.SelectAll();

            // further clicks will not select all
            txUrl.LostMouseCapture -= txUrl_LostMouseCapture;
        }

        private void txUrl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // once we've left the TextBox, return the select all behavior
            txUrl.LostMouseCapture += txUrl_LostMouseCapture;
        }
    }
}
