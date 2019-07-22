﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Stand
{
    public class UIController : MonoBehaviour
    {
        [Header("Colors")]
        public Color LowGray;
        public Color Gray;
        public Color VeryGray;

        [Header("BlindMode")]
        public float AnimTimeBlind = 1f;
        public Camera cam;
        public Material GrayScale;
        public AnimationCurve ScaleCam;
        public AnimationCurve GrayScaleAnim;

        private float NoBlind = 5f;
        private float YesBlind = 4.3f;
        private float StartAnimBlind = -10f;

        [Header("Mains")]
        public IWindow MainStaticSchedulesWindow;
        public IWindow MainChangesSchedulesWindow;
        private IWindow MainCurrentWindow;

        [Header("Calls")]
        public IWindow CallsWindow;

        [Header("Lessons")]
        public IWindow LessonsWindow;

        [Header("Lessons_Class")]
        public IWindow Lessons_ClassWindow;

        [Header("Extra")]
        public IWindow ExtraWindow;

        [Header("ChangeCallsWindow")]
        public IWindow ChangeCallsWindow;

        [Header("ChangeLessonsWindow")]
        public IWindow ChangeLessonsWindow;

        [Header("TimePanel")]
        public IWindow TimePanelWindow;

        [Header("TimeLine")]
        public IWindow TimeLineWindow;

        bool BlindMode = false;

        void Start()
        {
            MainChangesSchedulesWindow.SetActive(false);
            MainStaticSchedulesWindow.SetActive(false);
            if (Data.Instance.CurrentManifest.SupportChangesSchedules)
                MainCurrentWindow = MainChangesSchedulesWindow;
            else
                MainCurrentWindow = MainStaticSchedulesWindow;

            HideAll();
            MainCurrentWindow.SetActive(true);
            TimePanelWindow.PrimaryFill();
            TimePanelWindow.SetActive(true);
            TimeLineWindow.SetActive(true);
            LessonsWindow.PrimaryFill();
            CallsWindow.PrimaryFill();
            GrayScale.SetFloat("_EffectAmount", 0f);
        }

        void FixedUpdate()
        {
            TimePanelWindow.Fill();
            TimeLineWindow.Fill();

            if (StartAnimBlind + AnimTimeBlind > Time.time)
            {
                float t = (Time.time - StartAnimBlind) / AnimTimeBlind;

                if (t > 0.8f)
                    ApplicationController.Instance.Clear = false;
                else
                    ApplicationController.Instance.Clear = true;

                if (BlindMode)
                {
                    cam.orthographicSize = Mathf.Lerp(NoBlind, YesBlind, ScaleCam.Evaluate(t));
                    GrayScale.SetFloat("_EffectAmount", GrayScaleAnim.Evaluate(t));

                }
                else
                {
                    cam.orthographicSize = Mathf.Lerp(YesBlind, NoBlind, ScaleCam.Evaluate(t));
                    GrayScale.SetFloat("_EffectAmount", 1 - GrayScaleAnim.Evaluate(t));
                }
            }
        }

        public void Lessons_ClassWindow_ChooseClass(string ClassID)
        {
            Lessons_ClassWindow.SetActive(true);
            LessonsWindow.SetActive(false);
            TimePanelWindow.Merge(true);
            TimePanelWindow.SetActive(true);
            Lessons_ClassWindow.ChooseClass(ClassID);
        }

        public void MergeTimePanel(bool Status)
        {
            TimePanelWindow.Merge(Status);
        }

        public void OpenCallsWindow(bool Open)
        {
            HideAll();
            CallsWindow.SetActive(Open);
            TimePanelWindow.SetActive(true);
            MainCurrentWindow.SetActive(!Open);
            TimePanelWindow.Merge(Open);
            Loger.Log("Окно звонков", Open ? "открыли" : "закрыли");
        }

        public void OpenLessonsWindow(bool Open)
        {
            HideAll();
            LessonsWindow.SetActive(Open);
            TimePanelWindow.SetActive(!Open);
            MainCurrentWindow.SetActive(!Open);
            Loger.Log("Окно уроков", Open ? "открыли" : "закрыли");
        }

        public void OpenLessons_ClassWindow(bool Open)
        {
            HideAll();
            LessonsWindow.SetActive(true);
            TimePanelWindow.SetActive(false);
            MainCurrentWindow.SetActive(false);
            Loger.Log("Окно занятий", Open ? "открыли" : "закрыли");
        }

        public void OpenExtraWindow(bool Open)
        {
            HideAll();
            ExtraWindow.PrimaryFill();
            TimePanelWindow.Merge(Open);

            MainCurrentWindow.SetActive(!Open);
            ExtraWindow.SetActive(Open);
            TimePanelWindow.SetActive(true);
            Loger.Log("Окно доп.секций", Open ? "открыли" : "закрыли");
        }

        public void OpenChangeCallsWindow(bool Open)
        {
            HideAll();
            ChangeCallsWindow.PrimaryFill();
            TimePanelWindow.Merge(Open);

            MainCurrentWindow.SetActive(!Open);
            ChangeCallsWindow.SetActive(Open);
            TimePanelWindow.SetActive(true);
            Loger.Log("Окно изменений звонков", Open ? "открыли" : "закрыли");
        }
        public void OpenChangeLessonsWindow(bool Open)
        {
            HideAll();
            ChangeLessonsWindow.PrimaryFill();
            TimePanelWindow.Merge(Open);

            MainCurrentWindow.SetActive(!Open);
            ChangeLessonsWindow.SetActive(Open);
            TimePanelWindow.SetActive(true);
            Loger.Log("Окно изменений звонков", Open ? "открыли" : "закрыли");
        }
        void HideAll()
        {
            MainCurrentWindow.SetActive(false);
            CallsWindow.SetActive(false);
            LessonsWindow.SetActive(false);
            ExtraWindow.SetActive(false);
            TimePanelWindow.SetActive(false);
            Lessons_ClassWindow.SetActive(false);
            ChangeCallsWindow.SetActive(false);
            TimePanelWindow.Merge(false);
        }

        public void OnClickBlindMode()
        {
            BlindMode = !BlindMode;
            Loger.Log("BlindMode", BlindMode ? "включён" : "выключен");
            StartAnimBlind = Time.time;
        }
    }
}