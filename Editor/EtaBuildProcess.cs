using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once RedundantUsingDirective
using System.IO;

namespace ETA_Editor
{
    public class EtaBuildProcess : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Object inputManager = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/InputManager.asset");
            if (inputManager == null ) { return; }

            StringBuilder axesNames = new StringBuilder();
            SerializedObject obj = new SerializedObject(inputManager);
            SerializedProperty axisArray = obj.FindProperty("m_Axes");

            for (int i = 0; i < axisArray.arraySize; i++)
            {
                SerializedProperty axis = axisArray.GetArrayElementAtIndex(i);
                string name = axis.FindPropertyRelative("m_Name").stringValue;
                axesNames.AppendLine(name);
            }

            if (Directory.Exists(Application.streamingAssetsPath) == false)
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            string filepath = Path.Combine(Application.streamingAssetsPath, _filename);
            File.WriteAllText(filepath, axesNames.ToString());
            AssetDatabase.Refresh();
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            string filepath = Path.Combine(Application.streamingAssetsPath, _filename);
            File.Delete(filepath);
            File.Delete(filepath + ".meta");
        
            if (Directory.Exists(Application.streamingAssetsPath) && Directory.GetFiles(Application.streamingAssetsPath).Length == 0)
            {
                Directory.Delete(Application.streamingAssetsPath);
                File.Delete(Application.streamingAssetsPath + ".meta");
            }

            AssetDatabase.Refresh();
        }

        private readonly string _filename = "ETA_Axes.txt";
    }
}