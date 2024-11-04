using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class ABAssetBudleEnp : EditorWindow
{
    public static AESConfigScript config
    {
        get
        {
            return Resources.Load<AESConfigScript>("AESConfig");
        }
    }
    [MenuItem("Asset/ABEncy")]
    public static void Init()
    {
        EditorWindow window = GetWindow<ABAssetBudleEnp>();
        window.Show();
        window.Focus();
    }

    void ClearFolder(string folderPath)
    {
        if (GUILayout.Button("清除文件夹"))
        { 
            string[] files = Directory.GetFiles(folderPath);

            // 删除所有文件
            foreach (string file in files)
            {
                File.Delete(file);
                Console.WriteLine($"Deleted file: {file}");
            }

            Refresh();
        }
    }
    public Vector2 offset;
    private void OnGUI()
    {
        if (GUILayout.Button("选择导入文件夹"))
        {
            config.inputFolder = EditorUtility.OpenFolderPanel("选择加密导入文件夹", "", "");
        }
        GUILayout.BeginHorizontal();
        config.inputFolder =  GUILayout.TextField(config.inputFolder);
        ClearFolder(config.inputFolder);
        
        GUILayout.EndHorizontal();
        if (GUILayout.Button("选择输出文件夹"))
        {
            config.outFolder = EditorUtility.OpenFolderPanel("选择加密导出文件夹", "", "");
        }
        GUILayout.BeginHorizontal();
        config.outFolder = GUILayout.TextField(config.outFolder);
        ClearFolder(config.outFolder);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("指定文件夹 自动 ASE 加密"))
        {
            AESEcypt();
            Refresh();
        }

        if (GUILayout.Button("选择性 ASE 加密"))
        {
            SelectionEcypt();
            Refresh();
        }
        
        if (GUILayout.Button("选择解密输出文件夹"))
        {
            config.outDesFolder = EditorUtility.OpenFolderPanel("选择解密输出文件夹", "", "");
        }
        GUILayout.BeginHorizontal();
        config.outDesFolder = GUILayout.TextField(config.outDesFolder);
        ClearFolder(config.outDesFolder);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("ASE 解密"))
        {
            AESDcypt();
            Refresh();
        }

        if (!string.IsNullOrEmpty(config.inputFolder))
        {
            if (Directory.Exists(config.inputFolder))
            {
                var files = Directory.GetFiles(config.inputFolder);
                
                GUILayout.Label("要加密文件列表");

                offset = EditorGUILayout.BeginScrollView(offset);
                GUILayout.BeginVertical();
                
                foreach (var file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (SkipFile(file, fileInfo))
                    {
                        GUILayout.Label(file);
                    }
                 
                }
                
                GUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
        }
    }

    private static void Refresh()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    bool SkipFile(string filepath,FileInfo fileInfo)
    {     
      
        if (filepath.Contains("."))
            return false;
        if (fileInfo.Length < 1024)
             return false;
        return true;
    }
    
    private void AESEcypt()
    {
        var files = Directory.GetFiles(config.inputFolder);
        foreach (var file in files)
        {
            FileInfo fileinfo = new FileInfo(file);
            if (SkipFile(file, fileinfo))
            {
                string filename = fileinfo.Name;
                EncryptionUtils.EncryptFile(file,config.outFolder + "/" + filename, config.keys , config.iv);
            }

        }
    }

    void SelectionEcypt()
    {
        var abs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        foreach (var assetBundle in abs)
        {
            Debug.Log(assetBundle);
            string objpath = AssetDatabase.GetAssetPath(assetBundle);
            
            if(Directory.Exists(objpath))
                continue;
            if(assetBundle.name.Contains("manifest"))
                continue;
            
            string path = objpath.Replace("Assets/","");
            string realpath = Application.dataPath  + "/" +  path;
            Debug.Log(realpath);
            string outpath = config.outFolder + "/" + assetBundle.name;
            EncryptionUtils.EncryptFile(realpath,outpath, config.keys , config.iv);
        }
    }
    
    private void AESDcypt()
    {
        var files = Directory.GetFiles(config.outFolder);
        foreach (var file in files)
        {
            FileInfo fileinfo = new FileInfo(file);
            if (SkipFile(file, fileinfo))
            {
                string filename = fileinfo.Name;
                EncryptionUtils.DecryptFile(file,config.outDesFolder + "/" + filename, config.keys , config.iv);
            }

        }
    }

    private void SelectionDcypt()
    {
        var abs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        foreach (var assetBundle in abs)
        {
        
            string objpath = AssetDatabase.GetAssetPath(assetBundle);
            if(Directory.Exists(objpath))
                continue;
            string path = objpath.Replace("Assets/","");
            string realpath = Application.dataPath  + "/" +  path;
            string outpath = config.outDesFolder +  "/" + assetBundle.name;
            EncryptionUtils.DecryptFile(realpath,outpath, config.keys , config.iv);
        }

    }
}
