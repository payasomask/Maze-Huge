using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
  public static PrefabManager _PrefabManager = null;

  [SerializeField]
  private GameObject[] prefab_list = null;
    
    // Start is called before the first frame update
    void Start()
    {
    _PrefabManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  public GameObject GetPrefab(string name){

    foreach(var v in prefab_list){
      if (v.name == name)
        return v;
    }

    return null;
  }
}
