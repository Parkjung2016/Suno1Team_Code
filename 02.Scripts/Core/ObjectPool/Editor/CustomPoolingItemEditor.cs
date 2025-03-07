using ObjectPooling;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PoolingItemSO))]
public class CustomPoolingItemEditor : Editor
{
    private SerializedProperty _enumNameProp;
    private SerializedProperty _poolingNameProp;
    private SerializedProperty _descriptionProp;
    private SerializedProperty _poolCountProp;
    private SerializedProperty _prefabProp;

    private GUIStyle _textAreaStyle = null;

    private void OnEnable()
    {
        GUIUtility.keyboardControl = 0;
        _enumNameProp = serializedObject.FindProperty("enumName");
        _poolingNameProp = serializedObject.FindProperty("poolingName");
        _descriptionProp = serializedObject.FindProperty("description");
        _poolCountProp = serializedObject.FindProperty("poolCount");
        _prefabProp = serializedObject.FindProperty("prefab");
    }

    private void StyleSetup()
    {
        if (_textAreaStyle == null)
        {
            _textAreaStyle = new GUIStyle(EditorStyles.textArea);
            _textAreaStyle.wordWrap = true;
        }
    }

    public override void OnInspectorGUI()
    {
        StyleSetup();
        //디스크에있는 정보를 나한테 다시 직렬화해서 불러와라.
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal("HelpBox");
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    string prevName = _enumNameProp.stringValue;

                    EditorGUILayout.LabelField("EnumID", GUILayout.Width(60));
                    EditorGUILayout.DelayedTextField(_enumNameProp, GUIContent.none);

                    if (EditorGUI.EndChangeCheck())
                    {
                        //target 시리얼라이즈되지 않은 타겟 오브젝트를 말해.
                        string assetPath = AssetDatabase.GetAssetPath(target);
                        string newName = $"Pool_{_enumNameProp.stringValue}";
                        serializedObject.ApplyModifiedProperties();

                        string msg = AssetDatabase.RenameAsset(assetPath, newName);

                        if (string.IsNullOrEmpty(msg))
                        {
                            target.name = newName;
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        _enumNameProp.stringValue = prevName;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(_poolingNameProp);

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Description");
                    _descriptionProp.stringValue = EditorGUILayout.TextArea(
                        _descriptionProp.stringValue,
                        _textAreaStyle,
                        GUILayout.Height(60));
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("PoolSettings");
                    EditorGUILayout.PropertyField(_poolCountProp, GUIContent.none);
                    EditorGUILayout.PropertyField(_prefabProp, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}