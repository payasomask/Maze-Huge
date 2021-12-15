using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WallBuilder : MonoBehaviour{

  public static WallBuilder _WallBuilder = null;
  private List<GameObject> Wall_list = new List<GameObject>();
  private float wallwidth = 2.0f;

  [SerializeField]
  GameObject CellView;
  private GameObjectPool CellViewPool = null;

  private void Awake(){
    _WallBuilder = this;
    CellViewPool = new GameObjectPool(CellView,300);
  }

  public void BuildTopWall(Vector2 position,float grid_size){
    float positiony = position.y + grid_size * 0.5f;
    float positionxstart = position.x - grid_size * 0.5f;
    float positionxend = position.x + grid_size * 0.5f;
    //Debug.DrawLine(new Vector2(positionxstart, positiony), new Vector2(positionxend, positiony), Color.white, 1000.0f);
    CreateWall(new Vector2(positionxstart, positiony), new Vector2(positionxend, positiony));
  }
  public void BuildRightWall(Vector2 position, float grid_size)
  {
    float positionx = position.x + grid_size * 0.5f;
    float positionystart = position.y + grid_size * 0.5f;
    float positionyend = position.y - grid_size * 0.5f;
    //Debug.DrawLine(new Vector2(positionx, positionystart), new Vector2(positionx, positionyend), Color.white, 1000.0f);
    CreateWall(new Vector2(positionx, positionystart), new Vector2(positionx, positionyend));
  }
  public void BuildLeftWall(Vector2 position, float grid_size)
  {
    float positionx = position.x - grid_size * 0.5f;
    float positionystart = position.y + grid_size * 0.5f;
    float positionyend = position.y - grid_size * 0.5f;
    //Debug.DrawLine(new Vector2(positionx, positionystart), new Vector2(positionx, positionyend), Color.white, 1000.0f);
    CreateWall(new Vector2(positionx, positionystart), new Vector2(positionx, positionyend));

  }
  public void BuildBottomWall(Vector2 position, float grid_size)
  {
    float positiony = position.y - grid_size * 0.5f;
    float positionxstart = position.x - grid_size * 0.5f;
    float positionxend = position.x + grid_size * 0.5f;
    //Debug.DrawLine(new Vector2(positionxstart, positiony), new Vector2(positionxend, positiony), Color.white, 1000.0f);
    CreateWall(new Vector2(positionxstart, positiony), new Vector2(positionxend, positiony));
  }

  public void BuildCell(Cell c,float maze_size) {
    GameObject cellView = CellViewPool.Pool.Get();
    Sprite floorSprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", c.floorSpriteName);
    Sprite WallSprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", c.wallSpriteName);
    float iconscale = maze_size / floorSprite.bounds.size.x;//根據圖資重新計算scale大小
    cellView.transform.localScale = new Vector3(iconscale, iconscale, 1.0f);

    MLCamera._MLCamera.setGameObjectLayer(cellView, "MAZE");
    cellView.GetComponent<CellView>().setView(
      floorSprite,
      WallSprite
      );
    cellView.transform.localPosition = c.position();
  }


  void CreateWall(Vector3 Start,Vector3 End){
    GameObject tmp = instantiateObject(gameObject, "Wall");
    MLCamera._MLCamera.setGameObjectLayer(tmp, "MAZE");
    LineRenderer lr = tmp.GetComponent<LineRenderer>();
    lr.SetPosition(0, Start);
    lr.SetPosition(1, End);
    lr.startWidth = wallwidth;
    lr.endWidth = wallwidth;
    Wall_list.Add(tmp);
  }

  public void ClearWall(){
    for(int i = 0; i < Wall_list.Count; i++){
      Destroy(Wall_list[i]);
    }
    Wall_list = new List<GameObject>();

    CellViewPool.Pool.Clear();
  }

  GameObject instantiateObject(GameObject parent, string name){
    GameObject g = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(name);
    g.transform.SetParent(parent.transform, true);

    return g;
  }
}

