

namespace Pancake.Editor.Finder
{
	using System;
	using System.Collections.Generic;

	public class TextAssetParser : IDependenciesParser
	{
		public Type Type
		{
			get
			{
				return ReflectionTools.textAssetType;
			}
		}

		public List<string> GetDependenciesGUIDs(AssetKind kind, Type type, string path)
		{
			if (path.EndsWith(".cginc"))
			{
				// below is an another workaround for dependenciesGUIDs not include #include-ed files, like *.cginc
				return ShaderParser.ScanFileForIncludes(path);
			}

			return null;
		}
	}
}