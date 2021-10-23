using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
