using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Salaros;
using Salaros.Configuration;

namespace R6ServerChanger;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
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
    }

    private void SaveServerButton_Click(object sender, RoutedEventArgs e)
    {
        if (UUIDComboBox.SelectedItem as string is null)
            MessageBox.Show("Please select a UUID", "Oops!", MessageBoxButton.OK, MessageBoxImage.Warning);

        if (R6ConfigPath is null)
            MessageBox.Show("Failed getting GameSettings.ini", "Oops!", MessageBoxButton.OK, MessageBoxImage.Warning);

        if ((string)UUIDComboBox.SelectedItem != null && R6ConfigPath is not null)
        {
            if (ServerComboBox.SelectedItem as string is not null)
                R6ConfigPath.SetValue("ONLINE", "DataCenterHint", ServerComboBox.SelectedItem as string == "default" ? ServerComboBox.SelectedItem as string : $@"playfab/{ServerComboBox.SelectedItem}");

            R6ConfigPath.Save();
        }
    }

    private void UUIDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        R6ConfigPath = new ConfigParser($@"{SiegeDocPath}\{UUIDComboBox.SelectedItem}\GameSettings.ini");
        CurrentServer = R6ConfigPath.GetValue("ONLINE", "DataCenterHint");
        if (ServerComboBox.IsEnabled == false)
            ServerComboBox.IsEnabled = true;
        ServerComboBox.ItemsSource = ServerList;
        ServerComboBox.SelectedItem = CurrentServer.Contains("playfab/") ? CurrentServer.Split("/")[1] : CurrentServer;
    }
}