using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool
{

  public GameObjectPool(GameObject prefab,int maxSize, PoolType poolType = PoolType.Stack) {
    _Prefab = prefab;
    maxPoolSize = maxSize;
    this.poolType = poolType;
  }

  public enum PoolType {
    Stack,
    LinkedList
  }

  PoolType poolType;

  // Collection checks will throw errors if we try to release an item that is already in the pool.
  bool collectionChecks = true;
  int maxPoolSize = 300;
  GameObject _Prefab = null;
  IObjectPool<GameObject> m_Pool;

  public IObjectPool<GameObject> Pool {
    get {
      if (m_Pool == null) {
        if (poolType == PoolType.Stack)
          m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 100, maxPoolSize);
        else
          m_Pool = new LinkedPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, maxPoolSize);
      }
      return m_Pool;
    }
  }

  GameObject CreatePooledItem() {
    if (_Prefab == null)
      return null;
    var go = GameObject.Instantiate(_Prefab);
    //var ps = go.AddComponent<ParticleSystem>();
    //ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

    //var main = ps.main;
    //main.duration = 1;
    //main.startLifetime = 1;
    //main.loop = false;

    // This is used to return ParticleSystems to the pool when they have stopped.
    //var returnToPool = go.AddComponent<ReturnToPool>();
    //returnToPool.pool = Pool;

    return go;
  }

  // Called when an item is returned to the pool using Release
  void OnReturnedToPool(GameObject go) {
    go.SetActive(false);
  }

  // Called when an item is taken from the pool using Get
  void OnTakeFromPool(GameObject go) {
    go.SetActive(true);
  }

  // If the pool capacity is reached then any items returned will be destroyed.
  // We can control what the destroy behavior does, here we destroy the GameObject.
  void OnDestroyPoolObject(GameObject go) {
    GameObject.Destroy(go);
  }
}
