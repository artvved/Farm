using System;
using Game.Component;
using ScriptableData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class FarmUIScreen : MonoBehaviour
    {
        [SerializeField] private Image cultureImage;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Button exitButton;
        [SerializeField] public UpButton upTimeButton;
        [SerializeField] public UpButton upChanceButton;

        
        private void Awake()
        {
            gameObject.SetActive(false);
            exitButton.onClick.AddListener(()=>gameObject.SetActive(false));
        }
        

        public void UpdateLabel(Sprite cultureIcon,CultureType cultureType)
        {
            cultureImage.sprite = cultureIcon;
            text.text = cultureType.ToString();
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }


        public void UpdateEmpty()
        {
            cultureImage.gameObject.SetActive(false);
            text.text = "Nothing";
            upTimeButton.gameObject.SetActive(false);
            upChanceButton.gameObject.SetActive(false);
        }
    }
}