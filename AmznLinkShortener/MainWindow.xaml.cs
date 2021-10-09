using System;
using System.Threading;
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
            // Allow just a single instance
            if (!SingleAppInstanceChecker.IsNotRunning())
            {
                MessageBox.Show("Application is already running.","AmznLinkShortener");
                Environment.Exit(1);
                return;
            }
            
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShortenUrl(txUrl.Text);
        }

        private void ActivateClipboardMonitor()
        {
            // Initialize the clipboard now that we have a window source to use
            ClipboardManager windowClipboardManager = new ClipboardManager(this);
            windowClipboardManager.ClipboardChanged += ClipboardChanged;
        }

        private void ClipboardChanged(object sender, EventArgs e)
        {
            // Try not to interfere with other clipboard handlers
            Thread.Sleep(20);
            // Handle clipboard update
            try
            {
                string clip = Clipboard.GetText();
                Debug.WriteLine("Received " + clip + " from clipboard");
                if (Clipboard.ContainsText() && AmznShorten.IsAmazonLongUrl(clip))
                {
                    ShortenUrl(clip);
                    
                }
            }
            catch(Exception)
            {
                Debug.WriteLine("Clipboard not accessible");
                // ClipboardChanged(sender, e);
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
            Properties.Settings.Default.Save();
        }

        private async void ShortenUrl(string url)
        {
            if (AmznShorten.IsAmazonLongUrl(url))
            {
                string shorturl = await AmznShorten.ShortenUrl(url, (bool)!cbBitly.IsChecked);

                txUrl.Text = shorturl;
                Debug.WriteLine("Url shortended to " + shorturl);
                try
                {
                    Clipboard.SetText(shorturl);
                }
                catch
                {
                    Debug.WriteLine("Setting clipboard text failed.");
                    //Thread.Sleep(50);
                    //ShortenUrl(url);
                }
            }
        }

        private void txUrl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Fixes issue when clicking cut/ copy / paste in context menu
            if (txUrl.SelectionLength == 0)
            {
                txUrl.SelectAll();
            }
        }

        private void txUrl_LostMouseCapture(object sender, MouseEventArgs e)
        {
            // If user highlights some text, don't override it
            if (txUrl.SelectionLength == 0)
            {
                txUrl.SelectAll();
            }
            // further clicks will not select all
            txUrl.LostMouseCapture -= txUrl_LostMouseCapture;
        }

        private void txUrl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // once we've left the TextBox, return the select all behavior
            txUrl.LostMouseCapture += txUrl_LostMouseCapture;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("AmznLinkShortener\n\n(c) 2021 elpatron@mailbox.org\n\nLicense: GPL 3 (see License.txt)\nSourcecode: https://github.com/elpatron68/AmznLinkShortener", "About");
        }
    }
}
