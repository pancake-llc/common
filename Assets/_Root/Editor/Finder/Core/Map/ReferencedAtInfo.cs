namespace Pancake.Editor.Finder
{
    using System;
    using UnityEditor;

    [Serializable]
    internal abstract class ReferencedAtInfo
    {
        public ReferencingEntryData[] entries;

        public void AddNewEntry(ReferencingEntryData newEntry)
        {
            if (entries == null)
            {
                entries = new[] {newEntry};
            }
            else
            {
                ArrayUtility.Add(ref entries, newEntry);
            }
        }
    }
}