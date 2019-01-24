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

	private SearchWindow _openSearchWindow;
	private bool _showComponents = true;
	private Dictionary<Type, BaseModelComponentEditor> _editors = new Dictionary<Type, BaseModelComponentEditor>();
	private Dictionary<BaseModelComponent, BaseModelComponentEditor> _componentsEditorsOpen = new Dictionary<BaseModelComponent, BaseModelComponentEditor>();

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

		HashSet<BaseModelComponent> enabledComponentsOfModel = GetEnabledModelComponents(_targetModel);
		HashSet<BaseModelComponent> disabledComponentsOfModel = GetDisabledModelComponents(_targetModel);

		GUILayout.Label("Model Components: ");

		if(enabledComponentsOfModel != null)
		{
			if(_showComponents = EditorGUILayout.Foldout(_showComponents, string.Format("Components (E({0}), D({1}))", enabledComponentsOfModel.Count, disabledComponentsOfModel.Count)))
			{
				EditorGUILayout.BeginVertical();

				Action optionAction = null;
				foreach(BaseModelComponent component in enabledComponentsOfModel)
				{
					Action a = DrawComponentSection(component);
					if(optionAction == null)
					{
						optionAction = a;
					}
				}

				foreach(BaseModelComponent component in disabledComponentsOfModel)
				{
					Action a = DrawComponentSection(component);
					if(optionAction == null)
					{
						optionAction = a;
					}
				}

				if(optionAction != null)
				{
					optionAction();
				}

				if(GUILayout.Button("Add"))
				{
					Type[] componentTypes = Assembly.GetAssembly(typeof(ModelTransform)).GetTypes().Where(t => typeof(BaseModelComponent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass).ToArray();
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
		Color color = component.IsEnabled ? new Color(0.2f, 0.4f, 0.75f) : new Color(0.4f, 0.2f, 0.75f);
		GUIStyle s = new GUIStyle(GUI.skin.label);
		s.normal.textColor = color;
		Action optionAction = null;
		string componentName = component.IsEnabled ? " (E) " : " (D) ";
		componentName += component.GetType().Name;

		BaseModelComponentEditor editor = GetEditorForComponent(component);
		if(editor != null)
		{
			s = EditorStyles.foldout;
			s.normal.textColor = color;
			bool inContainer = _componentsEditorsOpen.ContainsKey(component);

			GUILayout.BeginHorizontal();
			inContainer = EditorGUILayout.Foldout(inContainer, componentName, s);
			optionAction = DrawComponentMenu(component);
			GUILayout.EndHorizontal();

			if(inContainer)
			{
				OpenEditor(component);
				GUILayout.BeginHorizontal();
				GUILayout.Space(20f);
				GUILayout.BeginVertical(GUI.skin.box);
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
		}
		else
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(componentName, s);
			optionAction = DrawComponentMenu(component);
			GUILayout.EndHorizontal();
		}

		return optionAction;
	}

	private void OpenEditor(BaseModelComponent component)
	{
		if(!_componentsEditorsOpen.ContainsKey(component))
		{
			BaseModelComponentEditor editor = GetEditorForComponent(component);
			if(editor != null)
			{
				_componentsEditorsOpen.Add(component, editor);
				editor.OnOpen();
			}
		}
	}

	private void OpenAllEditors()
	{
		if(_targetModel != null)
		{
			HashSet<BaseModelComponent> enabledComponents = GetEnabledModelComponents(_targetModel);
			foreach(BaseModelComponent c in enabledComponents)
			{
				OpenEditor(c);
			}

			HashSet<BaseModelComponent> disabledComponents = GetDisabledModelComponents(_targetModel);
			foreach(BaseModelComponent c in disabledComponents)
			{
				OpenEditor(c);
			}
		}
	}
	private void CloseEditor(BaseModelComponent component)
	{
		if(_componentsEditorsOpen.ContainsKey(component))
		{
			BaseModelComponentEditor editor = GetEditorForComponent(component);
			_componentsEditorsOpen.Remove(component);
			if(editor != null)
			{
				editor.OnClose();
			}
		}
	}

	private void CloseAllEditors()
	{
		foreach(var pair in _componentsEditorsOpen)
		{
			pair.Value.OnClose();
		}

		_componentsEditorsOpen.Clear();
	}

	private HashSet<BaseModelComponent> GetEnabledModelComponents(BaseModel model)
	{
		FieldInfo modelComponentsFieldInfo = typeof(BaseModel).GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo enabledComponentsFieldInfo = typeof(ModelComponents).GetField("_enabledComponents", BindingFlags.NonPublic | BindingFlags.Instance);
		return (HashSet<BaseModelComponent>)enabledComponentsFieldInfo.GetValue(modelComponentsFieldInfo.GetValue(model));
	}

	private HashSet<BaseModelComponent> GetDisabledModelComponents(BaseModel model)
	{
		FieldInfo modelComponentsFieldInfo = typeof(BaseModel).GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo disabledComponentsFieldInfo = typeof(ModelComponents).GetField("_disabledComponents", BindingFlags.NonPublic | BindingFlags.Instance);
		return (HashSet<BaseModelComponent>)disabledComponentsFieldInfo.GetValue(modelComponentsFieldInfo.GetValue(model));
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
	public virtual void OnOpen()
	{

	}

	public virtual void OnClose()
	{

	}

	public abstract void OnGUI(BaseModelComponent component);
}