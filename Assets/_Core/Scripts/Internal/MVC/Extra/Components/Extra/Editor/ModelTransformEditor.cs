using UnityEditor;

[ModelComponentEditor(typeof(ModelTransform))]
public class ModelTransformEditor : BaseModelComponentEditor
{
	public override void OnGUI(BaseModelComponent transformComponent)
	{
		ModelTransform t = transformComponent as ModelTransform;

		t.Position = EditorGUILayout.Vector3Field("Position", t.Position);
		t.Rotation = EditorGUILayout.Vector3Field("Rotation", t.Rotation);
		t.Scale = EditorGUILayout.Vector3Field("Scale", t.Scale);
	}
}
