using System;
using DefaultNamespace;
using UnityEngine;

namespace Game.Mono
{
    public class CultureView : BaseView
    {
        private Animator animator;
        private Collider collider;
        
        private void Start()
        {
            animator = GetComponent<Animator>();
            collider = GetComponent<Collider>();
        }

        public void Harvest()
        {
            collider.enabled = false;
            animator.SetTrigger(Idents.Animation.Harvest);
        }
        public void Grow()
        {
            collider.enabled = true;
            animator.SetTrigger(Idents.Animation.Grow);
        }
    }
}