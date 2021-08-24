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

        LoginUI.LiXian LiXian = new LoginUI.LiXian();
        LoginUI.WeiRuan WeiRuan = new LoginUI.WeiRuan();
        LoginUI.ZhengBan ZhengBan = new LoginUI.ZhengBan();
        public int launchMode = 1;
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
            LaunchOptions launchOptions = new LaunchOptions();
            switch (launchMode)
            {
                case 1:
                    launchOptions.Authenticator = new OfflineAuthenticator(LiXian.IDText.Text);
                    break;
                case 2:
                    launchOptions.Authenticator = new YggdrasilLogin(ZhengBan.Email.Text,ZhengBan.password.Password,false);
                    break;
            }
            
            launchOptions.MaxMemory = Convert.ToInt32(MemoryTextbox.Text);
            if (versionCombo.Text != string.Empty&&
                javaCombo.Text != string.Empty&&
                (LiXian.IDText.Text!=string.Empty||(ZhengBan.Email.Text != string.Empty&&ZhengBan.password.Password != string.Empty)&&
                MemoryTextbox.Text!=string.Empty))
            {
                try
                {
                    Core.JavaPath = javaCombo.Text;
                    var ver = (KMCCC.Launcher.Version)versionCombo.SelectedItem;
                    launchOptions.Version = ver;

                    var result = Core.Launch(launchOptions);
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

        /// <summary>
        /// 离线登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ContentControl1.Content = new Frame
            {
                Content = LiXian
            };
            launchMode = 1;
        }

        /// <summary>
        /// 正版登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ContentControl1.Content = new Frame
            {
                Content = ZhengBan
            };
            launchMode = 2;
        }

        /// <summary>
        /// 微软登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ContentControl1.Content = new Frame
            {
                Content = WeiRuan
            };
            launchMode = 3;
        }
    }
}
