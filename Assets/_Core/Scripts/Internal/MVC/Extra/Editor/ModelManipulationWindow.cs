﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ModelManipulationWindow : EditorWindow
{
    private MonoBaseView _targetView;
    private BaseModel _targetModel;
	
	private bool _showComponents = false;
	private Dictionary<Type, BaseModelComponentEditor> _editors = new Dictionary<Type, BaseModelComponentEditor>();
	private List<BaseModelComponent> _componentsEditorsOpen = new List<BaseModelComponent>();

    [MenuItem("MVC/Model Manipulator")]
    static void OpenWindow()
    {
        ModelManipulationWindow window = GetWindow<ModelManipulationWindow>("Model Manipulator", true);
        window.Show(true);
        window.Repaint();
    }

    protected void OnGUI()
    {
		SetupEditors();
        EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(0.5f, 0.5f, 0.5f));
        GameObject targetGameObject = Selection.activeGameObject;
        MonoBaseView monoBaseView = null;

        if (targetGameObject != null)
        {
            monoBaseView = targetGameObject.GetComponent<MonoBaseView>();
        }

        GUILayout.Label("Target Model Controll", EditorStyles.boldLabel);

        if(monoBaseView != null)
        {
            ShowInternalControllWindow(monoBaseView);
        }
        else
        {
            if (_targetView != null)
            {
                ShowInternalControllWindow(null);
            }

            GUIStyle badStyle = new GUIStyle(GUI.skin.label);
            badStyle.normal.textColor = new Color(0.85f, 0.1f, 0.1f);
            GUILayout.Label("No GameObject selected with an 'MonoBaseView' Component on it", badStyle);
        }

        Repaint();
    }

	private void SetupEditors()
	{
		_editors.Clear();
		Type[] componentEditors = Assembly.GetAssembly(typeof(BaseModelComponentEditor)).GetTypes().Where(t => t.IsClass && typeof(BaseModelComponentEditor).IsAssignableFrom(t) && !t.IsAbstract).ToArray();
		for(int i = 0; i < componentEditors.Length; i++)
		{
			object[] attributes = componentEditors[i].GetCustomAttributes(typeof(ModelComponentEditorAttribute), true);
			if(attributes.Length > 0)
			{
				ModelComponentEditorAttribute customEditorAttribute = attributes[0] as ModelComponentEditorAttribute;
				try
				{
					_editors.Add(customEditorAttribute.ComponentType, Activator.CreateInstance(componentEditors[i]) as BaseModelComponentEditor);
				}
				catch(Exception e)
				{
					Debug.Log("Model Component Editor Error for " + componentEditors[i].Name + ": " + e.Message);
				}
			}
		}
	}

	private void ShowInternalControllWindow(MonoBaseView monoBaseView)
    {
        if(monoBaseView == null || monoBaseView.LinkingController == null)
		{
			_componentsEditorsOpen.Clear();
			GUIStyle okStyle = new GUIStyle(GUI.skin.label);
            okStyle.normal.textColor = Color.yellow;
            GUILayout.Label(string.Format("Selected {0} is not connected to a model", (monoBaseView == null ? "NULL" : monoBaseView.GetType().Name)), okStyle);
            _targetModel = null;
            _targetView = null;
            return;
        }

        if(_targetView != monoBaseView)
        {
			BaseModel model  = MVCUtil.GetModel<BaseModel>(monoBaseView);
			if(_targetModel != model)
			{
				_componentsEditorsOpen.Clear();
				_targetModel = model;
				_targetView = monoBaseView;
			}
        }

        if (_targetModel == null)
            return;

        EditorGUILayout.BeginVertical(GUI.skin.textArea);

        GUILayout.Label("View Object Name: " + _targetView.gameObject.name);
        GUILayout.Label("Model Type: " + _targetModel.GetType());

		FieldInfo modelComponentsFieldInfo = typeof(BaseModel).GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo componentsFieldInfo = typeof(ModelComponents).GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
		HashSet<BaseModelComponent> componentsOfModel = (HashSet<BaseModelComponent>)componentsFieldInfo.GetValue(modelComponentsFieldInfo.GetValue(_targetModel));

		GUILayout.Label("Model Components: ");

		if(componentsOfModel != null)
		{
			if(_showComponents = EditorGUILayout.Foldout(_showComponents, string.Format("Components ({0})", componentsOfModel.Count)))
			{
				EditorGUILayout.BeginVertical();

				Action optionAction = null;

				foreach(BaseModelComponent component in componentsOfModel)
				{
					GUIStyle s = new GUIStyle(GUI.skin.label);
					s.normal.textColor = new Color(0.2f, 0.2f, 0.75f);
					
					BaseModelComponentEditor editor = GetEditorForComponent(component);
					if(editor != null)
					{
						s = EditorStyles.foldout;
						s.normal.textColor = new Color(0.3f, 0.2f, 0.75f);
						bool inContainer = _componentsEditorsOpen.Contains(component);

						GUILayout.BeginHorizontal();
						inContainer = EditorGUILayout.Foldout(inContainer, " * " + component, s);
						Action a = DrawComponentMenu(component);
						if(optionAction == null)
						{
							optionAction = a;
						}
						GUILayout.EndHorizontal();

						if(inContainer)
						{
							if(!_componentsEditorsOpen.Contains(component))
							{
								_componentsEditorsOpen.Add(component);
							}

							GUILayout.BeginVertical(GUI.skin.box);
							GUILayout.Space(5f);
							editor.OnGUI(component);
							GUILayout.Space(5f);
							GUILayout.EndHorizontal();
						}
						else
						{
							_componentsEditorsOpen.Remove(component);
						}
					}
					else
					{
						GUILayout.BeginHorizontal();
						GUILayout.Label(" * " + component, s);
						Action a = DrawComponentMenu(component);
						if(optionAction == null)
						{
							optionAction = a;
						}
						GUILayout.EndHorizontal();
					}
				}

				if(optionAction != null)
				{
					optionAction();
				}

				EditorGUILayout.EndVertical();

				EditorGUILayout.Space();
			}
		}

		EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        if (GUILayout.Button("Destroy Model"))
        {
            _targetModel.Destroy();
        }
    }

	private Action DrawComponentMenu(BaseModelComponent component)
	{
		GUIStyle s = new GUIStyle(GUI.skin.button);
		s.fixedWidth = 30f;

		GUILayout.Space(10);
		if(GUILayout.Button("x", s))
		{
			return () =>
			{
				_targetModel.RemoveComponent(component);
			};
		}
		GUILayout.Space(10);
		return null;
	}

	private BaseModelComponentEditor GetEditorForComponent(BaseModelComponent component)
	{
		foreach(var pair in _editors)
		{
			if(component.GetType().IsAssignableFrom(pair.Key))
			{
				return pair.Value;
			}
		}

		return null;
	}
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ModelComponentEditorAttribute : Attribute
{
	public Type ComponentType;
	public ModelComponentEditorAttribute(Type componentType)
	{
		ComponentType = componentType;
	}
}

public abstract class BaseModelComponentEditor
{
	public abstract void OnGUI(BaseModelComponent component);
}