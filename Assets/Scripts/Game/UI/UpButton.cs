using System;
using Game.Component;
using Source.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UpButton : MonoBehaviour
    {
        [SerializeField] private CounterView costCounter;
        [SerializeField] private TextMeshProUGUI fromText;
        [SerializeField] private TextMeshProUGUI toText;
        public Button button;
        
      
        private void Awake()
        {
            button = GetComponent<Button>();
        }


        public void SetupButton(float from, float to, int cost,bool interactable)
        {
            fromText.text = $"{from}";
            toText.text = $"{to}";
            costCounter.SetText(cost);
            button.interactable = interactable;
        }

        public void SetupButtonMax()
        {
            fromText.text = $"";
            toText.text = $"";
            costCounter.gameObject.SetActive(false);
            button.interactable = false;
        }
    }
}