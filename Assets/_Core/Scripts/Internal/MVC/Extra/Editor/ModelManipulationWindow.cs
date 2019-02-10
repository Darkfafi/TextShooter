using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ModelManipulationWindow : EditorWindow
{
    private MonoBaseView _targetView;
    private BaseModel _targetModel;

	private Vector2 _scrollPos = Vector2.zero;
	private SearchWindow _openSearchWindow;
	private bool _showComponents = true;
	private Dictionary<Type, ModelComponentEditor> _editors = new Dictionary<Type, ModelComponentEditor>();
	private Dictionary<BaseModelComponent, ModelComponentEditor> _componentsEditorsOpen = new Dictionary<BaseModelComponent, ModelComponentEditor>();

	private Color _enabledComponentColor = new Color(0.2f, 0.4f, 0.75f);
	private Color _disabledComponentColor = new Color(0.4f, 0.2f, 0.75f);

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
			CloseAllEditors();
			CloseOpenSearchWindow();
			if(_targetView != null)
			{
				ShowInternalControllWindow(null);
			}

			GUIStyle badStyle = new GUIStyle(GUI.skin.label);
            badStyle.normal.textColor = new Color(0.85f, 0.1f, 0.1f);
            GUILayout.Label("No GameObject selected with an 'MonoBaseView' Component on it", badStyle);
        }

        Repaint();
    }

	private void OnDestroy()
	{
		CloseAllEditors();
		_componentsEditorsOpen = null;

		if(_openSearchWindow != null)
		{
			_openSearchWindow.Close();
			_openSearchWindow = null;
		}

		_targetView = null;
		_targetModel = null;

		_editors = null;
	}

	private void SetupEditors()
	{
		_editors.Clear();
		Type[] componentEditors = Assembly.GetAssembly(typeof(ModelComponentEditor)).GetTypes().Where(t => t.IsClass && typeof(ModelComponentEditor).IsAssignableFrom(t) && !t.IsAbstract).ToArray();
		for(int i = 0; i < componentEditors.Length; i++)
		{
			object[] attributes = componentEditors[i].GetCustomAttributes(typeof(ModelComponentEditorAttribute), true);
			if(attributes.Length > 0)
			{
				ModelComponentEditorAttribute customEditorAttribute = attributes[0] as ModelComponentEditorAttribute;
				try
				{
					_editors.Add(customEditorAttribute.ComponentType, Activator.CreateInstance(componentEditors[i]) as ModelComponentEditor);
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
			CloseAllEditors();
			CloseOpenSearchWindow();
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
				_showComponents = true;
				CloseOpenSearchWindow();
				CloseAllEditors();
				 _targetModel = model;
				_targetView = monoBaseView;
				OpenAllEditors();
			}
        }

        if (_targetModel == null)
            return;

        EditorGUILayout.BeginVertical(GUI.skin.textArea);

        GUILayout.Label("View Object Name: " + _targetView.gameObject.name);
        GUILayout.Label("Model Type: " + _targetModel.GetType());

		HashSet<BaseModelComponent> componentsOfModel = GetModelComponents(_targetModel);

		GUILayout.Label("Model Components: ");

		if(componentsOfModel != null)
		{
			int disabledCount = componentsOfModel.Where(c => !c.IsEnabled).Count();
			if(_showComponents = EditorGUILayout.Foldout(_showComponents, string.Format("Components (E({0}), D({1}))", componentsOfModel.Count, disabledCount), true, new GUIStyle(EditorStyles.foldout)))
			{
				EditorGUILayout.BeginVertical();

				Action optionAction = null;
				_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(position.width - 20), GUILayout.Height(Mathf.Clamp(position.height - 200, 300, 500)));
				foreach(BaseModelComponent component in componentsOfModel)
				{
					Action a = DrawComponentSection(component);

					if(component.ComponentState == ModelComponentState.Removed)
						break;

					if(optionAction == null)
					{
						optionAction = a;
					}
				}
				EditorGUILayout.EndScrollView();

				if(optionAction != null)
				{
					optionAction();
				}

				if(GUILayout.Button("Add"))
				{
					Type[] componentTypes = Assembly.GetAssembly(typeof(ModelTransform)).GetTypes().Where(t => typeof(BaseModelComponent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass && !t.IsGenericType).ToArray();
					_openSearchWindow = SearchWindow.OpenWindow((index) =>
					{
						if(index >= 0)
						{
							if(_targetModel != null)
							{
								OpenEditor(_targetModel.AddComponent(componentTypes[index]));
							}

							CloseOpenSearchWindow();
						}

					}, componentTypes);
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

	private Action DrawComponentSection(BaseModelComponent component)
	{
		Color color = component.IsEnabled ? _enabledComponentColor : _disabledComponentColor;
		GUIStyle s = new GUIStyle(GUI.skin.label);
		s.normal.textColor = color;
		Action optionAction = null;
		string componentName = component.IsEnabled ? " (E) " : " (D) ";
		componentName += component.GetType().Name;

		ModelComponentEditor editor = GetEditorForComponent(component);

		if(editor == null)
		{
			editor = new ModelComponentEditor();
		}

		s = new GUIStyle(EditorStyles.foldout);
		s.onNormal.textColor = color;
		s.onActive.textColor = color;
		s.onFocused.textColor = color;
		s.onHover.textColor = color;

		bool inContainer = _componentsEditorsOpen.ContainsKey(component);

		GUILayout.BeginHorizontal();
		inContainer = EditorGUILayout.Foldout(inContainer, componentName, s);
		optionAction = DrawComponentMenu(component);
		GUILayout.EndHorizontal();

		if(inContainer && OpenEditor(component))
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			GUIStyle componentStyle = new GUIStyle(GUI.skin.box);
			componentStyle.stretchWidth = true;
			GUILayout.BeginVertical(componentStyle);
			GUILayout.Space(5f);
			editor.OnGUI(component);
			GUILayout.Space(5f);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		else
		{
			CloseEditor(component);
		}

		return optionAction;
	}

	private bool OpenEditor(BaseModelComponent component)
	{
		if(!_componentsEditorsOpen.ContainsKey(component))
		{
			ModelComponentEditor editor = GetEditorForComponent(component);

			if(editor == null)
			{
				editor = new ModelComponentEditor();
			}

			_componentsEditorsOpen.Add(component, editor);
			editor.CallOnOpen(component);
			if(editor.ShouldStayClosed)
			{
				CloseEditor(component);
				return false;
			}
		}

		return true;
	}

	private void OpenAllEditors()
	{
		if(_targetModel != null)
		{
			HashSet<BaseModelComponent> components = GetModelComponents(_targetModel);
			foreach(BaseModelComponent c in components)
			{
				OpenEditor(c);
			}
		}
	}
	private void CloseEditor(BaseModelComponent component)
	{
		if(_componentsEditorsOpen.ContainsKey(component))
		{
			ModelComponentEditor editor = GetEditorForComponent(component);
			_componentsEditorsOpen.Remove(component);
			if(editor != null)
			{
				editor.CallOnClose();
			}
		}
	}

	private void CloseAllEditors()
	{
		_scrollPos = Vector2.zero;

		foreach(var pair in _componentsEditorsOpen)
		{
			pair.Value.CallOnClose();
		}

		_componentsEditorsOpen.Clear();
	}

	private HashSet<BaseModelComponent> GetModelComponents(BaseModel model)
	{
		FieldInfo modelComponentsFieldInfo = typeof(BaseModel).GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo enabledComponentsFieldInfo = typeof(ModelComponents).GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
		return (HashSet<BaseModelComponent>)enabledComponentsFieldInfo.GetValue(modelComponentsFieldInfo.GetValue(model));
	}

	private Action DrawComponentMenu(BaseModelComponent component)
	{
		GUIStyle s = new GUIStyle(GUI.skin.button);
		s.fixedWidth = 30f;
		s.normal.textColor = component.IsEnabled ? _enabledComponentColor : _disabledComponentColor;

		if(GUILayout.Button(component.IsEnabled ? "(E)" : "(D)", s))
		{
			return () =>
			{
				if(component != null && component.ComponentState != ModelComponentState.Removed)
				{
					component.SetEnabledState(!component.IsEnabled);
				}
			};
		}

		s.normal.textColor = Color.red;

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

	private ModelComponentEditor GetEditorForComponent(BaseModelComponent component)
	{
		if(_componentsEditorsOpen.ContainsKey(component))
			return _componentsEditorsOpen[component];

		foreach(var pair in _editors)
		{
			if(component.GetType().IsAssignableFrom(pair.Key))
			{
				return pair.Value;
			}
		}

		return null;
	}

	private void CloseOpenSearchWindow()
	{
		if(_openSearchWindow != null)
		{
			_openSearchWindow.Close();
			_openSearchWindow = null;
		}
	}
}