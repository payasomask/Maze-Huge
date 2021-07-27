using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor (typeof(UIButton))]
[CanEditMultipleObjects]
public class UIButtonInspector : Editor {

  SerializedProperty script =null;

  SerializedProperty hoverState_atlas = null;
  SerializedProperty disabledState_atlas = null;
  SerializedProperty hoverState =null;
  SerializedProperty disabledState = null;

  SerializedProperty passTouchEvent =null;
  SerializedProperty passClickEvent =null;

  SerializedProperty longPressDuration =null;

  SerializedProperty CurrentAction = null;

  SerializedProperty btn_enabled =null;

  Rect hoverRect;
  Rect disabledRect;

  Sprite internalSpriteRef = null;
  public override void OnInspectorGUI ()
  {
    if (script ==null){
      script =serializedObject.FindProperty("m_Script");
      hoverState_atlas = serializedObject.FindProperty("hoverState_atlasName");
      hoverState =serializedObject.FindProperty("hoverState_spriteName");
      disabledState_atlas = serializedObject.FindProperty("disabledState_atlasName");
      disabledState = serializedObject.FindProperty("disabledState_spriteName");
      passTouchEvent =serializedObject.FindProperty("passTouchEvent");
      passClickEvent =serializedObject.FindProperty("passClickEvent");

      longPressDuration =serializedObject.FindProperty("longPressDuration");

      CurrentAction = serializedObject.FindProperty("CurrentAction");
      btn_enabled =serializedObject.FindProperty("btn_enabled");

    }

    serializedObject.Update ();

    GUI.enabled =false;
    EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
    GUI.enabled =true;

    EditorGUILayout.PropertyField(btn_enabled, new GUIContent("Enabled"), new GUILayoutOption[0]);

    EditorGUILayout.PropertyField(passTouchEvent, true, new GUILayoutOption[0]);
    EditorGUILayout.PropertyField(passClickEvent, true, new GUILayoutOption[0]);
    EditorGUILayout.PropertyField(longPressDuration, true, new GUILayoutOption[0]);

    EditorGUILayout.PropertyField(CurrentAction, true, new GUILayoutOption[0]);

    GUI.SetNextControlName("hoverSpriteName");
    Rect tmprc1 =LayoutSpriteUI("Hover State", hoverState);
    if(Event.current.type == EventType.Repaint) {
      hoverRect = tmprc1;
    }

    GUI.SetNextControlName("disabledSpriteName");
    Rect tmprc2 =LayoutSpriteUI("Disabled State", disabledState);
    if(Event.current.type == EventType.Repaint) {
      disabledRect = tmprc2;
    }

    DropToAddSingleSprite("hoverSpriteName", hoverRect, hoverState, hoverState_atlas);
    DropToAddSingleSprite("disabledSpriteName", disabledRect, disabledState, disabledState_atlas);

    // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
    serializedObject.ApplyModifiedProperties ();

  }

  Rect LayoutSpriteUI(string title, SerializedProperty property) {
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PrefixLabel(title);
    EditorGUILayout.TextField(property.stringValue, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
    Rect r = GUILayoutUtility.GetLastRect();
    EditorGUILayout.EndHorizontal();

    return r;
  }

  void DropToAddSingleSprite(string controlname, Rect rc, SerializedProperty sp, SerializedProperty atlas_sp) {
    Event evt = Event.current;

    if(evt.isKey && (evt.keyCode == KeyCode.Backspace || evt.keyCode == KeyCode.Delete) && GUI.GetNameOfFocusedControl() == controlname) {
      sp.stringValue = null;
      atlas_sp.stringValue = null;
      serializedObject.ApplyModifiedProperties();

      GUI.FocusControl("dummy"); //FORCE REPAINT TO WORK
      Repaint();

      return;
    }

    if(evt.type == EventType.DragExited)
      DragAndDrop.PrepareStartDrag();
    if(!rc.Contains(evt.mousePosition))
      return;

    switch(evt.type) {
      case EventType.Repaint:
        if(
        DragAndDrop.visualMode == DragAndDropVisualMode.None ||
        DragAndDrop.visualMode == DragAndDropVisualMode.Rejected) break;

        EditorGUI.DrawRect(rc, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        break;

      case EventType.DragUpdated:
        if(isDragTargetValid()) {
          DragAndDrop.visualMode = DragAndDropVisualMode.Link;

        } else {
          DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

        }

        evt.Use();
        break;

      case EventType.DragPerform:
        DragAndDrop.AcceptDrag();
        DragAndDrop.visualMode = DragAndDropVisualMode.Link;

        foreach(Object dragged_object in DragAndDrop.objectReferences) {
          if(dragged_object.GetType().ToString() == "UnityEngine.Sprite") {
            //get atlas name
            string atlas_name = ((Sprite)dragged_object).texture.name;
            string sprite_name = dragged_object.name;
            Debug.Log("atlas name =" + atlas_name + ", sprite name=" + sprite_name);

            sp.stringValue = sprite_name;
            atlas_sp.stringValue = atlas_name;

            internalSpriteRef = (Sprite)dragged_object;
            serializedObject.ApplyModifiedProperties();
          }
        }
        evt.Use();
        break;


    }
  }

  bool isDragTargetValid() {
    foreach(Object dragged_object in DragAndDrop.objectReferences) {
      if(dragged_object.GetType().ToString() != "UnityEngine.Sprite") {
        return false;
      }
    }

    return true;
  }


}
