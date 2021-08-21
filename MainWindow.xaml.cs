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
using Panuon.UI.Silver;
using KMCCC.Launcher;
using KMCCC.Authentication;
using SquareMinecraftLauncher;
using System.Collections;

namespace MCLauncher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {


        SquareMinecraftLauncher.Minecraft.Tools tools = new SquareMinecraftLauncher.Minecraft.Tools();
        public static LauncherCore Core = LauncherCore.Create();
        public MainWindow()
        {
            InitializeComponent();
            
            //自动找版本
            var versions = Core.GetVersions().ToArray();
            versionCombo.ItemsSource = versions;//绑定数据源
            //自动找java
            List<string> javaList = new List<string>();
            foreach(string i in KMCCC.Tools.SystemTools.FindJava())
            {
                javaList.Add(i);
            }
            javaList.Add(tools.GetJavaPath());
            javaCombo.ItemsSource = javaList;
            //初始选择
            versionCombo.SelectedItem = versionCombo.Items[0];
            javaCombo.SelectedItem = javaCombo.Items[0];
        }
        public void GameStart()
        {
            if (versionCombo.Text != string.Empty&&javaCombo.Text != string.Empty&&IdTextbox.Text!=string.Empty&&MemoryTextbox.Text!=string.Empty)
            {
                try
                {
                    Core.JavaPath = javaCombo.Text;
                    var ver = (KMCCC.Launcher.Version)versionCombo.SelectedItem;
                    var result = Core.Launch(new LaunchOptions
                    {
                        Version = ver, //Ver为Versions里你要启动的版本名字
                        MaxMemory = Convert.ToInt32(MemoryTextbox.Text), //最大内存，int类型
                        Authenticator = new OfflineAuthenticator(IdTextbox.Text), //离线启动，ZhaiSoul那儿为你要设置的游戏名
                                                                                //Authenticator = new YggdrasilLogin("邮箱", "密码", true), // 正版启动，最后一个为是否twitch登录
                        Mode = LaunchMode.MCLauncher, //启动模式，这个我会在后面解释有哪几种
                                                      //Server = new ServerInfo { Address = "服务器IP地址", Port = "服务器端口" }, //设置启动游戏后，自动加入指定IP的服务器，可以不要
                                                      //Size = new WindowSize { Height = 768, Width = 1280 } //设置窗口大小，可以不要
                    });
                    if (!result.Success)
                    {
                        switch (result.ErrorType)
                        {
                            case ErrorType.NoJAVA:
                                MessageBoxX.Show("java错误，详细信息：" + result.ErrorMessage, "错误");
                                break;
                            case ErrorType.AuthenticationFailed:
                                MessageBoxX.Show("登录错误，详细信息：" + result.ErrorMessage, "错误");
                                break;
                            case ErrorType.UncompressingFailed:
                                MessageBoxX.Show("文件错误，详细信息：" + result.ErrorMessage, "错误");
                                break;
                            default:
                                MessageBoxX.Show(result.ErrorMessage, "错误");
                                break;
                        }
                    }

                }
                catch
                {
                    MessageBoxX.Show("启动失败", "错误");
                }
            }
            else
            {
                MessageBoxX.Show("信息未填完整", "错误");
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GameStart();

        }
    }
}
