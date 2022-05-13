

namespace Pancake.Editor.Finder
{
	using System;
	using UnityEngine.SceneManagement;

	[Serializable]
	internal class ReferencedAtOpenedSceneInfo : ReferencedAtInfo
	{
		public Scene openedScene = default(Scene);
	}
}