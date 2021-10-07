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
using SquareMinecraftLauncher;
using System.Collections;
using System.Net;
using SquareMinecraftLauncher.Minecraft;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.Win32;

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
        microsoft_launcher.MicrosoftAPIs microsoftAPIs = new microsoft_launcher.MicrosoftAPIs();
        SquareMinecraftLauncher.Minecraft.Game game = new SquareMinecraftLauncher.Minecraft.Game();
        SquareMinecraftLauncher.Minecraft.Tools tools = new SquareMinecraftLauncher.Minecraft.Tools();
        SquareMinecraftLauncher.MinecraftDownload minecraftDownload = new SquareMinecraftLauncher.MinecraftDownload();
        string settingPath = @"Mclauncher.json";
        Setting setting = new Setting();
        RegisterSetting registerSetting = new RegisterSetting();
        public class Setting
        {
            //json数据保存步骤1
            public string Ram = "1024";
        }
        public class RegisterSetting
        {
            //注册表数据保存步骤1
            public string name = "攒钱买电脑的小叶君";
        }

        public void LauncehrInitialization()
        {
            if (!File.Exists(settingPath))
            {
                //json数据保存步骤2
                File.WriteAllText(settingPath, JsonConvert.SerializeObject(setting));
            }
            else
            {
                setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(settingPath));
            }
            bool isFirst=true;
            using(RegistryKey key1 = Registry.LocalMachine.OpenSubKey("SOFTWARE"))
            {
                foreach(var i in key1.GetSubKeyNames())
                {
                    if(i == "MclauncherSetting")
                    {
                        isFirst = false;
                    }
                }
            }
            if (isFirst)
            {
                using (RegistryKey key = Registry.LocalMachine)
                {
                    using (RegistryKey software = key.CreateSubKey("software\\MclauncherSetting"))
                    {
                        //注册表数据保存步骤2
                        software.SetValue("name",registerSetting.name);
                    }
                }
            }
            else
            {
                using (RegistryKey key = Registry.LocalMachine)
                {
                    using (RegistryKey software = key.CreateSubKey("software\\MclauncherSetting"))
                    {
                        //注册表数据保存步骤3
                        registerSetting.name = software.GetValue("name").ToString();
                    }
                }
            }
            LiXian.IDText.Text = registerSetting.name;
            //自动找版本
            var versions = tools.GetAllTheExistingVersion();
            versionCombo.ItemsSource = versions;//绑定数据源
            //自动找java
            List<string> javaList = new List<string>();
            javaList.Add(tools.GetJavaPath());
            foreach (var i in microsoftAPIs.GetJavaList())
            {
                javaList.Add(i.Path.ToString());
            }
            javaCombo.ItemsSource = javaList;
            //初始选择
            versionCombo.SelectedItem = versionCombo.Items[0];
            javaCombo.SelectedItem = javaCombo.Items[0];
            MemoryTextbox.Text = setting.Ram;


        }
        public MainWindow()
        {
            InitializeComponent();
            LauncehrInitialization();
            ServicePointManager.DefaultConnectionLimit = 512;
            
        }
        #region
        //public void CompleteFile()
        //{
        //    try
        //    {
        //        tools.DownloadSourceInitialization(DownloadSource.bmclapiSource);
        //        var v = tools.GetMissingFile(versionCombo.SelectedValue.ToString());
        //        Gac.DownLoadFile downLoadFile = new Gac.DownLoadFile();
        //        foreach(var i in v)
        //        {
        //            downLoadFile.AddDown(i.Url,i.path);//增加下载
        //        }
        //        downLoadFile.StartDown(0);
        //    }
        //    catch(Exception e)
        //    {
        //        MessageBoxX.Show(e.ToString(),"补全文件失败");
        //    }


        //}
        #endregion

        /// <summary>
        /// 游戏启动
        /// </summary>
        public async void GameStart()
        {
            try
            {
                if (startbutton.Content.ToString() == "启动")
                {
                    startbutton.Content = "补全文件ing";
                    //CompleteFile();
                    if (versionCombo.Text != string.Empty &&
                        javaCombo.Text != string.Empty &&
                        (LiXian.IDText.Text != string.Empty || ZhengBan.Email.Text != string.Empty && ZhengBan.password.Password != string.Empty &&
                        MemoryTextbox.Text != string.Empty))
                    {
                        switch (launchMode)
                        {
                            case 1:
                                startbutton.Content = "启动ing";
                                await game.StartGame(versionCombo.Text, javaCombo.Text, Convert.ToInt32(MemoryTextbox.Text), LiXian.IDText.Text);
                                break;
                            case 2:
                                startbutton.Content = "启动ing";
                                await game.StartGame(versionCombo.Text, javaCombo.Text, Convert.ToInt32(MemoryTextbox.Text), ZhengBan.Email.Text, ZhengBan.password.Password);
                                break;
                            case 3:
                                startbutton.Content = "微软登录验证ing";
                                microsoft_launcher.MicrosoftAPIs microsoftAPIs = new microsoft_launcher.MicrosoftAPIs();
                                var v = WeiRuan.wb.Source.ToString().Replace(microsoftAPIs.cutUri, string.Empty);
                                var t = Task.Run(() =>
                                {
                                    return microsoftAPIs.GetAccessTokenAsync(v, false).Result;
                                });
                                await t;
                                var v1 = microsoftAPIs.GetAllThings(t.Result.access_token, false);
                                startbutton.Content = "启动ing";
                                await game.StartGame(versionCombo.Text, javaCombo.Text, Convert.ToInt32(MemoryTextbox.Text), v1.name, v1.uuid, v1.mcToken, string.Empty, string.Empty);
                                break;
                        }
                    }
                    else
                    {
                        MessageBoxX.Show("信息未填完整", "错误");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBoxX.Show("启动失败" + e.Message, "错误");
            }
            finally
            {
                startbutton.Content = "启动";
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

        private void MemoryTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //数据保存通用步骤
            setting.Ram = MemoryTextbox.Text;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //json数据保存步骤3
            File.WriteAllText(settingPath, JsonConvert.SerializeObject(setting));
            using (RegistryKey key = Registry.LocalMachine)
            {
                using (RegistryKey software = key.CreateSubKey("software\\MclauncherSetting"))
                {
                    //注册表数据保存步骤4
                    software.SetValue("name", LiXian.IDText.Text);
                }
            }
        }
    }
}
