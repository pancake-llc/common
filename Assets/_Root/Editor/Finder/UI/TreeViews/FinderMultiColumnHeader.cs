
namespace Pancake.Editor.Finder
{
	using System;
	using UnityEditor.IMGUI.Controls;

	internal class FinderMultiColumnHeader : MultiColumnHeader
	{
		private HeaderMode _mode;

		public enum HeaderMode
		{
			LargeHeader,
			DefaultHeader,
			MinimumHeaderWithoutSorting
		}

		public FinderMultiColumnHeader(MultiColumnHeaderState state) : base(state)
		{
			Mode = HeaderMode.DefaultHeader;
		}

		public HeaderMode Mode
		{
			get { return _mode; }
			set
			{
				_mode = value;
				switch (_mode)
				{
					case HeaderMode.LargeHeader:
						canSort = true;
						height = 37f;
						break;
					case HeaderMode.DefaultHeader:
						canSort = true;
						height = DefaultGUI.defaultHeight;
						break;
					case HeaderMode.MinimumHeaderWithoutSorting:
						canSort = false;
						height = DefaultGUI.minimumHeight;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		// EXAMPLE FOR FUTURE REFERENCE
		/*protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
		{
			base.ColumnHeaderGUI(column, headerRect, columnIndex);

			if (Mode == HeaderMode.LargeHeader)
			{
				if (columnIndex > 2)
				{
					headerRect.xMax -= 3f;
					var oldAlignment = EditorStyles.largeLabel.alignment;
					EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
					GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
					EditorStyles.largeLabel.alignment = oldAlignment;
				}
			}
		}*/
	}
}