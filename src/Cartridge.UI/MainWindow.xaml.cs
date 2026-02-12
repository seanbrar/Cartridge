using System;
using System.Windows;
using System.Windows.Controls;

namespace Cartridge.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeDriveLetters();
            MountButton.Click += MountButton_Click;
        }

        private void InitializeDriveLetters()
        {
            // Populate available drive letters (excluding system drives)
            for (char c = 'D'; c <= 'Z'; c++)
            {
                DriveLetterComboBox.Items.Add(c + ":");
            }
            DriveLetterComboBox.SelectedIndex = 0;
        }

        private void MountButton_Click(object sender, RoutedEventArgs e)
        {
            string server = ServerTextBox.Text;
            string sharePath = SharePathTextBox.Text;
            string driveLetter = DriveLetterComboBox.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(sharePath))
            {
                MessageBox.Show("Please enter both server and share path.", "Input Required");
                return;
            }

            // TODO: Implement NFS mount logic here
            // This is where you'll integrate with your NFS implementation

            // Example of adding to ListView (replace with actual mounting logic)
            var item = new { Drive = driveLetter, Server = server, SharePath = sharePath, Status = "Mounting..." };
            MountedSharesListView.Items.Add(item);
            StatusTextBlock.Text = $"Mounting {server}:{sharePath} to {driveLetter}...";
        }
    }
}