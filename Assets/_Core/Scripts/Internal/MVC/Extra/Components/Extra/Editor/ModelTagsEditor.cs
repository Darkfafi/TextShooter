using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ModelComponentEditor(typeof(ModelTags))]
public class ModelTagsEditor : ModelComponentEditor
{
	private string _currentTagAddString = "";
	private SearchWindow _openSearchWindow = null;

	public override void OnGUI(BaseModelComponent component)
	{
		ModelTags tagsComponent = component as ModelTags;

		string[] tags = tagsComponent.GetTags();
		EditorGUILayout.BeginHorizontal();
		_currentTagAddString = EditorGUILayout.TextField("Add Tag: ", _currentTagAddString);
		if(GUILayout.Button("+", GUILayout.Width(30)))
		{
			tagsComponent.AddTag(_currentTagAddString);
			_currentTagAddString = string.Empty;
		}

		if(GUILayout.Button("search"))
		{
			Type[] tagHolders = Assembly.GetAssembly(typeof(ModelTags)).GetTypes().Where(t => t.GetCustomAttributes(typeof(TagsHolderAttribute), true).Length > 0 && t.IsClass).ToArray();
			List<string> allTags = new List<string>();
			foreach(Type tagsHolder in tagHolders)
			{
				FieldInfo[] fields = tagsHolder.GetFields(BindingFlags.Public | BindingFlags.Static).Where(t => t.GetCustomAttributes(typeof(TagFieldAttribute), true).Length > 0 && t.IsLiteral && !t.IsInitOnly).ToArray();
				foreach(FieldInfo f in fields)
				{
					object v = f.GetValue(null);
					if(v != null)
					{
						if(tagsComponent != null && tagsComponent.HasTag(v.ToString()))
							continue;

						allTags.Add(v.ToString());
					}
				}
			}
			_openSearchWindow = SearchWindow.OpenWindow((index) =>
			{
				if(index >= 0)
				{
					if(tagsComponent != null)
					{
						tagsComponent.AddTag(allTags[index]);
					}

					if(_openSearchWindow != null)
					{
						_openSearchWindow.Close();
						_openSearchWindow = null;
					}
				}
			}, allTags.ToArray());
		}

		EditorGUILayout.EndHorizontal();

		for(int i = 0; i < tags.Length; i++)
		{
			EditorGUILayout.BeginHorizontal();
			GUIStyle s = new GUIStyle(GUI.skin.label);
			s.normal.textColor = new Color(0.5f, 0.3f, 0.6f);
			GUILayout.Label(" * " + tags[i], s);
			s = new GUIStyle(GUI.skin.button);
			s.normal.textColor = Color.red;
			if(GUILayout.Button("x", s, GUILayout.Width(25)))
			{
				tagsComponent.RemoveTag(tags[i]);
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();
		
	}

	protected override void OnClose()
	{
		if(_openSearchWindow != null)
		{
			_openSearchWindow.Close();
			_openSearchWindow = null;
		}
	}
}