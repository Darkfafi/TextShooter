using UnityEngine;
using UnityEditor;

public class ModelManipulationWindow : EditorWindow
{
    private MonoBaseView _targetView;
    private BaseModel _targetModel;

    private bool _manipulateTransform = false;
    private bool _showTags = false;
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

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();


        IModelTagsHolder tagsHolder = _targetModel as IModelTagsHolder;

        EditorGUILayout.BeginVertical(GUI.skin.textArea);

        GUILayout.Label("Model Tags: " + (tagsHolder != null ? "OK" : "None Available"));

        if (tagsHolder != null)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<string> tags = tagsHolder.ModelTags.GetTags();
            if (_showTags = EditorGUILayout.Foldout(_showTags, string.Format("Tags ({0})", tags.Count)))
            {
                EditorGUILayout.BeginHorizontal();
                _currentTagAddString = EditorGUILayout.TextField("Add Tag: ", _currentTagAddString);
                if(GUILayout.Button("+", GUILayout.Width(40)))
                {
                    tagsHolder.ModelTags.AddTag(_currentTagAddString);
                    _currentTagAddString = string.Empty;
                }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < tags.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUIStyle s = new GUIStyle(GUI.skin.label);
                    s.normal.textColor = new Color(0.5f, 0.3f, 0.6f);
                    GUILayout.Label(" * " + tags[i], s);
                    s = new GUIStyle(GUI.skin.button);
                    s.normal.textColor = Color.red;
                    if (GUILayout.Button("x", s, GUILayout.Width(25)))
                    {
                        tagsHolder.ModelTags.RemoveTag(tags[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        IModelTransformHolder transformHolder = _targetModel as IModelTransformHolder;

        EditorGUILayout.BeginVertical(GUI.skin.textArea);

        GUILayout.Label("Model Transform: " + (transformHolder != null ? "OK" : "None Available"));

        if (transformHolder != null)
        {
            if (_manipulateTransform = EditorGUILayout.Foldout(_manipulateTransform, "Transform Data"))
            {
                transformHolder.ModelTransform.Position = EditorGUILayout.Vector3Field("Position", transformHolder.ModelTransform.Position);
                transformHolder.ModelTransform.Rotation = EditorGUILayout.Vector3Field("Rotation", transformHolder.ModelTransform.Rotation);
                transformHolder.ModelTransform.Scale = EditorGUILayout.Vector3Field("Scale", transformHolder.ModelTransform.Scale);
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
