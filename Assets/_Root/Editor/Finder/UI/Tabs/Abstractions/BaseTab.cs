

namespace Pancake.Editor.Finder
{
	using UnityEngine;

	internal abstract class BaseTab
	{
		protected readonly FinderWindow window;

		private GUIContent _caption;
		internal GUIContent Caption
		{
			get
			{
				if (_caption == null)
				{
					_caption = new GUIContent(CaptionName, CaptionIcon);
				}
				return _caption;
			}
		}

		protected abstract string CaptionName { get; }
		protected abstract Texture CaptionIcon { get; }

		protected BaseTab(FinderWindow window)
		{
			this.window = window;
		}
	}
}