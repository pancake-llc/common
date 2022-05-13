#if UNITY_2019_2_OR_NEWER



namespace Pancake.Editor.Finder
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.Compilation;
	using UnityEngine;

	public class AssemblyDefinitionReferenceParser : IDependenciesParser
	{
		public Type Type
		{
			get
			{
				return ReflectionTools.assemblyDefinitionReferenceAssetType;
			}
		}
		
		public List<string> GetDependenciesGUIDs(AssetKind kind, Type type, string path)
		{
			if (kind != AssetKind.Regular)
			{
				return null;
			}
			
			return GetAssetsReferencedFromAssemblyDefinitionReference(path);
		}
		
		private List<string> GetAssetsReferencedFromAssemblyDefinitionReference(string assetPath)
		{
			var result = new List<string>();

			var asset = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionReferenceAsset>(assetPath);
			var data = JsonUtility.FromJson<AssemblyDefinitionReferenceData>(asset.text);

			if (!string.IsNullOrEmpty(data.reference))
			{
				var assemblyDefinitionPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyReference(data.reference);
				if (!string.IsNullOrEmpty(assemblyDefinitionPath))
				{
					assemblyDefinitionPath = PathTools.EnforceSlashes(assemblyDefinitionPath);
					var guid = AssetDatabase.AssetPathToGUID(assemblyDefinitionPath);
					if (!string.IsNullOrEmpty(guid))
					{
						result.Add(guid);
					}
				}
			}

			data.reference = null;

			return result;
		}

		private class AssemblyDefinitionReferenceData
		{
			public string reference;
		}
	}
}
#endif