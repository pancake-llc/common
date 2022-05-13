
namespace Pancake.Editor.Finder
{
	using UnityEditor.IMGUI.Controls;

	internal class ListTreeViewItem<T> : FinderTreeViewItem<T> where T : TreeItem
	{
		internal ListTreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName, data)
		{

		}
	}

	internal class ListTreeView<T> : FinderTreeView<T> where T : TreeItem
	{
		public ListTreeView(TreeViewState state, TreeModel<T> model) : base(state, model)
		{

		}

		public ListTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model) : base(state, multiColumnHeader, model)
		{

		}

		protected override TreeViewItem GetNewTreeViewItemInstance(int id, int depth, string name, T data)
		{
			return new ListTreeViewItem<T>(id, depth, name, data);
		}

		protected override void SortByMultipleColumns()
		{
			return;
		}
	}
}