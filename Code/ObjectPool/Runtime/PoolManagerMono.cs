using UnityEngine;

namespace _01.Scripts.ObjectPool.Runtime
{
    public class PoolManagerMono : MonoBehaviour
    {
        [SerializeField] private PoolManagerSO poolManager;

        private void Awake()
        {
            poolManager.Initialize(transform);
        }

        public T Pop<T>(PoolingItemSO item) where T : IPoolable
        {
            return (T)poolManager.Pop(item);
        }

        public void Push(IPoolable target)
        {
            poolManager.Push(target);
        }
    }
}
