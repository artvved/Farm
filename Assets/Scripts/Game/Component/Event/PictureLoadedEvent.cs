using UnityEngine;

namespace Game.Component
{
    public struct PictureLoadedEvent
    {
        public Data Value;
        public struct Data
        {
            public Texture2D Picture;
            public int Farm;
        }
    }
}