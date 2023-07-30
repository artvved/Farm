using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Mono
{
    public class StackView : MonoBehaviour
    {
        [SerializeField] private float itemMoveDuration;
        [SerializeField] private int yRows;
        [SerializeField] private int xRows;
        [SerializeField] private int zRows;
        [SerializeField] private float offset;
        [SerializeField] private Transform parent; 
        private Transform[] itemPlaces;
        
        private List<LootView> items=new();
        
        private void Awake()
        {
            itemPlaces = new Transform[xRows * yRows * zRows];
            int count = 0;
            for (int y = 0; y < yRows; y++)
            {
                for (int x = 0; x < xRows; x++)
                {
                    for (int z = 0; z < zRows; z++)
                    {
                        var go = new GameObject();
                        go.transform.position = parent.position + new Vector3(x, y, z) * offset;
                        go.transform.parent = parent;
                        itemPlaces[count] = go.transform;
                        count++;
                    }
                }
            }
        }

        public void AddItem(LootView lootView)
        {
            lootView.ToggleCollider(false);
            items.Add(lootView);
            int i = items.Count - 1;

            Move( items[i].transform,itemPlaces[i]);

        }

        private void Move(Transform itemTr,Transform targetTr)
        {
            itemTr.SetParent(targetTr);
            //itemTr.DOLocalMove(Vector3.zero, itemMoveDuration);
           // itemTr.DOJump(targetTr.position, 2f, 1, itemMoveDuration);
              DOTween.Sequence()
                .Append(itemTr.DOJump(targetTr.position, 2, 1, itemMoveDuration))
                .Join(itemTr.DOLocalMoveX(Vector3.zero.x, itemMoveDuration))
                .Join(itemTr.DOLocalMoveZ(Vector3.zero.z, itemMoveDuration));
        }

        public void ReleaseAll(Transform target)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var itemTr= items[i].transform;
                itemTr.SetParent(target);
                DOTween.Sequence()
                    .Append(itemTr.DOJump(target.position, 2, 1, itemMoveDuration))
                    .Join(itemTr.DOLocalMoveX(Vector3.zero.x, itemMoveDuration))
                    .Join(itemTr.DOLocalMoveZ(Vector3.zero.z, itemMoveDuration))
                    .SetDelay(0.2f)
                    .OnComplete(() => {
                        for (int j = 0; j < items.Count; j++)
                        {
                            Destroy(items[j].gameObject);
                        }
                        items.Clear();
                    });
            }
           
        }

    }
}