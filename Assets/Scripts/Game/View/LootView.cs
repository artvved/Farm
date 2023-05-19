using System;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;


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
            collider.enabled = false;
            var newPos =transform.position+ new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            DOTween.Sequence()
                .Append(transform.DOJump(newPos, 2f, 1, 0.5f))
                .Append(transform.DOMoveY(0,0.1f))
                .AppendCallback(() => collider.enabled = true);

        }

       
    }
}