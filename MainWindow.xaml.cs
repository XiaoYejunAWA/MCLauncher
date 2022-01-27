using Gac;
using Microsoft.Win32;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using SquareMinecraftLauncher;
using SquareMinecraftLauncher.Core.OAuth;
using SquareMinecraftLauncher.Minecraft;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MCLauncher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        #region UI

        private LoginUI.skinlogin skinlogin = new LoginUI.skinlogin();
        private LoginUI.LiXian LiXian = new LoginUI.LiXian();
        private LoginUI.ZhengBan ZhengBan = new LoginUI.ZhengBan();

        #endregion UI

        #region 微软登录

        private MicrosoftLogin microsoftLogin = new MicrosoftLogin();
        private Xbox XboxLogin = new Xbox();
        private string Minecraft_Token;
        private MinecraftLoginToken Minecraft;

        #endregion 微软登录

        public int launchMode = 1;
        private Game game = new Game();
        private Tools tools = new Tools();
        private MinecraftDownload minecraftDownload = new MinecraftDownload();
        private string settingPath = @"Mclauncher.json";
        private Setting setting = new Setting();
        private RegisterSetting registerSetting = new RegisterSetting();

        public class Setting
        {
            //json数据保存步骤1
            public string Ram = "1024";
        }

        public class RegisterSetting
        {
            //注册表数据保存步骤1
            public string name = "小叶君";
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public async void LauncehrInitialization()
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
            bool isFirst = true;
            using (RegistryKey key1 = Registry.LocalMachine.OpenSubKey("SOFTWARE"))
            {
                foreach (var i in key1.GetSubKeyNames())
                {
                    if (i == "MclauncherSetting")
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
                        software.SetValue("name", registerSetting.name);
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
            javaCombo.ItemsSource = tools.GetJavaPath();
            //初始选择
            if (versionCombo.Items.Count != 0)
                versionCombo.SelectedItem = versionCombo.Items[0];
            if (javaCombo.Items.Count != 0)
                javaCombo.SelectedItem = javaCombo.Items[0];
            MemoryTextbox.Text = setting.Ram;

            var v = await tools.GetMCVersionList();
            MCVersionDataGrid.ItemsSource = v;
        }

        public MainWindow()
        {
            InitializeComponent();
            LauncehrInitialization();
            ServicePointManager.DefaultConnectionLimit = 512;
        }

        public async void CompleteFile()
        {
            GacDownload gacDownload = new GacDownload(18, tools.GetMissingFile(versionCombo.Text));
            AssetDownload assetDownload = new AssetDownload();//asset下载类
            gacDownload.StartDownload();
            await assetDownload.BuildAssetDownload(6, "1.8.1");//构建下载
            assetDownload.DownloadProgressChanged += AssetDownload_DownloadProgressChanged;
        }

        private void AssetDownload_DownloadProgressChanged(AssetDownload.DownloadIntermation Log)
        {
            MessageBox.Show(Log.Progress.ToString());
        }

        /// <summary>
        /// 游戏启动
        /// </summary>
        public async void GameStart()
        {
            try
            {
                CompleteFile();
                if (startbutton.Content.ToString() == "启动")
                {
                    if (versionCombo.Text != string.Empty &&
                        javaCombo.Text != string.Empty &&
                        (LiXian.IDText.Text != string.Empty || ZhengBan.Email.Text != string.Empty && ZhengBan.password.Password != string.Empty &&
                        MemoryTextbox.Text != string.Empty))
                    {
                        switch (launchMode)
                        {
                            case 1:
                                startbutton.Content = "启动ing";
                                await game.StartGame(versionCombo.Text, javaCombo.SelectedValue.ToString(), Convert.ToInt32(MemoryTextbox.Text), LiXian.IDText.Text);
                                break;

                            case 2:
                                startbutton.Content = "启动ing";
                                await game.StartGame(versionCombo.Text, javaCombo.SelectedValue.ToString(), Convert.ToInt32(MemoryTextbox.Text), ZhengBan.Email.Text, ZhengBan.password.Password);
                                break;

                            case 3:

                                startbutton.Content = "启动ing";

                                await game.StartGame(versionCombo.Text, javaCombo.SelectedValue.ToString(), Convert.ToInt32(MemoryTextbox.Text), Minecraft.name, Minecraft.uuid, Minecraft_Token, string.Empty, string.Empty);
                                break;

                            case 4:
                                await game.StartGame(versionCombo.Text, javaCombo.SelectedValue.ToString(), Convert.ToInt32(MemoryTextbox.Text), skinlogin.skinskin.Player.Text, skinlogin.skinskin.Player.SelectedValue.ToString(), skinlogin.skin.accessToken, string.Empty, string.Empty);
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
        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Minecraft_Token = new MinecraftLogin().GetToken(XboxLogin.XSTSLogin(XboxLogin.GetToken(microsoftLogin.GetToken(await microsoftLogin.Login(true)).access_token)));
            MinecraftLogin minecraftLogin = new MinecraftLogin();
            Minecraft = minecraftLogin.GetMincraftuuid(Minecraft_Token);
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ContentControl1.Content = new Frame
            {
                Content = skinlogin
            };
            launchMode = 4;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            List<MCDownload> ms = new List<MCDownload>();
            ms.Add(minecraftDownload.MCjarDownload(((MCVersionList)MCVersionDataGrid.SelectedItem).id));
            ms.Add(minecraftDownload.MCjsonDownload(((MCVersionList)MCVersionDataGrid.SelectedItem).id));
            GacDownload gacDownload = new GacDownload(18, ms.ToArray());
            gacDownload.StartDownload();
        }

        public class GacDownload
        {
            public float DownloadPercent { internal set; get; } = 1;
            private Thread[] Threads = new Thread[0];
            private SquareMinecraftLauncher.Minecraft.MCDownload[] download = new SquareMinecraftLauncher.Minecraft.MCDownload[0];
            private int EndDownload = 0;

            public GacDownload(int thread, SquareMinecraftLauncher.Minecraft.MCDownload[] download)
            {
                Threads = new Thread[thread];
                this.download = download;
            }

            public GacDownload(SquareMinecraftLauncher.Minecraft.MCDownload[] download)
            {
                Threads = new Thread[3];
                this.download = download;
            }

            private int ADindex = 0;

            private SquareMinecraftLauncher.Minecraft.MCDownload AssignedDownload()
            {
                if (ADindex == download.Length) return null;
                ADindex++;
                return download[ADindex - 1];
            }

            public void StartDownload()
            {
                for (int i = 0; i < Threads.Length; i++)
                {
                    Threads[i] = new Thread(DownloadProgress);
                    Threads[i].IsBackground = true;
                    Threads[i].Start();//启动线程
                }
            }

            private async void DownloadProgress()
            {
                List<FileDownloader> files = new List<FileDownloader>();
                for (int i = 0; i < 3; i++)
                {
                    SquareMinecraftLauncher.Minecraft.MCDownload download = AssignedDownload();//分配下载任务
                    try
                    {
                        if (download != null)
                        {
                            FileDownloader fileDownloader = new FileDownloader(download.Url, download.path.Replace(System.IO.Path.GetFileName(download.path), ""), System.IO.Path.GetFileName(download.path));//增加下载
                            fileDownloader.download(null);
                            files.Add(fileDownloader);
                        }
                    }
                    catch (Exception ex)//当出现下载失败时，忽略该文件
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                if (files.Count == 0) return;
                await Task.Factory.StartNew(() =>
                {
                    while (true)//循环检测当前线程files.Count个下载任务是否下载完毕
                    {
                        int end = 0;
                        for (int i = 0; i < files.Count; i++)
                        {
                            if (files[i].download(null) == files[i].getFileSize())
                            {
                                end++;
                            }
                        }
                        Console.WriteLine(EndDownload);

                        if (end == files.Count)//完成则递归当前函数
                        {
                            EndDownload += files.Count;
                            DownloadProgress();//递归
                            return;
                        }
                        Thread.Sleep(1000);
                    }
                });
            }

            public bool GetEndDownload()
            {
                return EndDownload == download.Length ? true : false;
            }
        }
    }
}