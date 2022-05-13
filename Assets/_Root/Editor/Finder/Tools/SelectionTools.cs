namespace Pancake.Editor.Finder
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;


    internal static class SelectionTools
    {
        public static bool RevealAndSelectFileAsset(string assetPath)
        {
            var instanceId = AssetTools.GetMainAssetInstanceID(assetPath);
            if (AssetDatabase.Contains(instanceId))
            {
                Selection.activeInstanceID = instanceId;
                EditorGUIUtility.PingObject(Selection.activeObject);
                return true;
            }

            return false;
        }

        public static bool RevealAndSelectSubAsset(string assetPath, string name, long objectId)
        {
            var targetAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            if (targetAssets == null || targetAssets.Length == 0) return false;

            foreach (var targetAsset in targetAssets)
            {
                if (!AssetDatabase.IsSubAsset(targetAsset)) continue;
                if (targetAsset is GameObject || targetAsset is Component) continue;
                if (!string.Equals(targetAsset.name, name, StringComparison.OrdinalIgnoreCase)) continue;

                var assetId = ObjectTools.GetUniqueObjectId(targetAsset);
                if (assetId != objectId) continue;

                Selection.activeInstanceID = targetAsset.GetInstanceID();
                EditorGUIUtility.PingObject(targetAsset);
                return true;
            }

            return false;
        }

        public static void RevealAndSelectReferencingEntry(string assetPath, ReferencingEntryData referencingEntry)
        {
            if (!string.IsNullOrEmpty(assetPath) &&
                (referencingEntry.location == Location.SceneLightingSettings || referencingEntry.location == Location.SceneNavigationSettings))
            {
                var sceneOpenResult = SceneTools.OpenSceneWithSavePrompt(assetPath);
                if (!sceneOpenResult.success)
                {
                    Debug.LogError(Finder.ConstructError("Can't open scene " + assetPath));
                    FinderWindow.ShowNotification("Can't show it properly");
                    return;
                }
            }

            switch (referencingEntry.location)
            {
                case Location.ScriptAsset:
                case Location.ScriptableObjectAsset:

                    if (!RevealAndSelectFileAsset(assetPath))
                    {
                        FinderWindow.ShowNotification("Can't show it properly");
                    }

                    break;
                case Location.PrefabAssetObject:
                    if (!RevealAndSelectSubAsset(assetPath, referencingEntry.transformPath, referencingEntry.objectId))
                    {
                        FinderWindow.ShowNotification("Can't show it properly");
                    }

                    break;
                case Location.PrefabAssetGameObject:
                case Location.SceneGameObject:

                    if (!RevealAndSelectGameObject(assetPath, referencingEntry.transformPath, referencingEntry.objectId, referencingEntry.componentId))
                    {
                        FinderWindow.ShowNotification("Can't show it properly");
                    }

                    break;

                case Location.SceneLightingSettings:

                    if (!MenuTools.ShowSceneSettingsLighting())
                    {
                        Debug.LogError(Finder.ConstructError("Can't open Lighting settings!"));
                        FinderWindow.ShowNotification("Can't show it properly");
                    }

                    break;

                case Location.SceneNavigationSettings:

                    if (!MenuTools.ShowSceneSettingsNavigation())
                    {
                        Debug.LogError(Finder.ConstructError("Can't open Navigation settings!"));
                        FinderWindow.ShowNotification("Can't show it properly");
                    }

                    break;

                case Location.NotFound:
                case Location.Invisible:
                    break;

                case Location.TileMap:

                    if (!RevealAndSelectGameObject(assetPath, referencingEntry.transformPath, referencingEntry.objectId, referencingEntry.componentId))
                    {
                        FinderWindow.ShowNotification("Can't show it properly");
                    }

                    // TODO: open tile map editor window?

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool RevealAndSelectGameObject(string assetPath, string transformPath, long objectId, long componentId)
        {
            var enclosingAssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (enclosingAssetType == ReflectionTools.sceneAssetType || string.IsNullOrEmpty(assetPath))
            {
                return RevealAndSelectGameObjectInScene(assetPath, transformPath, objectId, componentId);
            }

            return RevealAndSelectGameObjectInPrefab(assetPath, transformPath, objectId, componentId);
        }

        public static SceneTools.OpenSceneResult OpenSceneForReveal(string path)
        {
            var result = SceneTools.OpenScene(path);
            if (result.success)
            {
                SceneTools.CloseUntitledSceneIfNotDirty();

                if (EditorTools.lastRevealSceneOpenResult != null)
                {
                    if (SceneTools.IsOpenedSceneNeedsToBeClosed(EditorTools.lastRevealSceneOpenResult, path, true))
                    {
                        if (EditorTools.lastRevealSceneOpenResult.scene.isDirty)
                        {
                            if (!SceneTools.SaveDirtyScenesWithPrompt(new[] {EditorTools.lastRevealSceneOpenResult.scene}))
                            {
                                return new SceneTools.OpenSceneResult();
                            }
                        }
                    }

                    SceneTools.CloseOpenedSceneIfNeeded(EditorTools.lastRevealSceneOpenResult, path, true);
                }

                EditorTools.lastRevealSceneOpenResult = result;
            }

            return result;
        }

        public static bool TryFoldAllComponentsExceptId(long componentId)
        {
            var tracker = EditorTools.GetActiveEditorTrackerForSelectedObject();
            if (tracker == null)
            {
                Debug.LogError(Finder.ConstructError("Can't get active tracker."));
                return false;
            }

            tracker.RebuildIfNecessary();

            var editors = tracker.activeEditors;
            if (editors.Length > 1)
            {
                var targetFound = false;
                var skipCount = 0;

                for (var i = 0; i < editors.Length; i++)
                {
                    var editor = editors[i];
                    var editorTargetType = editor.target.GetType();
                    if (editorTargetType == ReflectionTools.assetImporterType || editorTargetType == ReflectionTools.gameObjectType)
                    {
                        skipCount++;
                        continue;
                    }

                    if (i - skipCount == componentId)
                    {
                        targetFound = true;

                        /* known corner cases when editor can't be set to visible via tracker */

                        if (editor.serializedObject.targetObject is ParticleSystemRenderer)
                        {
                            var renderer = (ParticleSystemRenderer) editor.serializedObject.targetObject;
                            var ps = renderer.GetComponent<ParticleSystem>();
                            componentId = ComponentTools.GetComponentIndex(ps);
                        }

                        break;
                    }
                }

                if (!targetFound)
                {
                    return false;
                }

                for (var i = 1; i < editors.Length; i++)
                {
                    tracker.SetVisible(i, i - skipCount != componentId ? 0 : 1);
                }

                var inspectorWindow2 = EditorTools.GetInspectorWindow();
                if (inspectorWindow2 != null)
                {
                    inspectorWindow2.Repaint();
                }

                // workaround for bug when tracker selection gets reset after scene open
                // (e.g. revealing TMP component in new scene)
                EditorApplication.delayCall += () =>
                {
                    EditorApplication.delayCall += () =>
                    {
                        try
                        {
                            for (var i = 0; i < editors.Length; i++)
                            {
                                tracker.SetVisible(i, i - skipCount != componentId ? 0 : 1);
                            }

                            var inspectorWindow = EditorTools.GetInspectorWindow();
                            if (inspectorWindow != null)
                            {
                                inspectorWindow.Repaint();
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    };
                };
            }

            return true;
        }

        private static bool RevealAndSelectGameObjectInScene(string path, string transformPath, long objectId, long componentId)
        {
            Scene targetScene;

            if (!string.IsNullOrEmpty(path))
            {
                var openResult = OpenSceneForReveal(path);
                if (!openResult.success)
                    return false;

                targetScene = openResult.scene;
            }
            else
            {
                targetScene = SceneTools.GetUntitledScene();
            }

            if (!targetScene.IsValid())
            {
                Debug.LogError(Finder.ConstructError("Target scene is not valid or not found! Scene path: " + path + ", looked for ObjectID " + objectId + "!"));
                return false;
            }

            var target = ObjectTools.FindGameObjectInScene(targetScene, objectId, transformPath);

            if (target == null)
            {
                Debug.LogError(Finder.ConstructError("Couldn't find target Game Object " + transformPath + " at " + path + " with ObjectID " + objectId + "!"));
                return false;
            }

            // workaround for a bug when Unity doesn't expand hierarchy in scene
            EditorApplication.delayCall += () => { EditorGUIUtility.PingObject(target); };

            ObjectTools.SelectGameObject(target, true);

            var enclosingAssetInstanceId = AssetTools.GetMainAssetInstanceID(path);
            EditorApplication.delayCall += () => { EditorGUIUtility.PingObject(enclosingAssetInstanceId); };

            if (componentId != -1)
            {
                return TryFoldAllComponentsExceptId(componentId);
            }

            return true;
        }

        private static bool RevealAndSelectGameObjectInPrefab(string path, string transformPath, long objectId, long componentId)
        {
            /*Debug.Log("LOOKING FOR objectId " + objectId);
            Debug.Log("path " + path);*/

            var targetAsset = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
            var prefabType = PrefabUtility.GetPrefabAssetType(targetAsset);

            GameObject target;

            if (prefabType == PrefabAssetType.Model)
            {
                target = targetAsset;
            }
            else
            {
                var prefabNeedsToBeOpened = true;

                var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
#if UNITY_2020_1_OR_NEWER
                    if (prefabStage.assetPath == path)
#else
					if (prefabStage.prefabAssetPath == path)
#endif
                    {
                        prefabNeedsToBeOpened = false;
                    }
                }

                if (prefabNeedsToBeOpened && !AssetDatabase.OpenAsset(targetAsset))
                {
                    Debug.LogError(Finder.ConstructError("Couldn't open prefab at " + path + "!"));
                    return false;
                }

                prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null)
                {
                    Debug.LogError(Finder.ConstructError("Couldn't get prefab stage for prefab at " + path + "!"));
                    return false;
                }

                target = prefabStage.prefabContentsRoot.transform.root.gameObject;
            }

            if (target == null)
            {
                Debug.LogError(Finder.ConstructError("Couldn't find target Game Object " + transformPath + " at " + path + " with ObjectID " + objectId + "!"));
                return false;
            }

            target = ObjectTools.FindChildGameObjectRecursive(target.transform, objectId, target.transform.name, transformPath);

            EditorApplication.delayCall += () =>
            {
                ObjectTools.SelectGameObject(target, false);
                EditorGUIUtility.PingObject(targetAsset);

                if (componentId != -1)
                {
                    EditorApplication.delayCall += () => { TryFoldAllComponentsExceptId(componentId); };
                }
            };

            return true;
        }
    }
}