using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public class ProjectExtension
    {
        public static bool RunCommand(string command)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "powershell";
                process.StartInfo.Arguments = command;
                process.StartInfo.CreateNoWindow = true; // 不显示窗口
                process.StartInfo.ErrorDialog = true;
                process.StartInfo.UseShellExecute = false;
                try
                {
                    process.Start();
                    process.WaitForExit();
                    process.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return false;
                }
            }
            return true;
        }
        
        [MenuItem("Assets/复制", false, 10000)]
        private static void CopyToClipboard()
        {
            StringBuilder stringBuilder = new StringBuilder("Set-Clipboard -Path ");
            for (int i = 0; i < Selection.assetGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);
                if (i != 0) stringBuilder.Append(",");
                stringBuilder.Append("\"");
                stringBuilder.Append(Path.GetFullPath(path));
                stringBuilder.Append("\"");
            }
            RunCommand(stringBuilder.Replace("/", "\\").ToString());
        }
        
        [MenuItem("Assets/粘贴", false, 10000)]
        private static void PasteFromClipboard()
        {
            StringBuilder stringBuilder = new StringBuilder("Get-Clipboard ");
        }
    }
}