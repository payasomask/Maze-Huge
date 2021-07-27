using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDebug : MonoBehaviour
{
  public class DebugGrid{
    public Vector2 position;
    public Color color;
    public float size;
    }
  public static GizmosDebug _gizmosdebug = null;
  private List<DebugGrid> DebugPosition_list = new List<DebugGrid>();
  private void Awake(){
    _gizmosdebug = this;
  }
  // Start is called before the first frame update
  void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  private void OnDrawGizmos(){
    if (DebugPosition_list == null)
      return;
    if (DebugPosition_list.Count == 0)
      return;
    for(int i = 0; i < DebugPosition_list.Count; i++){
      Vector2 grid_size = new Vector2(DebugPosition_list[i].size, DebugPosition_list[i].size);
      Gizmos.color = DebugPosition_list[i].color;
      Gizmos.DrawWireCube(DebugPosition_list[i].position,grid_size);
    }
  }

  public void AddDebugGrid(Vector2 position,Color color,float size){
    if (DebugPosition_list == null)
      DebugPosition_list = new List<DebugGrid>();

    DebugPosition_list.Add(new DebugGrid() { position = position,color = color,size = size });
  }

  public void clear_debug_list(){
    DebugPosition_list.Clear();
    DebugPosition_list = null;
  }
}
