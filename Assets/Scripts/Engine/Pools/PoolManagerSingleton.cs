// Copyright (c) Meta Platforms, Inc. and affiliates.

using MRUtils;
using UnityEngine;

namespace MR
{
    [RequireComponent(typeof(PoolManagerComponent))]
    [SingletonMonoBehaviour.InstantiationSettings(dontDestroyOnLoad = false)]
    public class PoolManagerSingleton : SingletonMonoBehaviour<PoolManagerSingleton>
    {
        private PoolManagerComponent poolManagerComponent;
        public PoolManager<GameObject, Pool<GameObject>> poolManager => poolManagerComponent.poolManager;

        private void Start()
        {
            poolManagerComponent = GetComponent<PoolManagerComponent>();
        }

        /*
         * Create is a drop-in replacement for Instantiate that uses a pool if available.
         * Note that it is not named Instantiate so that it is easy to find & replace Instantiate
         *  calls with Create calls when switching to using Pools.
         */
        public GameObject Create(GameObject primitive,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null)
        {
            return poolManagerComponent.Create(primitive,
                position,
                rotation,
                parent);
        }

        public GameObject Create(GameObject primitive,
            Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            return poolManagerComponent.Create(primitive,
                parent,
                instantiateInWorldSpace);
        }

        public T Create<T>(T primitive,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null) where T : Component
        {
            return poolManagerComponent.Create(primitive,
                position,
                rotation,
                parent);
        }

        public T Create<T>(T primitive,
            Transform parent = null,
            bool instantiateInWorldSpace = false) where T : Component
        {
            return poolManagerComponent.Create(primitive,
                parent,
                instantiateInWorldSpace);
        }

        /*
         * Discard is a drop-in replacement for Destroy that releases into a pool if available.
         * Note that it is not named Destroy so that it is easy to find & replace Destroy
         *  calls with Discard calls when switching to using Pools.
         */
        public void Discard(GameObject go)
        {
            poolManagerComponent.Discard(go);
        }
    }
}
