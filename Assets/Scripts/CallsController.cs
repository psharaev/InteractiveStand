﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

namespace Stand
{
    public class CallsController : Singleton<CallsController>
    {
        private TimeSpan TimeToCall;
        private bool TimeSet = false;

        void FixedUpdate()
        {
            if (TimeSet)
            {
                if (DateTime.Now.TimeOfDay >= TimeToCall)
                {
                    Call();
                    TimeSet = false;
                }
            }
            else
            {
                string[] Borders = BordersCalls();
                if (Borders[1] != "--:--")
                {
                    string[] s = Borders[1].Split(':');
                    TimeToCall = new TimeSpan(int.Parse(s[0]), int.Parse(s[1]),0);
                    TimeSet = true;
                    Loger.Log("звонок", $"время звонка установлено на:{TimeToCall}");
                }
            }
        }

        public string[] BordersCalls()
        {
            int day = DateTime.Now.DayOfWeek.Normalising();
            int IndexInTable = 0;
            switch (day)
            {
                case 1:
                    IndexInTable = 1;
                    break;
                case 5:
                    IndexInTable = 2;
                    break;
                case 6:
                    return new string[] { "--:--", "--:--" };
                default:
                    IndexInTable = 0;
                    break;
            }

            List<string> Times = new List<string>();
            char[] deaf = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '—', ':' };
            int i = 0;
            foreach (string[] s in Data.Instance.CallsMatrix)
            {
                string str = "";
                foreach (char c in s[IndexInTable])
                {
                    if (deaf.Select(x => x).Contains(c))
                        str += c;
                }
                string[] calls = str.Split(new char[] { '-', '—' });
                if (calls.Length == 2 && i > 0)
                {
                    Times.Add(calls[0]);
                    Times.Add(calls[1]);
                }
                i++;
            }

            string time = DateTime.Now.ToString("HH:mm");
            string[] result = { "--:--", "--:--" };
            if (string.CompareOrdinal(Times[0], time) > 0)
            {
                result[1] = Times[0];
                return result;
            }
            if (string.CompareOrdinal(Times[Times.Count - 1], time) <= 0)
            {
                result[0] = Times[Times.Count - 1];
                return result;
            }
            for (i = 0; i < Times.Count; i++)
            {
                if (string.CompareOrdinal(Times[i], time) > 0)
                {
                    continue;
                }
                else
                {
                    result[0] = Times[i];
                    result[1] = Times[i + 1];
                }
            }
            return result;
        }

        public int[] AttitudeCalls()
        {
            string[] res = BordersCalls();
            string LeftBorder = res[0];
            string RightBorder = res[1];

            int Difference = 0;
            int TimeLeft = 0;

            TimeSpan RightBorderTS = new TimeSpan();
            TimeSpan LeftBorderTS = new TimeSpan();

            if (RightBorder != "--:--")
            {
                string[] s = RightBorder.Split(':');
                RightBorderTS = new TimeSpan(int.Parse(s[0]), int.Parse(s[1]), 0);
                s = DateTime.Now.ToString("HH:mm:ss").Split(':');
                TimeSpan TimeCurrentTS = new TimeSpan(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));
                TimeLeft = (int)(RightBorderTS - TimeCurrentTS).TotalSeconds;
            }
            if (LeftBorder != "--:--")
            {
                string[] s = LeftBorder.Split(':');
                LeftBorderTS = new TimeSpan(int.Parse(s[0]), int.Parse(s[1]), 0);
            }

            if (RightBorder != "--:--" && LeftBorder != "--:--")
                Difference = (int)(RightBorderTS - LeftBorderTS).TotalSeconds;

            return new int[] { Difference, TimeLeft };
        }

        public int WhatNow() // <0 номер перемены; =0 фиг его знает; >0 номер урока
        {
            int day = DateTime.Now.DayOfWeek.Normalising();
            int IndexInTable = 0;
            switch (day)
            {
                case 1:
                    IndexInTable = 1;
                    break;
                case 5:
                    IndexInTable = 2;
                    break;
                case 6:
                    return 0;
                default:
                    IndexInTable = 0;
                    break;
            }

            List<string> Times = new List<string>();
            char[] deaf = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '—', ':' };
            int i = 0;
            foreach (string[] s in Data.Instance.CallsMatrix)
            {
                string str = "";
                foreach (char c in s[IndexInTable])
                {
                    if (deaf.Select(x => x).Contains(c))
                        str += c;
                }
                string[] calls = str.Split(new char[] { '-', '—' });
                if (calls.Length == 2 && i > 0)
                {
                    Times.Add(calls[0]);
                    Times.Add(calls[1]);
                }
                i++;
            }

            string time = DateTime.Now.ToString("HH:mm");
            int result = 0;
            if (string.CompareOrdinal(Times[0], time) > 0)
            {
                return 0;
            }
            for (i = 0; i < Times.Count; i++)
            {
                if (string.CompareOrdinal(Times[i], time) > 0)
                {
                    continue;
                }
                else
                {
                    result = (i % 2 == 0 ? 1 : -1) * (i % 2 == 0 ? (i + 2) / 2 : (i + 1) / 2); // и пускай чёрт ногу сломит в этой формуле
                }
            }
            return result;
        }

        public void Call(int t = 2)
        {
            UnityWebRequest uwr = UnityWebRequest.Get("http://192.168.1.211/Call?c=2000");
            uwr.SendWebRequest();
            Loger.Log("звонок", "включен звонок продолжительность:" + t);
        }
    }
}