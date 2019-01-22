using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ModelComponentEditor(typeof(ModelTags))]
public class ModelTagsEditor : BaseModelComponentEditor
{
	private string _currentTagAddString = "";

	public override void OnGUI(BaseModelComponent component)
	{
		ModelTags tagsComponent = component as ModelTags;

		string[] tags = tagsComponent.GetTags();
		EditorGUILayout.BeginHorizontal();
		_currentTagAddString = EditorGUILayout.TextField("Add Tag: ", _currentTagAddString);
		if(GUILayout.Button("+", GUILayout.Width(40)))
		{
			tagsComponent.AddTag(_currentTagAddString);
			_currentTagAddString = string.Empty;
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
}
