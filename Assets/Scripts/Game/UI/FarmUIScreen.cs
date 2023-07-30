using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class FarmUIScreen : MonoBehaviour
    {
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            exitButton.onClick.AddListener(()=>gameObject.SetActive(false));
        }
    }
}