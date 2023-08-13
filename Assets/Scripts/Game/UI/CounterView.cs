using System;
using TMPro;
using UnityEngine;

namespace Source.Scripts.UI
{
    public class CounterView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetText(int value)
        {
            text.text = $"{value}";
        }
    }
}