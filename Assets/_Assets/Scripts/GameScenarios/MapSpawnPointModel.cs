using UnityEngine;
using System.Collections.Generic;

namespace LazyFace
{
    public class MapSpawnPointModel : MonoBehaviour
    {
        [SerializeField] private List<Transform> spawnPointsTransform;

        public List<Transform> GetSpawnPointsTransform()
        {
            return spawnPointsTransform;
        }
    }
}
