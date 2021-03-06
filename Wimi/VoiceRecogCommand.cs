﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.Media.SpeechRecognition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using DSBingSpeech;
using System.Threading;
using Windows.UI.Xaml;
using System.Diagnostics;

namespace Wimi
{
    public partial class MainPage : Page
    {
        //제약조건
        private SpeechRecognitionListConstraint helloConstraint;
        private SpeechRecognitionListConstraint ByeConstraint;
#if false
        #region 영어제약조건
        private SpeechRecognitionListConstraint TellWeatherConstraint;
        private SpeechRecognitionListConstraint TestConstraint;
        private SpeechRecognitionListConstraint PlayMusicConstraint;
        private SpeechRecognitionListConstraint StopMusicConstraint;
        private SpeechRecognitionListConstraint PauseMusicConstraint;
        private SpeechRecognitionListConstraint ShowNewsConstraint;
        private SpeechRecognitionListConstraint ShowBusConstraint;
        private SpeechRecognitionListConstraint FullScreenConstraint;
        private SpeechRecognitionListConstraint HaloConstraint;
        #endregion
#endif
        AudioRecorder audioRecorder = new AudioRecorder();

#if false
        #region 조명
        private SpeechRecognitionListConstraint TurnOnLightConstraint;
        private SpeechRecognitionListConstraint TurnOffLightConstraint;
        private SpeechRecognitionListConstraint ChangeLightModeOn;
        private SpeechRecognitionListConstraint ChangeLightModeOff;

        private SpeechRecognitionListConstraint RedColorLightConstraint;
        private SpeechRecognitionListConstraint BrownColorLightConstraint;
        private SpeechRecognitionListConstraint YellowColorLightConstraint;
        private SpeechRecognitionListConstraint GreenColorLightConstraint;
        private SpeechRecognitionListConstraint BlueColorLightConstraint;
        private SpeechRecognitionListConstraint PinkColorLightConstraint;
        private SpeechRecognitionListConstraint PurpleColorLightConstraint;
        private SpeechRecognitionListConstraint WhiteColorLightConstraint;
        #endregion
#endif

        private bool _isWimiRecording = false;

        private DispatcherTimer listenTimer = new DispatcherTimer();
        public DateTime TimerStart { get; set; }

        private DispatcherTimer expireTimer = new DispatcherTimer();
        public DateTime ExpireTimerStart { get; set; }

        private const int INTERVAL = 3;
        private const int ExpireINTERVAL = 10;//명령없을 시 만료되기위한 시간(초)

        private void InitVoiceCommand()
        {
            listenTimer.Interval = new TimeSpan(0, 0, 1);
            listenTimer.Tick += ListenTimer_Tick;
        }
        private void InitExpireCommand()
        {
            expireTimer.Interval = new TimeSpan(0, 0, 1);
            expireTimer.Tick += ExpireTimer_Tick;
        }

        private async void VoiceCommandAsync(string heard, string tag, string confidence)
        {
            resultTextBlock.Text = string.Format("Heard: {0}, Tag: {1}, Confidence: {2}", heard, tag, confidence);
            
            if (!string.IsNullOrEmpty(tag))
            {
                if (tag == "Wimi" && !_isWimiRecording)
                {
                    if (audioRecorder.GetStatues())
                    {
                        expireTimer.Stop();
                        double pVolume = mediaElement.Volume;
                        _isWimiRecording = true;
                        mediaElement.Volume = mediaElement.Volume * 0.25;
                        ClearPanel();

                        if (mediaElement.CurrentState == MediaElementState.Playing && mediaElement.IsFullWindow == true)
                        {
                            mediaElement.IsFullWindow = false;
                        }
                        SetVoice("wimi.mp3", true);
                        gridCommand.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        gridConentRoot.Blur(20, 800).Start();
                        tbRecog.Text = "Listening..." + INTERVAL;

                        await gridCommand.Offset(0, 0, 400, 0, EasingType.Linear).StartAsync();
                        VoiceRecogEffect.Play();

                        string result = await StartRecordingAsync();
                        tbRecog.Text = result;//음성인식 결과 출력 디버그용.
                        mediaElement.Volume = pVolume;
                        bool Ordered = await CommandByVoiceAsync(result);
                        _isWimiRecording = false;
                        //if (Ordered)//명령 수행이 없을 시에 10초후 창을 닫음.
                        {
                            //StartExpiring();
                        }
                    }
                }
                else if(tag == "Bye" && gridCommand.Visibility == Visibility.Visible)
                {
                    SetVoice("wimi_close.mp3", true);
                    WimiClose();
                }
#if false
                #region 영어인식파트
                else
                {
                    if (gridCommand.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                    {
                        return;
                    }
                    SetVoice("wimi_succeed.mp3", true);

                    ClearPanel();
                    switch (tag)
                    {
                        //case "Wimi":                        
                        //    break;
                        case "Halo":
                            SetVoice("안녕하세요.");
                            break;
                        case "Weather":
                            ShowForecast();
                            TellmeWeatherAsync();
                            break;
                        case "PauseMusic":
                            PauseMusic();
                            break;
                        case "StopMusic":
                            StopMusic();
                            break;
                        case "PlayMusic":
                            await PlayMusic();
                            break;
                        case "FullScreen":
                            SetFullScreen();
                            break;
                        case "News":
                            ShowNews();
                            break;
                        case "Bus":
                            ShowBus();
                            break;
                        case "LightModeOn":
                            HueAtrBool = await HueControl.HueEffect(1);
                            break;
                        case "LightModeOff":
                            HueAtrBool = await HueControl.HueEffect(0);
                            break;
                        case "TurnOn":
                            HueAtrBool = await HueControl.HueLightOn();
                            break;
                        case "TurnOff":
                            HueAtrBool = await HueControl.HueLightOff();
                            break;
                        case "RedColor":
                            HueAtrBool = await HueControl.SetColor("red");
                            break;
                        case "BrownColor":
                            HueAtrBool = await HueControl.SetColor("brown");
                            break;
                        case "YellowColor":
                            HueAtrBool = await HueControl.SetColor("yellow");
                            break;
                        case "GreenColor":
                            HueAtrBool = await HueControl.SetColor("green");
                            break;
                        case "BlueColor":
                            HueAtrBool = await HueControl.SetColor("blue");
                            break;
                        case "PurpleColor":
                            HueAtrBool = await HueControl.SetColor("purple");
                            break;
                        case "PinkColor":
                            HueAtrBool = await HueControl.SetColor("pink");
                            break;
                        case "WhiteColor":
                            HueAtrBool = await HueControl.SetColor("white");
                            break;
                        default:
                            {
                                resultTextBlock.Text = "등록된 명령어가 아닙니다";
                                break;
                            }
                    }
                }
                #endregion
#endif
            }
        }

        private async void WimiClose()
        {
            expireTimer.Stop();
            //ClearPanel();
            tbHello.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                //chris: idle화면으로 나가더라도 계속 플레이하고 싶으면 주석처리하고, idle에서 stop명령 수행가능하도록 처리하면 된다.
                //StopMusic();
            }

            VoiceRecogEffect.Stop();
            await gridCommand.Offset(0, -(float)Window.Current.Bounds.Height, 400, 0, EasingType.Linear).StartAsync();
            gridCommand.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            gridConentRoot.Blur(0, 800).Start();
        }

        private void StartExpiring()
        {
            ExpireTimerStart = DateTime.Now;
            expireTimer.Start();
        }

        private void ExpireTimer_Tick(object sender, object e)
        {
            var currentValue = DateTime.Now - this.ExpireTimerStart;
            var remainSec = ExpireINTERVAL - currentValue.Seconds;
            if (remainSec <= -1)
            {
                expireTimer.Stop();
                SetVoice("wimi_close.mp3", true);
                WimiClose();
                return;
            }
        }

        private async Task<string> StartRecordingAsync()
        {
            TimerStart = DateTime.Now;
            listenTimer.Start();

            audioRecorder.StartRecord();//음성 녹화 시작.
            await Task.Delay((INTERVAL + 1) * 1000);
            string result = await audioRecorder.StopRecord();
            result = ReviseSentence(result);
            return result;
        }

        private void ListenTimer_Tick(object sender, object e)
        {
            var currentValue = DateTime.Now - this.TimerStart;
            var remainSec = INTERVAL - currentValue.Seconds;
            if(remainSec <= -1)
            {
                listenTimer.Stop();
                tbRecog.Text = "Processing...";
                return;
            }
            tbRecog.Text = "Listening..." + remainSec;
        }

        private string ReviseSentence(string str)
        {
            Dictionary<string, string> replaceDictionary = new Dictionary<string, string>()
            {
                { "급십", "급식" },
                { "육십", "급식" },
                { "특식", "급식" },
                { "자 께", "작게" },
                { "밖에", "작게" },
                { "잡게", "작게" },
                { "너 에", "노래" },
                { "모레", "노래" },
                { "몸", "멈" },
                { "들어", "틀어" },
                { "소비", "소리" },
                { "하면", "화면" },
                { "상금", "잠금" },
                { "성금", "잠금" }
            };

            foreach(KeyValuePair<string, string> pair in replaceDictionary)
            {
                if (str.Contains(pair.Key))
                {
                    Debug.WriteLine(pair.Key + " Revised!");
                }
                str = str.Replace(pair.Key, pair.Value);
            }
            return str;
        }

        private async Task<bool> CommandByVoiceAsync(string str)
        {
            if (gridCommand.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                return true;
            }
            SetVoice("wimi_succeed.mp3", true);

            ClearPanel();
            //명령어 추가 필요 아마 키워드를 통한 명령을 하지싶음.
            if (str.Contains("날씨"))             //날씨알려줘
            {
                await ShowForecastAsync();
                TellmeWeatherAsync();
            }
            else if (str.Contains("버스"))        //버스 정보
            {
                ShowBus();
            }
            else if (str.Contains("뉴스"))        //뉴스 알려줘
            {
                ShowNews();
            }
            else if (str.Contains("음악") || str.Contains("노래"))
            {
                if (str.Contains("재생") || str.Contains("틀"))   //음악 틀어줘
                {
                    await PlayMusic();
                }
                else if (str.Contains("일시정지"))//음악 일시정지
                {
                    PauseMusic();
                }
                else if (str.Contains("정지") || str.Contains("멈") || str.Contains("꺼"))   //음악 멈춰
                {
                    StopMusic();
                }
                else if (str.Contains("크") || str.Contains("높"))
                {
                    SetMusicVolume(true);
                }
                else if (str.Contains("작") || str.Contains("낮"))
                {
                    SetMusicVolume(false);
                }
            }
            else if (str.Contains("소리") || str.Contains("볼륨"))
            {
                if (str.Contains("크") || str.Contains("높"))
                {
                    SetMusicVolume(true);
                }
                else if (str.Contains("작") || str.Contains("낮"))
                {
                    SetMusicVolume(false);
                }
            }
            else if (str.Contains("안녕"))        //안녕
            {
                string hello = "안녕하세요!";
                if (!string.IsNullOrEmpty(CurrentUser) && !CurrentUser.Equals("Guest"))
                {
                    hello += CurrentUser + "님!";
                }
                SetVoice(hello);
            }
#region 조명제어
            else if (str.Contains("불") || str.Contains("조명"))
            {

                if (!HueControl.IsInit())
                {
                    SetVoice("조명제어는 현재 지원하지 않는 기능입니다.");
                }
                else
                {
                    if (str.Contains("켜"))           //불켜
                    {
                        await HueControl.HueLightOn();
                    }
                    else if (str.Contains("꺼"))      //불꺼
                    {
                        await HueControl.HueLightOff();
                    }
                    else if (str.Contains("노란"))
                    {
                        await HueControl.SetColor("yellow");
                    }
                    else if (str.Contains("빨간"))
                    {
                        await HueControl.SetColor("red");
                    }
                    else if (str.Contains("갈색"))
                    {
                        await HueControl.SetColor("brown");
                    }
                    else if (str.Contains("녹색"))
                    {
                        await HueControl.SetColor("green");
                    }
                    else if (str.Contains("파란"))
                    {
                        await HueControl.SetColor("blue");
                    }
                    else if (str.Contains("보라"))
                    {
                        await HueControl.SetColor("purple");
                    }
                    else if (str.Contains("분홍") || str.Contains("핑크"))
                    {
                        await HueControl.SetColor("pink");
                    }
                    else if (str.Contains("파란"))
                    {
                        await HueControl.SetColor("blue");
                    }
                    else if (str.Contains("흰") || str.Contains("백색"))
                    {
                        await HueControl.SetColor("white");
                    }
                }
            }
            else if (str.Contains("형광"))
            {
                await HueControl.SetColor("white");
            }
#endregion
            else if (str.Contains("연구"))
            {
                ShowLocationAsync();
            }
            else if (str.Contains("명령"))
            {
                ShowHelp();
            }
            else if (str.Contains("급식"))
            {
                ShowMeal();
            }
            else if (str.Contains("전체화면"))
            {
                SetFullScreen();
            }
            else if (str.Contains("잠금") || str.Contains("잠궈"))
            {
                WimiClose();
                await LockScreenAsync();
            }
            else//명령을 찿지 못했을때.
            {
                return true;
            }
            return false;
        }

        public void AddConstraints()
        {
            //{"들을 내용1", "내용2"},"태그이름");
            helloConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Hello Wimi", "Hi Wimi" }, "Wimi");
            ByeConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Good bye" }, "Bye");
#if false
    #region 영어인식제약
            TellWeatherConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Today Weather"}, "Weather");
            TestConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "VoiceTest" }, "Test");
            PlayMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Play Music" }, "PlayMusic");
            StopMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Stop Music"}, "StopMusic");
            PauseMusicConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Pause Music"}, "PauseMusic");
            ShowNewsConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Today News", "Show Todays News" }, "News");
            ShowBusConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Where is Bus" }, "Bus");
            FullScreenConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Set FullScreen" }, "FullScreen");
            HaloConstraint = new SpeechRecognitionListConstraint(new List<String>() { "Hello", "Hi" }, "Halo");
#endregion
#endif

#if false
    #region 조명
            TurnOnLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "turn On the Light"}, "TurnOn");
            TurnOffLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "turn Off the Light"}, "TurnOff");
            ChangeLightModeOn = new SpeechRecognitionListConstraint(new List<string>()
            { "Change Light Mode On","Loop colors start"}, "LightModeOn");
            ChangeLightModeOff = new SpeechRecognitionListConstraint(new List<string>()
            { "Change Light Mode Off","Loop colors stop"}, "LightModeOff");
            RedColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Red","turn on Red"}, "RedColor");
            BrownColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Brown","turn on Brown"}, "BrownColor");
            YellowColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Yellow","turn on Yellow"}, "YellowColor");
            GreenColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Green","turn on Green"}, "GreenColor");
            BlueColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Blue","turn on Blue"}, "BlueColor");
            PinkColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Pink","turn on Pink"}, "PinkColor");
            PurpleColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color Purple","turn on Purple"}, "PurpleColor");
            WhiteColorLightConstraint = new SpeechRecognitionListConstraint(new List<string>()
            { "Change color White","turn on White"}, "WhiteColor");
#endregion
#endif
            speechRecognizer.Constraints.Add(helloConstraint);
            speechRecognizer.Constraints.Add(ByeConstraint);
#if false
    #region 영어인식제약추가
            speechRecognizer.Constraints.Add(TellWeatherConstraint);
            speechRecognizer.Constraints.Add(TestConstraint);
            speechRecognizer.Constraints.Add(PlayMusicConstraint);
            speechRecognizer.Constraints.Add(StopMusicConstraint);
            speechRecognizer.Constraints.Add(PauseMusicConstraint);
            speechRecognizer.Constraints.Add(FullScreenConstraint);
            speechRecognizer.Constraints.Add(ShowNewsConstraint);
            speechRecognizer.Constraints.Add(ShowBusConstraint);
            speechRecognizer.Constraints.Add(HaloConstraint);
#endregion
#endif

#if false
    #region 조명
            speechRecognizer.Constraints.Add(TurnOnLightConstraint);
            speechRecognizer.Constraints.Add(TurnOffLightConstraint);
            speechRecognizer.Constraints.Add(ChangeLightModeOn);
            speechRecognizer.Constraints.Add(ChangeLightModeOff);
            speechRecognizer.Constraints.Add(RedColorLightConstraint);
            speechRecognizer.Constraints.Add(YellowColorLightConstraint);
            speechRecognizer.Constraints.Add(BrownColorLightConstraint);
            speechRecognizer.Constraints.Add(GreenColorLightConstraint);
            speechRecognizer.Constraints.Add(BlueColorLightConstraint);
            speechRecognizer.Constraints.Add(PinkColorLightConstraint);
            speechRecognizer.Constraints.Add(PurpleColorLightConstraint);
            speechRecognizer.Constraints.Add(WhiteColorLightConstraint);
#endregion
#endif
        }

        public void RemoveConstraints()
        {
            speechRecognizer.Constraints.Remove(helloConstraint);
            speechRecognizer.Constraints.Remove(ByeConstraint);
#if false
    #region 영어인식제약삭제
            speechRecognizer.Constraints.Remove(TellWeatherConstraint);
            speechRecognizer.Constraints.Remove(TestConstraint);
            speechRecognizer.Constraints.Remove(PlayMusicConstraint);
            speechRecognizer.Constraints.Remove(StopMusicConstraint);
            speechRecognizer.Constraints.Remove(PauseMusicConstraint);
            speechRecognizer.Constraints.Remove(FullScreenConstraint);
            speechRecognizer.Constraints.Remove(ShowNewsConstraint);
            speechRecognizer.Constraints.Remove(ShowNewsConstraint);
            speechRecognizer.Constraints.Remove(HaloConstraint);
#endregion
#endif

#if false
    #region 조명
            speechRecognizer.Constraints.Remove(TurnOnLightConstraint);
            speechRecognizer.Constraints.Remove(TurnOffLightConstraint);
            speechRecognizer.Constraints.Remove(ChangeLightModeOn);
            speechRecognizer.Constraints.Remove(ChangeLightModeOff);
            speechRecognizer.Constraints.Remove(RedColorLightConstraint);
            speechRecognizer.Constraints.Remove(YellowColorLightConstraint);
            speechRecognizer.Constraints.Remove(BrownColorLightConstraint);
            speechRecognizer.Constraints.Remove(GreenColorLightConstraint);
            speechRecognizer.Constraints.Remove(BlueColorLightConstraint);
            speechRecognizer.Constraints.Remove(PinkColorLightConstraint);
            speechRecognizer.Constraints.Remove(PurpleColorLightConstraint);
            speechRecognizer.Constraints.Remove(WhiteColorLightConstraint);
#endregion
#endif
        }

    }
}
