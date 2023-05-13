using Game.Mono;
using Game.UI;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        public PlayerData playerData;

        public PlayerView PlayerPrefab;

        public CultureData[] Cultures;

        //ui
    }
}