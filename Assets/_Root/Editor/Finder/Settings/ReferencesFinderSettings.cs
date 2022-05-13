namespace Pancake.Editor.Finder
{
    using System;

    /// <summary>
    /// References Finder module settings saved in ProjectSettings folder.
    /// </summary>
    /// Contains only filtering settings so far.
    [Serializable]
    public class ReferencesFinderSettings
    {
        public FilterItem[] pathIgnoresFilters = new FilterItem[0];
        public FilterItem[] pathIncludesFilters = new FilterItem[0];
    }
}