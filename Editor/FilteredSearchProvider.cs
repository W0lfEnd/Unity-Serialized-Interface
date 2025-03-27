using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializableInterface.Editor
{
    public class FilteredSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private Action<Object> _onSelect;
        private Type _baseType;
        private Type[] _interfaceTypes;

        public static void OpenSearchWindow(Type baseType, Type interfaceType, Action<Object> onSelect)
        {
            OpenSearchWindow(baseType, new[] { interfaceType }, onSelect);
        }

        public static void OpenSearchWindow(Type baseType, Type[] interfaceTypes, Action<Object> onSelect)
        {
            var provider = CreateInstance<FilteredSearchProvider>();
            provider.Init(baseType, interfaceTypes, onSelect);

            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 600), provider);
        }

        public void Init(Type baseType, Type[] interfaceTypes, Action<Object> onSelect)
        {
            _baseType = baseType;
            _interfaceTypes = interfaceTypes;
            _onSelect = onSelect;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent($"Select {_baseType.Name}"))
            };

            var allObjects = AssetDatabase.FindAssets("t:" + _baseType.Name)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Object>)
                .Where(obj => obj != null && (_interfaceTypes.Length == 0 || _interfaceTypes.Any(filter => filter.IsAssignableFrom(obj.GetType()))))
                .ToArray();

            foreach (var obj in allObjects)
            {
                var contentWithIcon = new GUIContent(obj.name, EditorGUIUtility.ObjectContent(obj, obj.GetType()).image);
                var entry = new SearchTreeEntry(contentWithIcon)
                {
                    level = 1,
                    userData = obj
                };
                searchTree.Add(entry);
            }

            if (allObjects.Length == 0)
            {
                searchTree.Add(new SearchTreeEntry(new GUIContent("No valid objects found")) { level = 1 });
            }

            return searchTree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is Object selectedObject)
            {
                _onSelect?.Invoke(selectedObject);
                return true;
            }

            return false;
        }
    }
}