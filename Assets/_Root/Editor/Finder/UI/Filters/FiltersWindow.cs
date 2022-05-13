namespace Pancake.Editor.Finder
{
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    internal abstract class FiltersWindow : EditorWindow
    {
        internal delegate void TabChangeCallback(int newTab);

        private static bool needToRepaint;

        private event TabChangeCallback TabChangedCallback;

        private TabBase[] _tabs;
        private GUIContent[] _tabsCaptions;
        private TabBase _currentTab;
        private int _currentTabIndex;

        private Event _currentEvent;
        private EventType _currentEventType;

        protected void Init(string caption, TabBase[] windowTabs, int initialTab, TabChangeCallback tabChangeCallback)
        {
            titleContent = new GUIContent(caption + " Filters");

            minSize = new Vector2(600f, 300f);

            TabChangedCallback = tabChangeCallback;

            if (windowTabs != null && windowTabs.Length > 0)
            {
                _tabs = windowTabs;

                _currentTabIndex = windowTabs.Length > initialTab ? initialTab : 0;

                _currentTab = windowTabs[_currentTabIndex];
                _currentTab.Show(this);

                var captions = new GUIContent[windowTabs.Length];

                for (var i = 0; i < windowTabs.Length; i++)
                {
                    captions[i] = windowTabs[i].caption;
                }

                _tabsCaptions = captions;
            }
            else
            {
                Debug.LogError(Finder.LogPrefix + "no tabs were passed to the Filters Window!");
            }
        }

        protected abstract void InitOnEnable();
        protected abstract void UnInitOnDisable();

        protected virtual void OnGUI()
        {
            UIHelpers.SetupStyles();

            _currentEvent = Event.current;
            _currentEventType = _currentEvent.type;

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.BeginChangeCheck();
                _currentTabIndex = GUILayout.Toolbar(_currentTabIndex, _tabsCaptions, UIHelpers.richButton, GUILayout.Height(21));
                if (EditorGUI.EndChangeCheck())
                {
                    RemoveNotification();
                }

                _currentTab = _tabs[_currentTabIndex];
            }
            if (EditorGUI.EndChangeCheck())
            {
                _currentTab.Show(this);
                if (TabChangedCallback != null)
                {
                    TabChangedCallback.Invoke(_currentTabIndex);
                }
            }

            _currentTab.currentEvent = _currentEvent;
            _currentTab.currentEventType = _currentEventType;

            _currentTab.ProcessDrags();
            _currentTab.Draw();
        }

        [DidReloadScripts]
        private static void OnScriptsRecompiled() { needToRepaint = true; }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
            InitOnEnable();
        }

        private void OnDisable()
        {
            ProjectSettings.Save();
            UnInitOnDisable();
        }

        private void OnInspectorUpdate()
        {
            if (needToRepaint)
            {
                needToRepaint = false;
                Repaint();
            }
        }
    }
}