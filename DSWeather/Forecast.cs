﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace DSWeather
{

    public partial class Weather
    {

        public async Task<List<ForecastInfo>> GetForecastInfoByCountAsync(int count = 5)
        {
            List<ForecastInfo> lForcastInfo = new List<ForecastInfo>();
            XmlDocument docX = new XmlDocument(); // XmlDocument 생성
            try
            {
                //docX.Load("http://www.kma.go.kr/wid/queryDFS.jsp?gridx=86&gridy=86"); // url로 xml 파일 로드
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri("http://www.kma.go.kr/wid/queryDFS.jsp?gridx=86&gridy=86"));
                var jsonStream = await response.Content.ReadAsStreamAsync();
                docX.Load(jsonStream);
            }

            catch
            {
                Console.WriteLine("Error");
                return null;
            }

            XmlNodeList hourList = docX.GetElementsByTagName("hour"); // 태그 이름으로 노드 리스트 저장
            XmlNodeList tempList = docX.GetElementsByTagName("temp");
            XmlNodeList weatherList = docX.GetElementsByTagName("wfKor");

            for(int i = 0; i < count; i++)
            {
                ForecastInfo info = new ForecastInfo();
                info.hour = int.Parse(hourList[i].InnerText);
                info.hourStr = info.hour + "시";
                info.temperture = double.Parse(tempList[i].InnerText);
                info.stat = weatherList[i].InnerText;
                info.statSymbol = GetSymbol(info.stat);
                lForcastInfo.Add(info);
            }

            return lForcastInfo;
        }

        public string GetSymbol(string stat)
        {
            string sky = "";
            switch (stat)
            {
                case "SKY_O01":
                    sky = "\uE284";
                    //맑음
                    break;
                case "구름 조금":
                    sky = "\uE286";
                    //구름 조금
                    break;
                case "SKY_O03":
                    sky = "\uE285";
                    //구름 많음
                    break;
                case "SKY_O04":
                    sky = "\uE288";
                    //비
                    break;
                case "SKY_O05":
                    sky = "\uE28A";
                    //눈
                    break;
                case "SKY_A11":
                    sky = "\uE289";
                    //낙뢰
                    break;
            }
            return sky;
        }
    }

    public class ForecastInfo
    {
        public int hour { get; set; }
        public string hourStr { get; set; }
        public double temperture { get; set; }
        public string stat { get; set; }
        public string statSymbol { get; set; }

        public ForecastInfo() {}
        public ForecastInfo(int hour, double temperture, string stat)
        {
            this.hour = hour;
            this.temperture = temperture;
            this.stat = stat;
        }
    }
}
