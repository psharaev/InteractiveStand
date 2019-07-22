﻿using UnityEngine;
using UnityEngine.UI;

namespace Stand
{
    public class CallsWindow : IWindow
    {
        public Text Monday;
        public Text Tuesday;
        public Text Saturday;

        public override void PrimaryFill()
        {
            Monday.text = "";
            Tuesday.text = "";
            Saturday.text = "";

            for (int i = 0; i <= Data.Instance.CallsMatrix.LastRowNum; i++)
            {
                Monday.text += Data.Instance.CallsMatrix.GetCell(i, 0) + '\n';
                Tuesday.text += Data.Instance.CallsMatrix.GetCell(i, 1) + '\n';
                Saturday.text += Data.Instance.CallsMatrix.GetCell(i, 2) + '\n';
            }
        }

        public override void Refill() => PrimaryFill();
        public override void Fill() => PrimaryFill();
        public override void Fill(int id) => PrimaryFill();
        public override void Fill(GameObject gameObject) => PrimaryFill();
        public override void ChooseClass(string Class) => PrimaryFill();
        public override void ChooseDay(int id) => PrimaryFill();

        public override void Merge(bool Status) { }
    }
}