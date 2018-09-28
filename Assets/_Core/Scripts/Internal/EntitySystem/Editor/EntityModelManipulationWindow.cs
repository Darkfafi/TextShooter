using UnityEngine;
using UnityEditor;

public class EntityModelManipulationWindow : EditorWindow
{
    private EntityView _targetView;
    private EntityModel _targetModel;
    private bool _manipulateTransform = false;

    [MenuItem("EntitySystem/Entity Model Manipulator")]
    static void OpenWindow()
    {
        EntityModelManipulationWindow window = GetWindow<EntityModelManipulationWindow>("Entity Model Manipulator", true);
        window.Show(true);
        window.Repaint();
    }

    protected void OnGUI()
    {
        GameObject targetGameObject = Selection.activeGameObject;
        EntityView entityView = null;

        if (targetGameObject != null)
        {
            entityView = targetGameObject.GetComponent<EntityView>();
        }

        GUILayout.Label("Target Entity Controll", EditorStyles.boldLabel);

        if(entityView != null)
        {
            ShowEntityControllWindow(entityView);
        }
        else
        {
            if (_targetView != null)
            {
                ShowEntityControllWindow(null);
            }

            GUILayout.Label("No GameObject selected with an 'EntityView' Component");
        }

        Repaint();
    }

    private void ShowEntityControllWindow(EntityView entityView)
    {
        if(entityView == null || entityView.LinkingController == null)
        {
            GUILayout.Label("Selected 'EntityView' is not connected to a model");
            _targetModel = null;
            _targetView = null;
            return;
        }

        if(_targetView != entityView)
        {
            _targetModel = MVCUtil.GetModel<EntityModel>(entityView);
            _targetView = entityView;
            _manipulateTransform = false;
        }

        if (_targetModel == null)
            return;


        GUILayout.Label("View Object Name: " + _targetView.gameObject.name);
        GUILayout.Label("Model Type: " + _targetModel.GetType());

        EditorGUILayout.Space();

        GUILayout.Label("Model Transform: ");

        EditorGUILayout.Space();

        if (_manipulateTransform = EditorGUILayout.Foldout(_manipulateTransform, (_manipulateTransform ? "Deactivate" : "Activate") + " Manipulation: "))
        {
            _targetModel.Transform.Position = EditorGUILayout.Vector3Field("Position", _targetModel.Transform.Position);
            _targetModel.Transform.Rotation = EditorGUILayout.Vector3Field("Rotation", _targetModel.Transform.Rotation);
            _targetModel.Transform.Scale = EditorGUILayout.Vector3Field("Scale", _targetModel.Transform.Scale);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Destroy Model"))
        {
            _targetModel.Destroy();
        }
    }
}
