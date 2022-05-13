namespace Pancake.Editor.Finder
{
    using UnityEditor.AI;
    using UnityEngine;

    internal static class SettingsTools
    {
        public static Object GetInSceneLightmapSettings()
        {
            var mi = ReflectionTools.GetGetLightmapSettingsMethodInfo();
            if (mi != null)
            {
                return (Object) mi.Invoke(null, null);
            }

            Debug.LogError(Finder.ConstructError("Can't retrieve LightmapSettings object via reflection!"));
            return null;
        }

        public static Object GetInSceneRenderSettings()
        {
            var mi = ReflectionTools.GetGetRenderSettingsMethodInfo();
            if (mi != null)
            {
                return (Object) mi.Invoke(null, null);
            }

            Debug.LogError(Finder.ConstructError("Can't retrieve RenderSettings object via reflection!"));
            return null;
        }

        public static Object GetInSceneNavigationSettings() { return NavMeshBuilder.navMeshSettingsObject; }
    }
}