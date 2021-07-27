using UnityEngine;
using System.Collections;
using System;

public enum SceneSettingsType{
  SCENE_TYPE =0,
  LAYOUT_TYPE,
  PROBABILITY_TYPE/*,
  SCENE_EXTRA_INFO,
  GAME_NO,
  MISSION_INFO*/
}


public interface ISceneSettings{
  object getSettings(string gameName, SceneSettingsType type);
}
