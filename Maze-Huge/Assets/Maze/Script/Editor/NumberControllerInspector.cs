using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor(typeof(NumberController))]
[CanEditMultipleObjects]
public class NumberControllerInspector : Editor
{

  SerializedProperty script = null;

  SerializedProperty sprite_rect_width_scale = null;
  SerializedProperty cancelline_extra_length = null;

  SerializedProperty digitsAtlasName = null;
  SerializedProperty digitsSpriteName = null;

  SerializedProperty commaAtlasName = null;
  SerializedProperty commaSpriteName = null;
  SerializedProperty useComma = null;
  SerializedProperty useCancelLine = null;
  SerializedProperty usePoint = null;

  SerializedProperty pointAtlasName = null;
  SerializedProperty pointSpriteName = null;

  SerializedProperty unitAtTheEndAtlasName = null;
  SerializedProperty unitAtTheEndSpriteName = null;

  SerializedProperty unitAtTheFrontAtlasName = null;
  SerializedProperty unitAtTheFrontSpriteName = null;

  SerializedProperty CancelLineAtlasName = null;
  SerializedProperty CancelLineSpriteName = null;

  SerializedProperty extraSymbolAtlasName = null;
  SerializedProperty extraSymbolSpriteName = null;

  Rect unitAtTheEnd_rect_parent;
  Rect unitAtTheFront_rect_parent;
  Rect cancelLine_rect_parent;


  Rect comma_rect_parent;
  Rect dash_rect_parent;
  Rect point_rect_parent;
  Rect sprite_rect_parent; //digits

  void LayoutSpriteUI(string title, SerializedProperty property)
  {
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PrefixLabel(title);
    EditorGUILayout.SelectableLabel(property.stringValue, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
    EditorGUILayout.EndHorizontal();
  }

  public override void OnInspectorGUI()
  {
    if (script == null)
    {
      script = serializedObject.FindProperty("m_Script");
      digitsAtlasName = serializedObject.FindProperty("digitsAtlasName");
      digitsSpriteName = serializedObject.FindProperty("digitsSpriteName");

      commaAtlasName = serializedObject.FindProperty("commaAtlasName");
      commaSpriteName = serializedObject.FindProperty("commaSpriteName");

      unitAtTheFrontAtlasName = serializedObject.FindProperty("unitAtTheFrontAtlasName");
      unitAtTheFrontSpriteName = serializedObject.FindProperty("unitAtTheFrontSpriteName");

      CancelLineAtlasName = serializedObject.FindProperty("CancelLineAtlasName");
      CancelLineSpriteName = serializedObject.FindProperty("CancelLineSpriteName");

      unitAtTheEndAtlasName = serializedObject.FindProperty("unitAtTheEndAtlasName");
      unitAtTheEndSpriteName = serializedObject.FindProperty("unitAtTheEndSpriteName");

      pointAtlasName = serializedObject.FindProperty("pointAtlasName");
      pointSpriteName = serializedObject.FindProperty("pointSpriteName");

      extraSymbolAtlasName = serializedObject.FindProperty("extraSymbolAtlasName");
      extraSymbolSpriteName = serializedObject.FindProperty("extraSymbolSpriteName");

      useComma = serializedObject.FindProperty("mUseComma");
      useCancelLine = serializedObject.FindProperty("mUseCancelLine");
      usePoint = serializedObject.FindProperty("mUsePoint");

      sprite_rect_width_scale = serializedObject.FindProperty("sprite_rect_width_scale");
      cancelline_extra_length = serializedObject.FindProperty("cancelline_extra_length");
    }

    serializedObject.Update();

    GUI.enabled = false;
    EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
    GUI.enabled = true;

    EditorGUILayout.Space();

    EditorGUILayout.PropertyField(sprite_rect_width_scale, new GUIContent("Digit Width Scale"));

    EditorGUILayout.PropertyField(cancelline_extra_length, new GUIContent("CencelLine Extra Length"));

    GUI.SetNextControlName("pointSpriteName");
    LayoutSpriteUI("Point Sprite", pointSpriteName);
    if (Event.current.type == EventType.Repaint)
    {
      point_rect_parent = GUILayoutUtility.GetLastRect();
    }

    GUI.SetNextControlName("UnitAtTheFrontSpriteName");
    LayoutSpriteUI("Unit at the front Sprite", unitAtTheFrontSpriteName);
    if (Event.current.type == EventType.Repaint)
    {
      unitAtTheFront_rect_parent = GUILayoutUtility.GetLastRect();
    }

    GUI.SetNextControlName("UnitAtTheEndSpriteName");
    LayoutSpriteUI("Unit at the end Sprite", unitAtTheEndSpriteName);
    if (Event.current.type == EventType.Repaint)
    {
      unitAtTheEnd_rect_parent = GUILayoutUtility.GetLastRect();
    }

    GUI.SetNextControlName("CancelLineSpriteName");
    LayoutSpriteUI("CancelLine Sprite", CancelLineSpriteName);
    if (Event.current.type == EventType.Repaint)
    {
      cancelLine_rect_parent = GUILayoutUtility.GetLastRect();
    }

    GUI.SetNextControlName("commaSpriteName");
    LayoutSpriteUI("Comma Sprite", commaSpriteName);
    if (Event.current.type == EventType.Repaint)
    {
      comma_rect_parent = GUILayoutUtility.GetLastRect();
    }

    GUI.SetNextControlName("useComma");
    EditorGUILayout.PropertyField(useComma);

    GUI.SetNextControlName("useCancelLine");
    EditorGUILayout.PropertyField(useCancelLine);

    EditorGUILayout.Space();

    GUI.SetNextControlName("digitsSpriteName");
    bool isList1 = EditorGUILayout.PropertyField(digitsSpriteName);
    if (Event.current.type == EventType.Repaint)
    {
      sprite_rect_parent = GUILayoutUtility.GetLastRect();
    }
    if (isList1 && digitsSpriteName.isExpanded)
    {
      EditorGUI.indentLevel++;
      EditorGUILayout.PropertyField(digitsSpriteName.FindPropertyRelative("Array.size"));
      for (int j = 0; j < digitsSpriteName.arraySize; ++j)
      {
        EditorGUILayout.PropertyField(digitsSpriteName.GetArrayElementAtIndex(j));
      }
      EditorGUI.indentLevel--;
    }

    EditorGUILayout.Space();

    GUI.SetNextControlName("extraSymbolSpriteName");
    bool isList2 = EditorGUILayout.PropertyField(extraSymbolSpriteName);
    if (Event.current.type == EventType.Repaint)
    {
      dash_rect_parent = GUILayoutUtility.GetLastRect();
    }
    if (isList2 && extraSymbolSpriteName.isExpanded)
    {
      EditorGUI.indentLevel++;
      EditorGUILayout.PropertyField(extraSymbolSpriteName.FindPropertyRelative("Array.size"));
      for (int j = 0; j < extraSymbolSpriteName.arraySize; ++j)
      {
        EditorGUILayout.PropertyField(extraSymbolSpriteName.GetArrayElementAtIndex(j));
      }
      EditorGUI.indentLevel--;
    }

    DropToAddSingleSprite("UnitAtTheFrontSpriteName", unitAtTheFront_rect_parent, unitAtTheFrontSpriteName, unitAtTheFrontAtlasName);
    DropToAddSingleSprite("UnitAtTheEndSpriteName", unitAtTheEnd_rect_parent, unitAtTheEndSpriteName, unitAtTheEndAtlasName);
    DropToAddSingleSprite("CancelLineSpriteName", cancelLine_rect_parent, CancelLineSpriteName, CancelLineAtlasName);
    DropToAddSingleSprite("commaSpriteName", comma_rect_parent, commaSpriteName, commaAtlasName);
    DropToAddSingleSprite("pointSpriteName", point_rect_parent, pointSpriteName, pointAtlasName);
    DropToAddMultipleSprite(sprite_rect_parent, digitsSpriteName, digitsAtlasName);
    DropToAddMultipleSprite(dash_rect_parent, extraSymbolSpriteName, extraSymbolAtlasName);

    // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
    serializedObject.ApplyModifiedProperties();

  }

  void DropToAddSingleSprite(string controlname, Rect rc, SerializedProperty sp, SerializedProperty atlas_sp)
  {
    Event evt = Event.current;

    if (evt.isKey && (evt.keyCode == KeyCode.Backspace || evt.keyCode == KeyCode.Delete) && GUI.GetNameOfFocusedControl() == controlname)
    {
      sp.stringValue = null;
      atlas_sp.stringValue = null;
      serializedObject.ApplyModifiedProperties();

      GUI.FocusControl("dummy"); //FORCE REPAINT TO WORK
      Repaint();

      return;
    }

    if (evt.type == EventType.DragExited)
      DragAndDrop.PrepareStartDrag();
    if (!rc.Contains(evt.mousePosition))
      return;

    switch (evt.type)
    {
      case EventType.DragUpdated:
        if (isDragTargetValid())
        {
          DragAndDrop.visualMode = DragAndDropVisualMode.Link;
        }
        else
        {
          DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
        }

        evt.Use();
        break;

      case EventType.DragPerform:
        DragAndDrop.AcceptDrag();
        DragAndDrop.visualMode = DragAndDropVisualMode.Link;

        foreach (Object dragged_object in DragAndDrop.objectReferences)
        {
          if (dragged_object.GetType().ToString() == "UnityEngine.Sprite")
          {
            //get atlas name
            string atlas_name = ((Sprite)dragged_object).texture.name;
            string sprite_name = dragged_object.name;
            Debug.Log("atlas name =" + atlas_name + ", sprite name=" + sprite_name);

            sp.stringValue = sprite_name;
            atlas_sp.stringValue = atlas_name;
            serializedObject.ApplyModifiedProperties();
          }
        }
        evt.Use();
        break;
    }
  }

  void DropToAddMultipleSprite(Rect rc, SerializedProperty sp, SerializedProperty atlas_sp)
  {
    Event evt = Event.current;
    if (evt.type == EventType.DragExited)
      DragAndDrop.PrepareStartDrag();
    if (!rc.Contains(evt.mousePosition))
      return;

    switch (evt.type)
    {
      case EventType.DragUpdated:
        if (isDragTargetValid())
        {
          DragAndDrop.visualMode = DragAndDropVisualMode.Link;
        }
        else
        {
          DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
        }

        evt.Use();
        break;

      case EventType.DragPerform:
        DragAndDrop.AcceptDrag();
        DragAndDrop.visualMode = DragAndDropVisualMode.Link;

        foreach (Object dragged_object in DragAndDrop.objectReferences)
        {
          if (dragged_object.GetType().ToString() == "UnityEngine.Sprite")
          {
            //get atlas name
            string atlas_name = ((Sprite)dragged_object).texture.name;
            string sprite_name = dragged_object.name;
            Debug.Log("atlas name =" + atlas_name + ", sprite name=" + sprite_name);

            sp.arraySize++;
            atlas_sp.arraySize = sp.arraySize;
            int idx = sp.arraySize - 1;
            sp.GetArrayElementAtIndex(idx).stringValue = sprite_name;
            atlas_sp.GetArrayElementAtIndex(idx).stringValue = atlas_name;
            serializedObject.ApplyModifiedProperties();
          }
        }
        evt.Use();
        break;
    }
  }

  bool isDragTargetValid()
  {
    foreach (Object dragged_object in DragAndDrop.objectReferences)
    {
      if (dragged_object.GetType().ToString() != "UnityEngine.Sprite")
      {
        return false;
      }
    }

    return true;
  }
}
