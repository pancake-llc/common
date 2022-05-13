

namespace Pancake.Editor.Finder
{
	using System;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEngine;

	internal abstract class StringFiltersTab : TabBase
	{
		internal delegate void SaveFiltersCallback(FilterItem[] filters);
		internal delegate FilterItem[] GetDefaultsCallback();

		protected FilterItem[] filters;
		protected SaveFiltersCallback saveFiltersCallback;

		protected bool didFocus;

		private readonly GetDefaultsCallback _defaultsCallback;

		private string _newItemText = "";
		private FilterKind _newItemKind = FilterKind.Path;
		private bool _newItemIgnoreCase;

		protected StringFiltersTab(FilterType filterType, FilterItem[] filters, SaveFiltersCallback saveFiltersCallback, GetDefaultsCallback defaultsCallback = null) :base(filterType)
		{
			this.filters = filters;
			this.saveFiltersCallback = saveFiltersCallback;
			this._defaultsCallback = defaultsCallback;
		}

		internal override void Show(FiltersWindow hostingWindow)
		{
			base.Show(hostingWindow);

			_newItemText = "";
			didFocus = false;
		}

		protected override void DrawTabContents()
		{
			DrawTabHeader();

			GUILayout.Space(5);
			UIHelpers.Separator();
			GUILayout.Space(5);

			DrawAddItemSection();

			GUILayout.Space(5);
			UIHelpers.Separator();
			GUILayout.Space(5);

			DrawFiltersList();
		}

		protected virtual void DrawAddItemSection()
		{
			EditorGUILayout.LabelField(GetAddNewLabel());
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(6);
				GUI.SetNextControlName("AddButton");

				var flag = currentEvent.isKey && Event.current.type == EventType.KeyDown && (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter);
				if (UIHelpers.IconButton(Uniform.PlusIcon, "Adds custom filter to the list.") || flag)
				{
					if (string.IsNullOrEmpty(_newItemText))
					{
						window.ShowNotification(new GUIContent("You can't add an empty filter!"));
					}
					else if (_newItemText.IndexOf('*') != -1)
					{
						window.ShowNotification(new GUIContent("Masks are not supported!"));
					}
					else
					{
						if (_newItemKind == FilterKind.Extension && !_newItemText.StartsWith("."))
						{
							_newItemText = "." + _newItemText;
						}

						if (CheckNewItem(ref _newItemText))
						{
							if (FilterTools.TryAddNewItemToFilters(ref filters, FilterItem.Create(_newItemText, _newItemKind, _newItemIgnoreCase)))
							{
								SaveChanges();
								_newItemText = "";
								GUI.FocusControl("AddButton");
								didFocus = false;
							}
							else
							{
								window.ShowNotification(new GUIContent("This filter already exists in the list!"));
							}
						}
					}
				}
				if (flag)
				{
					currentEvent.Use();
					currentEvent.Use();
				}

				_newItemKind = DrawFilterKindDropdown(_newItemKind);
				_newItemIgnoreCase = DrawFilterIgnoreCaseToggle(_newItemIgnoreCase);
				GUILayout.Space(5);

				GUI.SetNextControlName("filtersTxt");
				_newItemText = EditorGUILayout.TextField(_newItemText);
				if (!didFocus)
				{
					didFocus = true;
					EditorGUI.FocusTextInControl("filtersTxt");
				}
			}
		}

		protected virtual void DrawFiltersList()
		{
			if (filters == null) return;

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			foreach (var filter in filters)
			{
				using (new GUILayout.HorizontalScope(UIHelpers.panelWithBackground))
				{
					if (UIHelpers.IconButton(Uniform.MinusIcon, "Removes filter from the list."))
					//if (GUILayout.Button("<b><color=#FF4040>X</color></b>", UIHelpers.compactButton, GUILayout.ExpandWidth(false)))
					{
						ArrayUtility.Remove(ref filters, filter);
						SaveChanges();
					}
					DrawFilterKindLabel(filter.kind);
					DrawFilterIgnoreCaseLabel(filter.ignoreCase);
					GUILayout.Space(5);
					if (GUILayout.Button(filter.value, UIHelpers.richLabel))
					{
						_newItemText = filter.value;
						_newItemIgnoreCase = filter.ignoreCase;
						_newItemKind = filter.kind;
						GUI.FocusControl("AddButton");
						didFocus = false;
					}
				}
			}
			GUILayout.EndScrollView();

			if (filters.Length > 0)
			{
				if (UIHelpers.ImageButton("Clear All " + caption.text, "Removes all added filters from the list.", Uniform.ClearIcon))
				{
					var cleanCaption = Regex.Replace(caption.text, @"<[^>]*>", string.Empty);
					if (EditorUtility.DisplayDialog("Clearing the " + cleanCaption + " list",
						"Are you sure you wish to clear all the filters in the " + cleanCaption + " list?",
						"Yes", "No"))
					{
						Array.Resize(ref filters, 0);
						SaveChanges();
					}
				}
			}

			if (_defaultsCallback != null)
			{
				if (UIHelpers.ImageButton("Reset to Defaults", Uniform.RestoreIcon))
				{
					filters = _defaultsCallback();
					SaveChanges();
				}
			}
		}

		protected void SaveChanges()
		{
			if (saveFiltersCallback != null)
			{
				saveFiltersCallback(filters);
			}
		}

		protected virtual string GetAddNewLabel()
		{
			return "Add new filter to the list:";
		}

		protected virtual FilterKind DrawFilterKindDropdown(FilterKind kind)
		{
			return kind;
		}

		protected virtual bool DrawFilterIgnoreCaseToggle(bool ignore)
		{
			return ignore;
		}

		protected virtual void DrawFilterKindLabel(FilterKind kind)
		{
		}

		protected virtual void DrawFilterIgnoreCaseLabel(bool ignore)
		{
		}

		protected abstract void DrawTabHeader();

		protected abstract bool CheckNewItem(ref string newItem);
	}
}