using ObjectPooling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public enum WindowType
{
    Pool
}

public class UtilityWindow : EditorWindow
{
    private static int toolbarIndex = 0;
    private static Vector2 viewScrollPosition = Vector2.zero;

    private static Dictionary<WindowType, Vector2> scrollPosition
        = new Dictionary<WindowType, Vector2>();

    private static Dictionary<WindowType, Object> selectedItem
        = new Dictionary<WindowType, Object>();

    #region 데이터 테이블 영역

    private readonly string _poolDirectory = "Assets/00.Work/PJH/03.SO/ObjectPool";
    private PoolList _poolList = null;

    #endregion

    private string[] _toolbarIndexNames;
    private Editor _cachedEditor;
    private Texture2D _selectedBoxTexture;
    private GUIStyle _selectedBoxStyle;

    [MenuItem("Tools/Utility")]
    private static void OpenWindow()
    {
        UtilityWindow window = GetWindow<UtilityWindow>("Game Utility");
        window.minSize = new Vector2(700, 500);
        window.Show();
    }

    private void OnEnable()
    {
        SetUpUtility();
    }

    private void OnDisable()
    {
        DestroyImmediate(_selectedBoxTexture);
        DestroyImmediate(_cachedEditor);
    }

    private void SetUpUtility()
    {
        _selectedBoxTexture = new Texture2D(1, 1);
        _selectedBoxTexture.SetPixel(0, 0, new Color(0.31f, 0.40f, 0.50f));
        _selectedBoxTexture.Apply();

        _selectedBoxStyle = new GUIStyle();
        _selectedBoxStyle.normal.background = _selectedBoxTexture;

        _selectedBoxTexture.hideFlags = HideFlags.DontSave;

        _toolbarIndexNames = Enum.GetNames(typeof(WindowType));

        foreach (WindowType type in Enum.GetValues(typeof(WindowType)))
        {
            //이부분은 나중에 값이 없을 경우만 초기화하도록 변경해야해.
            scrollPosition[type] = Vector2.zero; //딕셔너리 초기화
            selectedItem[type] = null; //선택된 오브젝트가 없도록 셋업
        }


        if (_poolList == null)
        {
            Debug.Log(  $"{_poolDirectory}/table.asset");
            _poolList = AssetDatabase.LoadAssetAtPath<PoolList>(
                $"{_poolDirectory}/table.asset");
            if (_poolList == null)
            {
                //이건 메모리상에만 생성한거야.
                _poolList = ScriptableObject.CreateInstance<PoolList>();

                //이건 파일 생성 절대경로를 알아내는거다.
                string filename = AssetDatabase.GenerateUniqueAssetPath(
                    $"{_poolDirectory}/table.asset");
                AssetDatabase.CreateAsset(_poolList, filename);
                Debug.Log($"Pool list data genenrated at {filename}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    private void OnGUI()
    {
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, _toolbarIndexNames);
        EditorGUILayout.Space(8);

        switch (toolbarIndex)
        {
            case 0:
                DrawPoolingMenus();
                break;
        }
    }


    private void DrawPoolingMenus()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUI.color = new Color(0.19f, 0.76f, 0.08f);
            if (GUILayout.Button("Generate Item"))
            {
                Guid guid = Guid.NewGuid(); //고유한 스트링을 생성해준다.

                PoolingItemSO newData = CreateInstance<PoolingItemSO>();
                newData.enumName = guid.ToString();

                AssetDatabase.CreateAsset(newData,
                    $"{_poolDirectory}/Pool_{newData.enumName}.asset");
                _poolList.GetList().Add(newData);

                EditorUtility.SetDirty(_poolList);
                AssetDatabase.SaveAssets();
            }

            GUI.color = new Color(0.81f, 0.13f, 0.18f);
            if (GUILayout.Button("Generate Enum file"))
            {
                GeneratePoolingEnumFile();
            }
        }
        EditorGUILayout.EndHorizontal();


        GUI.color = Color.white; //컬러 원상복귀
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(300f));
            {
                EditorGUILayout.LabelField("PoolingType List");
                EditorGUILayout.Space(2f);

                scrollPosition[WindowType.Pool] = EditorGUILayout.BeginScrollView(scrollPosition[WindowType.Pool],
                    false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
                {
                    foreach (PoolingItemSO item in _poolList.GetList())
                    {
                        GUIStyle style = selectedItem[WindowType.Pool] == item ? _selectedBoxStyle : GUIStyle.none;
                        EditorGUILayout.BeginHorizontal(style, GUILayout.Height(40));
                        {
                            float labelWidth = 240f;
                            EditorGUILayout.LabelField(item.enumName,
                                GUILayout.Width(labelWidth),
                                GUILayout.Height(40f));
                        }

                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.Space(10f);

                            GUI.color = Color.red;
                            if (GUILayout.Button("X", GUILayout.Width(20f)))
                            {
                                _poolList.GetList().Remove(item);
                                AssetDatabase.DeleteAsset($"{_poolDirectory}/Pool_{item.enumName}.asset");
                                EditorUtility.SetDirty(_poolList);
                                AssetDatabase.SaveAssets();
                            }

                            GUI.color = Color.white;
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.EndHorizontal();

                        Rect lastRect = GUILayoutUtility.GetLastRect();

                        if (Event.current.type == EventType.MouseDown
                            && lastRect.Contains(Event.current.mousePosition))
                        {
                            viewScrollPosition = Vector2.zero;
                            selectedItem[WindowType.Pool] = item;
                            Event.current.Use();
                        }

                        if (item == null) break;
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            if (selectedItem[WindowType.Pool] != null)
            {
                viewScrollPosition = EditorGUILayout.BeginScrollView(viewScrollPosition);
                {
                    EditorGUILayout.Space(2f);
                    Editor.CreateCachedEditor(
                        selectedItem[WindowType.Pool], null, ref _cachedEditor);
                    _cachedEditor.OnInspectorGUI();
                }
                EditorGUILayout.EndScrollView();
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    private void GeneratePoolingEnumFile()
    {
        StringBuilder codeBuilder = new StringBuilder();
        foreach (PoolingItemSO item in _poolList.GetList())
        {
            codeBuilder.Append(item.enumName);
            codeBuilder.Append(",");
        }

        string code = string.Format(CodeFormat.PoolTypeFormat, codeBuilder.ToString());
        string path = $"{Application.dataPath}/00.Work/PJH/02.Scripts/Core/ObjectPool/PoolingType.cs";
        Debug.Log(path);
        File.WriteAllText(path, code);

        AssetDatabase.Refresh();
    }
}