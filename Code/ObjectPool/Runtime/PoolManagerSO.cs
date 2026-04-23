using System.Collections.Generic;
using UnityEngine;

namespace _01.Scripts.ObjectPool.Runtime
{
    [CreateAssetMenu(fileName = "PoolManager", menuName = "SO/Pool/Manager", order = 0)]
    public class PoolManagerSO : ScriptableObject
    {
        public List<PoolingItemSO> itemList = new List<PoolingItemSO>();
        
        
        private Dictionary<PoolingItemSO, Pool> _pools;
        private Transform _rootTrm;

        public void Initialize(Transform rootTrm)
        {
            _rootTrm = rootTrm;
            _pools = new Dictionary<PoolingItemSO, Pool>();

            foreach (var item in itemList)
            {
                IPoolable poolable = item.prefab.GetComponent<IPoolable>();
                Debug.Assert(poolable != default(IPoolable), $"Pooling item {item.prefab.name} has no poolable component" );

                GameObject newParent = new GameObject(poolable.PoolingType.poolingName);
                newParent.transform.SetParent(_rootTrm);
                
                Pool pool = new Pool(poolable, newParent.transform, item.initCount);
                _pools.Add(item, pool); 
            }
        }

        public IPoolable Pop(PoolingItemSO findItem)
        {
            if (_pools.TryGetValue(findItem, out Pool pool))
            {
                return pool.Pop();
            }

            return default;
        }

        public void Push(IPoolable item)
        {
            if (_pools.TryGetValue(item.PoolingType, out Pool pool))
            {
                pool.Push(item);
            }
        }
    }
}