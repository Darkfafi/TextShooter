using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ModelManipulationWindow : EditorWindow
{
    private MonoBaseView _targetView;
    private BaseModel _targetModel;

    private bool _manipulateTransform = false;
    private bool _showTags = false;
	private bool _showComponents = false;
    private string _currentTagAddString = "";

    [MenuItem("MVC/Model Manipulator")]
    static void OpenWindow()
    {
        ModelManipulationWindow window = GetWindow<ModelManipulationWindow>("Model Manipulator", true);
        window.Show(true);
        window.Repaint();
    }

    protected void OnGUI()
    {
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

    private void ShowInternalControllWindow(MonoBaseView monoBaseView)
    {
        if(monoBaseView == null || monoBaseView.LinkingController == null)
        {
            GUIStyle okStyle = new GUIStyle(GUI.skin.label);
            okStyle.normal.textColor = Color.yellow;
            GUILayout.Label(string.Format("Selected {0} is not connected to a model", (monoBaseView == null ? "NULL" : monoBaseView.GetType().Name)), okStyle);
            _targetModel = null;
            _targetView = null;
            return;
        }

        if(_targetView != monoBaseView)
        {
            _targetModel = MVCUtil.GetModel<BaseModel>(monoBaseView);
            _targetView = monoBaseView;
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

				foreach(BaseModelComponent component in componentsOfModel)
				{
					GUIStyle s = new GUIStyle(GUI.skin.label);
					s.normal.textColor = new Color(0.2f, 0.2f, 0.75f);
					GUILayout.Label(" * " + component, s);
				}
				EditorGUILayout.EndVertical();

				EditorGUILayout.Space();
			}
		}



		EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        ModelTags tagsComponent = _targetModel.GetComponent<ModelTags>();

        EditorGUILayout.BeginVertical(GUI.skin.textArea);

        GUILayout.Label("Model Tags: " + (tagsComponent != null ? "OK" : "None Available"));

        if (tagsComponent != null)
        {
            string[] tags = tagsComponent.GetTags();
            if (_showTags = EditorGUILayout.Foldout(_showTags, string.Format("Tags ({0})", tags.Length)))
            {
                EditorGUILayout.BeginHorizontal();
                _currentTagAddString = EditorGUILayout.TextField("Add Tag: ", _currentTagAddString);
                if(GUILayout.Button("+", GUILayout.Width(40)))
                {
                    tagsComponent.AddTag(_currentTagAddString);
                    _currentTagAddString = string.Empty;
                }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < tags.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUIStyle s = new GUIStyle(GUI.skin.label);
                    s.normal.textColor = new Color(0.5f, 0.3f, 0.6f);
                    GUILayout.Label(" * " + tags[i], s);
                    s = new GUIStyle(GUI.skin.button);
                    s.normal.textColor = Color.red;
                    if (GUILayout.Button("x", s, GUILayout.Width(25)))
                    {
                        tagsComponent.RemoveTag(tags[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        ModelTransform transformComponent = _targetModel.GetComponent<ModelTransform>();

        EditorGUILayout.BeginVertical(GUI.skin.textArea);

        GUILayout.Label("Model Transform: " + (transformComponent != null ? "OK" : "None Available"));

        if (transformComponent != null)
        {
            if (_manipulateTransform = EditorGUILayout.Foldout(_manipulateTransform, "Transform Data"))
            {
                transformComponent.Position = EditorGUILayout.Vector3Field("Position", transformComponent.Position);
                transformComponent.Rotation = EditorGUILayout.Vector3Field("Rotation", transformComponent.Rotation);
                transformComponent.Scale = EditorGUILayout.Vector3Field("Scale", transformComponent.Scale);
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Destroy Model"))
        {
            _targetModel.Destroy();
        }
    }
}
