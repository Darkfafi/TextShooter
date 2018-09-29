using UnityEngine;
using UnityEditor;

public class ModelManipulationWindow : EditorWindow
{
    private MonoBaseView _targetView;
    private BaseModel _targetModel;
    private bool _manipulateTransform = false;

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
            _manipulateTransform = false;
        }

        if (_targetModel == null)
            return;


        GUIStyle goodStyle = new GUIStyle(GUI.skin.label);
        goodStyle.normal.textColor = new Color(0f, 0.9f, 0f);
        GUILayout.Label("View Object Name: " + _targetView.gameObject.name, goodStyle);
        GUILayout.Label("Model Type: " + _targetModel.GetType());

        EditorGUILayout.Space();

        GUILayout.Label("Model Transform: ");

        EditorGUILayout.Space();

        IModelTransformHolder transformHolder = _targetModel as IModelTransformHolder;

        if (transformHolder != null)
        {
            if (_manipulateTransform = EditorGUILayout.Foldout(_manipulateTransform, (_manipulateTransform ? "Deactivate" : "Activate") + " Manipulation: "))
            {
                transformHolder.ModelTransform.Position = EditorGUILayout.Vector3Field("Position", transformHolder.ModelTransform.Position);
                transformHolder.ModelTransform.Rotation = EditorGUILayout.Vector3Field("Rotation", transformHolder.ModelTransform.Rotation);
                transformHolder.ModelTransform.Scale = EditorGUILayout.Vector3Field("Scale", transformHolder.ModelTransform.Scale);
            }

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Destroy Model"))
        {
            _targetModel.Destroy();
        }
    }
}
