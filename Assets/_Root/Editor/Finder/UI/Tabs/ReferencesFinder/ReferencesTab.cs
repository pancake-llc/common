

namespace Pancake.Editor.Finder
{
	using UnityEngine;
	using System;
	using UnityEditor;

	public enum ReferenceFinderTab
	{
		Project = 0,
		Scene = 1,
	}

	internal class ReferencesTab : TwoColumnsTab
	{
		[NonSerialized]
		private ReferenceFinderTab _currentTab;

		[NonSerialized]
		private readonly GUIContent[] _tabsCaptions;

		[NonSerialized]
		private readonly ProjectReferencesTab _projectTab;

		[NonSerialized]
		private readonly HierarchyReferencesTab _hierarchyTab;

		protected override string CaptionName
		{
			get { return ReferencesFinder.ModuleName; }
		}

		protected override Texture CaptionIcon
		{
			get { return Uniform.FindIcon; }
		}

		public ReferencesTab(FinderWindow window) : base(window)
		{
			_projectTab = new ProjectReferencesTab(window);

			_hierarchyTab = new HierarchyReferencesTab(window);

			_tabsCaptions = new[] { _projectTab.Caption, _hierarchyTab.Caption };
		}

		public override void Refresh(bool newData)
		{
			base.Refresh(newData);

			_currentTab = UserSettings.Instance.referencesFinder.selectedTab;

			switch (_currentTab)
			{
				case ReferenceFinderTab.Project:
					_projectTab.Refresh(newData);
					break;
				case ReferenceFinderTab.Scene:
					_hierarchyTab.Refresh(newData);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void DrawLeftColumnHeader()
		{
			using (new GUILayout.VerticalScope())
			{
				GUILayout.Space(10);

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);
					GUILayout.Label("<size=16><b>Search scope</b></size>", UIHelpers.richLabel);
					GUILayout.FlexibleSpace();

					using (new GUILayout.VerticalScope())
					{
						/*GUILayout.Space(-3);*/
						//if (UIHelpers.ImageButton(null, CSIcons.HelpOutline, GUILayout.ExpandWidth(false)))
						if (UIHelpers.IconButton(Uniform.HelpOutlineIcon, GUILayout.ExpandWidth(false)))
						{
							// TODO: update
							EditorUtility.DisplayDialog(ReferencesFinder.ModuleName + " scopes help",
								"Use " + _projectTab.Caption.text + " scope to figure out where any specific asset is referenced in whole project.\n\n" +
								"Use " + _hierarchyTab.Caption.text + " scope to figure out where any specific Game Object or component is referenced in active scene or opened prefab.",
								"OK");
						}
					}
					GUILayout.Space(10);
				}

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);
					UIHelpers.Separator();
					GUILayout.Space(10);
				}

				GUILayout.Space(10);

				EditorGUI.BeginChangeCheck();
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);
					_currentTab = (ReferenceFinderTab)GUILayout.SelectionGrid((int)_currentTab, _tabsCaptions, 1,
						GUILayout.Height(56), GUILayout.ExpandWidth(true));
					GUILayout.Space(10);
				}

				if (EditorGUI.EndChangeCheck())
				{
					UserSettings.Instance.referencesFinder.selectedTab = _currentTab;
					Refresh(false);
				}

				switch (_currentTab)
				{
					case ReferenceFinderTab.Project:
						_projectTab.DrawLeftColumnHeader();
						break;
					case ReferenceFinderTab.Scene:
						_hierarchyTab.DrawLeftColumnHeader();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			base.DrawLeftColumnHeader();
		}

		protected override void DrawLeftColumnBody()
		{
			using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
			{
				switch (_currentTab)
				{
					case ReferenceFinderTab.Project:
						_projectTab.DrawSettings();
						break;
					case ReferenceFinderTab.Scene:
						_hierarchyTab.DrawSettings();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		protected override bool DrawRightColumnCenter()
		{
			switch (_currentTab)
			{
				case ReferenceFinderTab.Project:
					_projectTab.DrawRightColumn();
					break;
				case ReferenceFinderTab.Scene:
					_hierarchyTab.DrawRightColumn();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return true;
		}

		protected override void DrawRightColumnBottom()
		{
			switch (_currentTab)
			{
				case ReferenceFinderTab.Project:
					_projectTab.DrawFooter();
					break;
				case ReferenceFinderTab.Scene:
					_hierarchyTab.DrawFooter();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
