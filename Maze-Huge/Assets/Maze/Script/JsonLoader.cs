using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonLoader : MonoBehaviour
{

  string jsonfilename = "BigMazeClassSetting.xlsx";

  public static JsonLoader _JsonLoader = null;
  Dictionary<int, MazeConfig> mazeConfig_dic = new Dictionary<int, MazeConfig>();

  private void Awake(){
    _JsonLoader = this;
  }

  public void Init(){
    //start init

    Dictionary<string, object> jsontable = (Dictionary<string, object>)MiniJSON.Json.Deserialize(getJson(jsonfilename));
    Dictionary<string, object> tablesheets = (Dictionary<string, object>)jsontable[jsonfilename];
    //parse string_sheet
    //Dictionary<string, object> String_sheets = (Dictionary<string, object>)tablesheets["String"];
    //foreach (var v in String_sheets)
    //{
    //  Dictionary<string, object> string_value = (Dictionary<string, object>)v.Value;
    //  ml_string tmp = new ml_string();
    //  string string_key = v.Key;
    //  tmp.addLang(string_value, "String", available_language_arr);
    //  string_dic.Add(string_key, tmp);
    //}

    //parse data_sheet
    Dictionary<string, object> Data_sheets = (Dictionary<string, object>)tablesheets["Setup"];
    foreach (var v in Data_sheets)
    {
      int start = int.Parse(v.Key);
      Dictionary<string, object> vv =  (Dictionary<string, object>)v.Value;

      string ends = (string)vv["End"];
      //string sstring = (string)vv["End"];
      string Xs = (string)vv["X"];
      string Ys = (string)vv["Y"];

      int end = int.Parse(ends);
      int X = int.Parse(Xs);
      int Y = int.Parse(Ys);

      for (int i = start; i <= end; i++){
        MazeConfig tmp = new MazeConfig();
        tmp.Rows = X;
        tmp.Columns = Y;
        tmp.boxADReward = new ADReward(ItmeType.Item2, 1, ItmeType.Item1, 1);
        tmp.CompletedReward = new ADReward(ItmeType.Item2, 1, ItmeType.Item1, 1);
        tmp.GameOverReward = new ADReward(ItmeType.Item2, 1, ItmeType.Item1, 1);
        tmp.DownUIReward = new ADReward(ItmeType.Item2, 1, ItmeType.Item1, 0);
        mazeConfig_dic.Add(i, tmp);
      }
    }

    Debug.Log("562 - Loaded Json");

    //level init

  }
  string getJson(string jsonfilename)
  {
    string json_text;
#if UNITY_ANDROID || UNITY_EDITOR
    TextAsset t = (TextAsset)Resources.Load(jsonfilename);
    json_text = t.text;
#elif UNITY_STANDALONE_WIN
    string file_path = Path.Combine(Application.streamingAssetsPath, jsonfilename + ".json");
    return File.ReadAllText(file_path);
#endif

    return json_text;
  }

  public MazeConfig GetMazeConfig(int level){

    if (mazeConfig_dic.Count == 0){
      Debug.Log("645 - Init JsonLoader first..");
      return null;
    }
    

    return mazeConfig_dic[level];
  }
}

public enum ItmeType{
  Item1,
  Item2
}

//public enum GameType{
//  LIGHT,
//  NIGHT
//}

public enum MazeType{
  Prims=0,
  HuntKill,
  SZ
}

public class ADReward{
  public ADReward(ItmeType Type, int Num, ItmeType skipType, int SkipNum)
  {
    this.Type = Type;
    this.Num = Num;
    this.SkipType = skipType;
    this.SkipNum = SkipNum;
  }
  //看廣告
  public ItmeType Type;
  public int Num;
  //不看廣告獎勵
  public ItmeType SkipType;
  public int SkipNum;
}

public class MazeConfig{
  public int Rows, Columns;
  public ADReward boxADReward;
  public ADReward CompletedReward;//
  public ADReward GameOverReward;//
  public ADReward DownUIReward;//下方廣告獎勵
}

public class MazeRecord{
  public Cell[,] cells;
  public JsonVector2 playerlocation;
  public JsonVector2 goallocation;
  public JsonVector2 boxlocation;
  public List<JsonVector3[]> lineRender_lists = null;
  public float time;
}

public class JsonVector2{
  public float x;
  public float y;
  public JsonVector2(float x, float y){
    this.x = x;
    this.y = y;
  }
  public Vector2 vector2() {
    return new Vector2(x, y);
  }
}

public class JsonVector3{
  public float x;
  public float y;
  public float z;
  public JsonVector3(float x, float y, float z)
  {
    this.x = x;
    this.y = y;
    this.z = z;
  }
  public Vector3 vector3()
  {
    return new Vector3(x, y, z);
  }
}
