
namespace Pancake.Editor.Finder
{
	internal class ExactReferencesListItem<T> : FinderTreeViewItem<T> where T : HierarchyReferenceItem
	{
		public ExactReferencesListItem(int id, int depth, string displayName, T data) : base(id, depth, displayName, data) { }

		protected override void Initialize()
		{
			if (depth == -1) return;
		}
	}
}