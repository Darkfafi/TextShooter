using UnityEditor;

[ModelComponentEditor(typeof(ModelTransform))]
public class ModelTransformEditor : ModelComponentEditor
{
	public override void OnGUI(BaseModelComponent transformComponent)
	{
		ModelTransform t = transformComponent as ModelTransform;

		t.SetPos(EditorGUILayout.Vector3Field("Position", t.Position));
		t.SetRot(EditorGUILayout.Vector3Field("Rotation", t.Rotation));
		t.SetScale(EditorGUILayout.Vector3Field("Scale", t.Scale));
	}
}
