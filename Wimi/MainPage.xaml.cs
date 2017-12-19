﻿using System;
using System.Threading.Tasks;
using DSMusic;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
// ref. http://uwpcommunitytoolkit.readthedocs.io/en/master/

namespace Wimi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        const bool USE_FACERECOG = false; //chris: for test

        DispatcherTimer ClockTimer = new DispatcherTimer();
        Music mu = new Music();

        public MainPage()
        {
            this.InitializeComponent();
            BackgroundBrush.ImageSource = null; //chris: 이미지 액자로 활용시 BackgroundBrush에 이미지를 설정하면 된다.
            //GradientAnimation.Begin(); //chris: 음성인식되는중 배경을 쓸지도 몰라서 코드는 남겨둠.

            ClockTimer.Tick += ClockTimer_Tick;
            ClockTimer.Interval = new TimeSpan(0, 0, 2);
            ClockTimer.Start();
        }

        private void ClearPanel()
        {
            int count = VisualTreeHelper.GetChildrenCount(gridCommand);
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(gridCommand, i);

                if (child is Grid && !((Grid)child).Name.Equals("gridVoiceHelper"))
                {
                    child.Visibility = Visibility.Collapsed;
                }
            }

            //gridBus.Visibility = Visibility.Collapsed;
            //gridWeather.Visibility = Visibility.Collapsed;
            //gridNews.Visibility = Visibility.Collapsed;
            tbHello.Visibility = Visibility.Collapsed;
        }

        private void ShowTbHello(string ment)
        {
            ClearPanel();
            tbHello.Text = ment;
            tbHello.Visibility = Visibility.Visible;
        }

        private void ClockTimer_Tick(object sender, object e)
        {
            SetTime(); // tbClock.Text = DateTime.Now.ToString("HH:mm");
        }

        public void SetTime()
        {
            DateTime dt = DateTime.Now;
            tbDateTime.Text = dt.ToString("dddd, MMM d").ToUpper() + "th";
            tbTime.Text = dt.ToString("h:mm");
            tbAmPm.Text = dt.ToString("tt");
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            await gridVoiceHelper.Offset(0, -400, 0, 0, EasingType.Linear).StartAsync();

            InitVoiceCommand();
            initMusicList();
            initSynthesizer();
            await InitializeRecognizer();

            HueInit();

            bool isInit = await Webcam.InitializeCameraAsync();

            if (isInit)
            {
                captureElement.Source = Webcam.mediaCapture;
                await Webcam.StartCameraPreview();

                if (USE_FACERECOG)
                {
                    InitFaceRec();
                    await face.InitListAsync();
                }
            }
            else
            {
                tbCameraStat.Text = "연결되어있는 웹캠이 존재하지 않습니다!";
            }
            
            GetBusInfo();
#if PC_MODE
            Getschedule();
#endif
            await GetForecastInfo();

            await GetNewsInfo();
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            await Webcam.StopCameraPreview();
            Webcam.mediaCapture.Dispose();

            CleanSpeechRecognizer();
            RemoveConstraints();
        }
    }
}
