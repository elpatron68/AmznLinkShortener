using System;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmznLinkShortener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly Queue<string> statusMessages;

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
            statusMessages = new Queue<string>();
            EnqueueStatusMessage("Ready");
            tgBitly.IsOn = Properties.Settings.Default.useBitly;
            tgSmile.IsOn = Properties.Settings.Default.useSmile;
        }

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            string su = await ShortenUrl(txUrl.Text);
            if (su != "")
            {
                
                try
                {
                    Clipboard.SetText(su);
                    EnqueueStatusMessage("Shortened link copied to clipboard");
                }
                catch
                {
                    EnqueueStatusMessage("Clipboard not accessible");
                }
            }
        }

        private void ClipboardChanged(object sender, EventArgs e)
        {
            // Try not to interfere with other clipboard handlers
            Thread.Sleep(59);
            // Handle clipboard update
            try
            {
                string clip = Clipboard.GetText();
                Debug.WriteLine("Received " + clip + " from clipboard");
                if (Clipboard.ContainsText() && AmznShorten.IsAmazonLongUrl(clip))
                {
                    _ = ShortenUrl(clip);
                }
                else if (!AmznShorten.IsAmazonLongUrl(clip))
                {
                    EnqueueStatusMessage("No Amazon product link found");
                }
            }
            catch(Exception)
            {
                EnqueueStatusMessage("Clipboard not accessible");
            }
        }

        private async Task<string> ShortenUrl(string url)
        {
            if (AmznShorten.IsAmazonLongUrl(url))
            {
                string shorturl = await AmznShorten.ShortenUrl(url, tgBitly.IsOn, tgSmile.IsOn);

                txUrl.Text = shorturl;
                Debug.WriteLine("Url shortended to " + shorturl);
                return shorturl;
            }
            else
            {
                EnqueueStatusMessage("No Amazon link detected in clipboad data");
                return "";
            }
        }

        #region Textbox behaviour
        private void TxUrl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Fixes issue when clicking cut/ copy / paste in context menu
            if (txUrl.SelectionLength == 0)
            {
                txUrl.SelectAll();
            }
        }

        private void TxUrl_LostMouseCapture(object sender, MouseEventArgs e)
        {
            // If user highlights some text, don't override it
            if (txUrl.SelectionLength == 0)
            {
                txUrl.SelectAll();
            }
            // further clicks will not select all
            txUrl.LostMouseCapture -= TxUrl_LostMouseCapture;
        }

        private void TxUrl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // once we've left the TextBox, return the select all behavior
            txUrl.LostMouseCapture += TxUrl_LostMouseCapture;
        }
        #endregion

        // About dailog
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string aVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            MessageBox.Show("AmznLinkShortener v" + aVersion + "\n\nelpatron@mailbox.org\n\nLicense: GPL 3 (see License.txt)\nSourcecode: https://github.com/elpatron68/AmznLinkShortener", "About");
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(txUrl.Text);
            EnqueueStatusMessage("Link copied to clipboard");
        }

        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            txUrl.Text = Clipboard.GetText();
            EnqueueStatusMessage("Link pasted from clipboard");
        }

        // Save Bitly setting
        private void TgBitly_Toggled(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.useBitly = tgBitly.IsOn;
            Properties.Settings.Default.Save();
            if (tgBitly.IsOn)
            {
                EnqueueStatusMessage("Bitly activated");
            }
            else
            {
                EnqueueStatusMessage("Bitly deactivated");
            }
        }

        // Save Amazon Smile setting
        private void TgSmile_Toggled(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.useSmile = tgSmile.IsOn;
            Properties.Settings.Default.Save();
            if (tgSmile.IsOn)
            {
                EnqueueStatusMessage("Amzn Smile activated");
            }
            else
            {
                EnqueueStatusMessage("Amzn Smile deactivated");
            }
        }

        // Display message, enqueue last 10 messages and display them as tooltip
        private void EnqueueStatusMessage(string message)
        {
            Debug.WriteLine(message);
            string prefix = DateTime.Now.ToString("HH:mm:ss");
            statusMessages.Enqueue(prefix + " - " + message);
            if (statusMessages.Count > 9)
            {
                statusMessages.Dequeue();
            }
            string messagetext = string.Join("\n", statusMessages);
            sbStatus.ToolTip= messagetext;
            lblStatus.Text = message;
        }
    }
}