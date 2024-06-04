using HappyCard.Entities;
using HayypCard.Entities;
using HayypCard.Enums;
using UnityEditor;
using UnityEngine;

namespace HayypCard.Editor
{
    public sealed class GameDataEditor : EditorWindow
    {
        private GameDataType _gameDataType;
        private PropType _propType;
        private string _saveDirectory = "Assets/Game Data/";
        private string _fileName;

        [MenuItem("HappyCard/GameDataEditor")]
        public static void OpenWindow()
        {
            var window = GetWindow<GameDataEditor>("创建游戏数据");

            window.position = new Rect(320, 240, 300, 140);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("选择创建的游戏数据类型");
            _gameDataType = (GameDataType)EditorGUILayout.EnumPopup(_gameDataType);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if(_gameDataType == GameDataType.Product)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("商品名称");
                _fileName = EditorGUILayout.TextField(_fileName);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            else if(_gameDataType == GameDataType.PropInfo)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("选择创建的道具类型");
                _propType = (PropType)EditorGUILayout.EnumPopup(_propType);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            else if(_gameDataType == GameDataType.TaskInfo)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("任务名称");
                _fileName = EditorGUILayout.TextField(_fileName);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("保存目录");
            _saveDirectory = EditorGUILayout.TextField(_saveDirectory);
            EditorGUILayout.Space();

            if (GUILayout.Button("创建"))
            { 
                if(_gameDataType == GameDataType.Product)
                {
                    var p = ScriptableObject.CreateInstance<Product>();
                    p.name = _fileName;
                    AssetDatabase.CreateAsset(p,$"{_saveDirectory}{_fileName}.asset");
                    AssetDatabase.Refresh();
                }
                else if(_gameDataType == GameDataType.PropInfo)
                {
                    var p = ScriptableObject.CreateInstance<PropInfo>();
                    p.name = _propType.ToString();
                    p.PropType = _propType;
                    AssetDatabase.CreateAsset(p, $"{_saveDirectory}{p.name}.asset");
                    AssetDatabase.Refresh();
                }
                else if (_gameDataType == GameDataType.TaskInfo)
                {
                    var p = ScriptableObject.CreateInstance<TaskInfo>();
                    p.name = _fileName;
                    AssetDatabase.CreateAsset(p, $"{_saveDirectory}{p.name}.asset");
                    AssetDatabase.Refresh();
                }
            }
        }

    }

    public enum GameDataType
    {
        Product,
        PropInfo,
        TaskInfo
    }
}
