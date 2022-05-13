
namespace Pancake.Editor.Finder
{
	using System.Collections.Generic;

	internal class TreeConjunction
	{
		public readonly List<ProjectReferenceItem> treeElements = new List<ProjectReferenceItem>();
		public AssetInfo referencedAsset;
		public ReferencedAtInfo referencedAtInfo;
	}
}