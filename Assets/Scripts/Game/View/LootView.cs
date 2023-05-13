using System;
using DefaultNamespace;
using UnityEngine;

namespace Game.Mono
{
    public class LootView : BaseView
    {
        private Animator animator;
        private Collider collider;
        
        private void Start()
        {
            animator = GetComponent<Animator>();
            collider = GetComponent<Collider>();
        }

       
    }
}