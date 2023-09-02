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
        
        [SerializeField] public Button defaultGroundButton;
        [SerializeField] public Button pictureButton;
        [SerializeField] public UpButton upTimeButton;
        [SerializeField] public UpButton upChanceButton;
        [SerializeField] public int size;
        


        private void Awake()
        {
            gameObject.SetActive(false);
            exitButton.onClick.AddListener(() => gameObject.SetActive(false));
        }

        public void UpdateLabel(Sprite cultureIcon, CultureType cultureType)
        {
            cultureImage.gameObject.SetActive(true);
            cultureImage.sprite = cultureIcon;
            text.text = cultureType.ToString();
            ToggleButtons(true);
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        private void ToggleButtons(bool active)
        {
            upTimeButton.gameObject.SetActive(active);
            upChanceButton.gameObject.SetActive(active);
            pictureButton.gameObject.SetActive(active);
            defaultGroundButton.gameObject.SetActive(active);
        }

        public void UpdateEmpty()
        {
            cultureImage.gameObject.SetActive(false);
            text.text = "Nothing";
            ToggleButtons(false);
        }
    }
}