#region copyright

//---------------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov - focus [https://codestage.net]
//---------------------------------------------------------------

#endregion

namespace Pancake.Editor.Finder
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class FinderWindow : EditorWindow
    {
        private static FinderWindow windowInstance;
        [NonSerialized] private ReferencesTab _referencesTab;
        [NonSerialized] private bool _inited;

        public static FinderWindow Create()
        {
            windowInstance = GetWindow<FinderWindow>(false, "Finder", true);
            windowInstance.Focus();

            return windowInstance;
        }

        public static void ShowForScreenshot()
        {
            var window = Create();
            window.minSize = new Vector2(1024, 768);
        }

        public static void ShowAssetReferences()
        {
            UserSettings.Instance.referencesFinder.selectedTab = ReferenceFinderTab.Project;
            CreateImpl();
        }

        public static void ShowObjectReferences()
        {
            UserSettings.Instance.referencesFinder.selectedTab = ReferenceFinderTab.Scene;
            CreateImpl();
        }

        public static void ShowNotification(string text)
        {
            if (windowInstance) windowInstance.ShowNotification(new GUIContent(text));
        }

        public static void ClearNotification()
        {
            if (windowInstance) windowInstance.RemoveNotification();
        }

        public static void RepaintInstance()
        {
            if (windowInstance) windowInstance.Repaint();
        }

        private static FinderWindow CreateImpl()
        {
            windowInstance = Create();
            windowInstance.Refresh(true);
            RepaintInstance();
            return windowInstance;
        }

        private void Init()
        {
            if (_inited) return;
            CreateTabs();
            Repaint();
            Refresh(false);
            _inited = true;
        }

        private void CreateTabs()
        {
            if (_referencesTab == null) _referencesTab = new ReferencesTab(this);
        }

        public void Refresh(bool data) { _referencesTab.Refresh(data); }

        private void Awake() { EditorApplication.quitting += OnQuit; }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
            windowInstance = this;
            Init();
        }

        private void OnLostFocus() { ProjectSettings.Save(); }

        private void OnGUI()
        {
            UIHelpers.SetupStyles();
            UserSettings.Instance.scroll = GUILayout.BeginScrollView(UserSettings.Instance.scroll, false, false);
            _referencesTab.Draw();
            GUILayout.EndScrollView();
        }

        private void OnQuit() { ProjectSettings.Save(); }
    }
}