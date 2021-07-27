using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//負責記錄Linerenderer的資料
public class LineRenderManager : MonoBehaviour
{
  public static LineRenderManager _LineRenderManager = null;
  [SerializeField]
  private Color LineColor = Color.red;
  float line_scale = 0.1f;
  private void Awake(){
    _LineRenderManager = this;
  }

    public void BuildLine(Vector3[] positions){
    GameObject tmp = new GameObject("Line");
    LineRenderer lr = tmp.AddComponent<LineRenderer>();
    lr.alignment = LineAlignment.TransformZ;
    lr.endColor = LineColor;
    lr.startColor = LineColor;
    lr.startWidth = lineWidth();
    lr.endWidth = lineWidth();
    lr.SetPositions(positions);
  }

  public float lineWidth(){
    return MazeManager._MazeManager.getCellSize() * line_scale;
  }
}
