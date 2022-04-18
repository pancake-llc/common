using System;
using System.Collections.Generic;

namespace Pancake.Editor
{
    [Serializable]
    public class UniformSettings
    {
        public Dictionary<string, bool> uppercaseSectionsFoldoutStates;

        public UniformSettings() { uppercaseSectionsFoldoutStates = new Dictionary<string, bool>(); }
    }
}