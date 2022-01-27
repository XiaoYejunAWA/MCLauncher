using System.Windows;
using System.Windows.Controls;

namespace MCLauncher.LoginUI
{
    /// <summary>
    /// skinlogin.xaml 的交互逻辑
    /// </summary>
    public partial class skinlogin : Page
    {
        public skinskin skinskin = new skinskin();
        public SquareMinecraftLauncher.Minecraft.Skin skin = new SquareMinecraftLauncher.Minecraft.Skin();

        public skinlogin()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SquareMinecraftLauncher.Minecraft.Tools tools = new SquareMinecraftLauncher.Minecraft.Tools();
            //var t = Task.Run(() =>
            //{
            //    return tools.GetAuthlib_Injector(Uri.Text, username.Text, password.Password);
            //});
            //await t;
            skin = tools.GetAuthlib_Injector(Uri.Text, username.Text, password.Password);
            Content = new Frame() { Content = skinskin };
            skinskin.Player.ItemsSource = skin.NameItem;
        }
    }
}