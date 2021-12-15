using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;

public class Process : IPreprocessBuildWithReport{
  public int callbackOrder { get { return 0; } }

  public void OnPreprocessBuild(BuildReport report) {
    bool forceStop = false;

    string key = "9v~pJcOkGZYQ";
    if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass)) {
      //throw new BuildFailedException("key password is empty ");
      PlayerSettings.Android.keystorePass = key;
    }

    if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass)) {
      //throw new BuildFailedException("key password is empty ");
      PlayerSettings.Android.keyaliasPass = key;
    }


    bool useCuurrentBundleCode = EditorUtility.DisplayDialog("preBuild(bundle Version Check)", "current bundle Version : " + PlayerSettings.Android.bundleVersionCode, "ok", "step");
    if (!useCuurrentBundleCode) {
      PlayerSettings.Android.bundleVersionCode++;
      forceStop = EditorUtility.DisplayDialog("preBuild(bundle Version Check)", "current bundle Version : " + PlayerSettings.Android.bundleVersionCode, "cancel Build", "ok");
    }

    if (forceStop) {
      throw new BuildFailedException("Force Stop");
    }
    //Debug.unityLogger.logEnabled = EditorUtility.DisplayDialog("prebuild...", "dsiable Debug.Log?", "enable", "disable");
    //Debug.Log("MyCustomBuildProcessor.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
  }

  [PostProcessBuildAttribute(1)]
  public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
    EditorUtility.DisplayDialog("AfterBuild(Info)", "info has been build : \n" +
      "Bundle Version : " + PlayerSettings.Android.bundleVersionCode +"\n" +
      "key : " + PlayerSettings.Android.keystorePass, "ok");
  }
}
