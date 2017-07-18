﻿using DSWeather;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Wimi
{
    public partial class MainPage : Page
    {
        DispatcherTimer TTSDispatcherTimer;

        private SpeechSynthesizer synthesizer;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        private MediaElement media;
        public Boolean TTSflag;


        private async void TellmeWeatherAsync()
        {
            Weather weather = new Weather();
            String WeatherResult="";
            SKWeather.Minutely nowWeather = new SKWeather.Minutely();
            nowWeather = await weather.GetCurrentWeatherAsync();
            
            WeatherResult += nowWeather.station.name +"의 오늘 날씨는 현재 "
                +nowWeather.temperature.tc + "도 습도 " + nowWeather.humidity + "퍼센트, ";
            //Debug.WriteLine(nowWeather.humidity);
            
            switch (nowWeather.sky.code)
            {
                case "SKY_A01": WeatherResult += "맑은 날씨입니다."; break;
                case "SKY_A02": WeatherResult += "구름 조금 낀 날씨입니다."; break;
                case "SKY_A03": WeatherResult += "구름많은 날씨입니다."; break;
                case "SKY_A04": WeatherResult += "구름이 많고 비가 내립니다."; break;
                case "SKY_A05": WeatherResult += "구름이 많고 눈이 내립니다."; break;
                case "SKY_A06": WeatherResult += "구름이 많고 비나 눈이 내립니다."; break;
                case "SKY_A07": WeatherResult += "흐린 날씨입니다."; break;
                case "SKY_A08": WeatherResult += "흐리고 비가 내립니다."; break;
                case "SKY_A09": WeatherResult += "흐리고 눈이 내립니다."; break;
                case "SKY_A10": WeatherResult += "흐리고 비나 눈이 내립니다."; break;
                case "SKY_A11": WeatherResult += "흐리고 낙뢰가 칠수 있습니다."; break;
                case "SKY_A12": WeatherResult += "뇌우를 동반한 비가 내립니다."; break;
                case "SKY_A13": WeatherResult += "뇌우를 동반한 눈이 내립니다."; break;
                case "SKY_A14": WeatherResult += "뇌우를 동반한 비나 눈이 내립니다."; break;
            }
            SetVoice(WeatherResult);


        }
        private void TTSDispatcherTimer_Tick(object sender, object e)
        {
            TTSflag = false;
            TTSDispatcherTimer.Stop();
        }

        
        
        private void initSynthesizer()
        {
            TTSDispatcherTimer = new DispatcherTimer();
            TTSDispatcherTimer.Tick += TTSDispatcherTimer_Tick;
            TTSDispatcherTimer.Interval = new TimeSpan(0, 0, 3);

            media = new MediaElement();
            media.Volume = 1;
            synthesizer = new SpeechSynthesizer();
            
            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");

            var voices = SpeechSynthesizer.AllVoices;

            // Get the currently selected voice.
            VoiceInformation currentVoice = synthesizer.Voice;
            
            foreach (VoiceInformation voice in voices.OrderBy(p => p.Language))
            {
                synthesizer.Voice = voice;
            }
        }

        
        public async void SetVoice(string str)
        {
            //return; //잡음 문제로 일단 아래코드를 처리하지 않는다.

            if (media.CurrentState.Equals(MediaElementState.Playing))
            {
                media.Stop();
            }
            else
            {
                string text = str;
                if (!String.IsNullOrEmpty(text))
                {
                    try
                    {
                        // Create a stream from the text. This will be played using a media element.
                        SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(text);

                        // Set the source and start playing the synthesized audio stream.
                        media.AutoPlay = true;
                        media.SetSource(synthesisStream, synthesisStream.ContentType);

                        TTSDispatcherTimer.Start();
                        TTSflag = true;
                        media.Play();
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                    }
                    catch (Exception)
                    {
                        media.AutoPlay = false;
                    }
                }
            }
        }
    }
}
