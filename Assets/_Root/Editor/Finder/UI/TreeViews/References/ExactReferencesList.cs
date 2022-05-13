
namespace Pancake.Editor.Finder
{
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;

	internal class ExactReferencesList<T> : ListTreeView<T> where T : HierarchyReferenceItem
	{
		public ExactReferencesList(TreeViewState state, TreeModel<T> model):base(state, model)
		{
		}

		protected override void PostInit()
		{
			showAlternatingRowBackgrounds = false;
			rowHeight = ROW_HEIGHT - 4;
		}

		protected override TreeViewItem GetNewTreeViewItemInstance(int id, int depth, string name, T data)
		{
			return new ExactReferencesListItem<T>(id, depth, name, data);
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			CenterRectUsingSingleLineHeight(ref args.rowRect);

			var item = (ExactReferencesListItem<T>)args.item;
			var lastRect = args.rowRect;
			lastRect.xMin += 4;

			if (item.data == null || item.data.reference == null)
			{
				GUI.Label(lastRect, item.displayName);
				return;
			}

			var entry = item.data.reference;
			Rect iconRect;
			
			if (entry.location == Location.NotFound)
			{
				iconRect = lastRect;
				iconRect.width = UIHelpers.WARNING_ICON_SIZE;
				iconRect.height = UIHelpers.WARNING_ICON_SIZE;

				GUI.DrawTexture(iconRect, EditorIcons.WarnSmallIcon, ScaleMode.ScaleToFit);
				lastRect.xMin += UIHelpers.WARNING_ICON_SIZE + UIHelpers.EYE_BUTTON_PADDING;
			}
			else if (entry.location == Location.Invisible)
			{
				iconRect = lastRect;
				iconRect.width = UIHelpers.WARNING_ICON_SIZE;
				iconRect.height = UIHelpers.WARNING_ICON_SIZE;

				GUI.DrawTexture(iconRect, EditorIcons.InfoSmallIcon, ScaleMode.ScaleToFit);
				lastRect.xMin += UIHelpers.WARNING_ICON_SIZE + UIHelpers.EYE_BUTTON_PADDING;
			}
			else
			{
				iconRect = lastRect;
				iconRect.width = UIHelpers.EYE_BUTTON_SIZE;
				iconRect.height = UIHelpers.EYE_BUTTON_SIZE;
				if (UIHelpers.IconButton(iconRect, Uniform.ShowIcon))
				{
					ShowItem(item);
				}
				lastRect.xMin += UIHelpers.EYE_BUTTON_SIZE + UIHelpers.EYE_BUTTON_PADDING;
			}

			var boxRect = iconRect;
			boxRect.height = lastRect.height;
			boxRect.xMin = iconRect.xMax;
			boxRect.xMax = lastRect.xMax;

			var label = entry.GetLabel();
			DefaultGUI.Label(lastRect, label, args.selected, args.focused);
		}

		protected override void ShowItem(TreeViewItem clickedItem)
		{
			var item = (ExactReferencesListItem<T>)clickedItem;

			var assetPath = item.data.AssetPath;
			var referencingEntry = item.data.Reference;

			SelectionTools.RevealAndSelectReferencingEntry(assetPath, referencingEntry);
		}
	}
}