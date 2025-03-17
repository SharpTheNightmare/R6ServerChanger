using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Salaros.Configuration;

namespace R6ServerChanger;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private BrushConverter bc = new();

    private readonly BackgroundWorker statusWorker = new();

    public List<string> ServerList =
    [
        "default",
        "australiaeast",
        "brazilsouth",
        "centralus",
        "eastasia",
        "eastus",
        "japaneast",
        "northeurope",
        "southafricanorth",
        "southcentralus",
        "southeastasia",
        "uaenorth",
        "westeurope",
        "westus"
    ];

    public string CurrentServer = "";
    public string SiegeDocPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\My Games\Rainbow Six - Siege\";
    public ConfigParser R6ConfigPath = new();
    public IEnumerable<string> SiegeUser;

    public MainWindow()
    {
        InitializeComponent();
        SiegeUser = Directory.GetDirectories(SiegeDocPath).Select(d => new DirectoryInfo(d).Name);
        UUIDComboBox.ItemsSource = SiegeUser;
        statusWorker.RunWorkerCompleted += Status_RunWorkerCompleted;
    }

    private void SaveServerButton_Click(object sender, RoutedEventArgs e)
    {
        if (UUIDComboBox.SelectedItem as string is null)
            MessageBox.Show("Please select a UUID", "Oops!", MessageBoxButton.OK, MessageBoxImage.Warning);

        if (R6ConfigPath is null)
            MessageBox.Show("Failed to get GameSettings.ini", "Oops!", MessageBoxButton.OK, MessageBoxImage.Warning);

        if ((string?)UUIDComboBox.SelectedItem != null && R6ConfigPath is not null)
        {
            if (ServerComboBox.SelectedItem as string is not null)
                R6ConfigPath.SetValue("ONLINE", "DataCenterHint", ServerComboBox.SelectedItem as string == "default" ? ServerComboBox.SelectedItem as string : $@"playfab/{ServerComboBox.SelectedItem}");

            R6ConfigPath.Save();
            statusWorker.RunWorkerAsync();
        }
    }

    private void UUIDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        R6ConfigPath = new ConfigParser($@"{SiegeDocPath}\{UUIDComboBox.SelectedItem}\GameSettings.ini");
        CurrentServer = R6ConfigPath.GetValue("ONLINE", "DataCenterHint");
        if (ServerComboBox.IsEnabled == false)
            ServerComboBox.IsEnabled = true;
        if (OpenConfig.IsEnabled == false)
            OpenConfig.IsEnabled = true;
        if (SaveServer.IsEnabled == false)
            SaveServer.IsEnabled = true;
        ServerComboBox.ItemsSource = ServerList;
        ServerComboBox.SelectedItem = CurrentServer.Contains("playfab/") ? CurrentServer.Split("/")[1] : CurrentServer;
    }

    private void Status_DoWork(object? sender, DoWorkEventArgs e)
    {
    }

    private void Status_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            StatusLabel.Content = "Server change failed!";
            StatusLabel.Foreground = Brushes.Red;
        }
        else
        {
            StatusLabel.Content = "Server changed!";
            StatusLabel.Foreground = Brushes.Green;
        }
    }

    private void ServerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        StatusLabel.Content = "Idle...";
        StatusLabel.Foreground = bc.ConvertFrom("#ffbf00") as Brush;
    }

    private void OpenConfig_Click(object sender, RoutedEventArgs e)
    {
        if (R6ConfigPath is null)
            MessageBox.Show("Failed to get GameSettings.ini", "Oops!", MessageBoxButton.OK, MessageBoxImage.Warning);
        else
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = $@"{SiegeDocPath}\{UUIDComboBox.SelectedItem}\GameSettings.ini",
                UseShellExecute = true
            });
    }
}