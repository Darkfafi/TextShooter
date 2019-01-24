using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ModelComponentEditorAttribute : Attribute
{
	public Type ComponentType;
	public ModelComponentEditorAttribute(Type componentType)
	{
		ComponentType = componentType;
	}
}

public class ModelComponentEditor
{
	public bool ShouldStayClosed
	{
		get
		{
			return GetType() == typeof(ModelComponentEditor) && (_editorFields.Length + _editorMethods.Length) == 0;
		}
	}

	private Dictionary<ParameterInfo, object> _parameterEditorValuesMap = new Dictionary<ParameterInfo, object>();
	private FieldInfo[] _editorFields = new FieldInfo[] { };
	private MethodInfo[] _editorMethods = new MethodInfo[] { };

	private bool _showMethods = true;
	private bool _showFields = true;

	public void CallOnOpen(BaseModelComponent component)
	{
		_editorFields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.GetCustomAttributes(typeof(ModelEditorFieldAttribute), true).Length > 0).ToArray();
		_editorMethods = component.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.GetCustomAttributes(typeof(ModelEditorMethodAttribute), true).Length > 0).ToArray();
		_showMethods = true;
		_showFields = true;
		OnOpen();
	}

	public void CallOnClose()
	{
		OnClose();
		_parameterEditorValuesMap.Clear();
	}

	public virtual void OnGUI(BaseModelComponent component)
	{
		if(_editorFields.Length > 0)
		{
			_showFields = EditorGUILayout.Foldout(_showFields, "Fields: ");
			if(_showFields)
			{
				EditorGUILayout.BeginVertical(GUI.skin.box);
				foreach(FieldInfo editorField in _editorFields)
				{
					EditorGUILayout.BeginVertical(GUI.skin.box);
					DrawField(editorField, component);
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndVertical();

				GUILayout.Space(5f);
			}
		}

		if(_editorMethods.Length > 0)
		{
			_showMethods = EditorGUILayout.Foldout(_showMethods, "Methods: ");
			if(_showMethods)
			{
				EditorGUILayout.BeginVertical(GUI.skin.box);
				foreach(MethodInfo editorMethod in _editorMethods)
				{
					EditorGUILayout.BeginVertical(GUI.skin.box);
					DrawMethod(editorMethod, component);
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndVertical();
			}
		}
	}

	protected virtual void OnOpen()
	{

	}

	protected virtual void OnClose()
	{

	}

	private void DrawMethod(MethodInfo editorMethod, BaseModelComponent obj)
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Method: " + editorMethod.Name);
		if(GUILayout.Button("Invoke", GUILayout.Width(70)))
		{
			ParameterInfo[] pms = editorMethod.GetParameters();
			List<object> parameters = new List<object>();
			for(int i = 0; i < pms.Length; i++)
			{
				foreach(var pair in _parameterEditorValuesMap)
				{
					if(pair.Key == pms[i])
					{
						parameters.Add(pair.Value);
					}
				}
			}
			
			editorMethod.Invoke(obj, parameters.ToArray());
		}
		EditorGUILayout.EndHorizontal();

		if(editorMethod.GetParameters().Length > 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(25f);
			GUILayout.Label("Parameters: ");
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(45f);
			foreach(ParameterInfo parameterInfo in editorMethod.GetParameters())
			{
				object preValue;
				object value;
				_parameterEditorValuesMap.TryGetValue(parameterInfo, out preValue);
				value = DrawTypeField(parameterInfo.Name, parameterInfo.ParameterType, preValue);
				if(value != preValue)
				{
					if(!_parameterEditorValuesMap.ContainsKey(parameterInfo))
					{
						_parameterEditorValuesMap.Add(parameterInfo, value);
					}

					_parameterEditorValuesMap[parameterInfo] = value;
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private void DrawField(FieldInfo field, BaseModelComponent obj)
	{
		Type fieldType = field.FieldType;
		object newValue = DrawTypeField("Field: " + field.Name.Replace("_", " ").Trim(), fieldType, field.GetValue(obj));
		field.SetValue(obj, newValue);
	}

	private object DrawTypeField(string labelName, Type fieldType, object value)
	{
		if(fieldType == typeof(string))
		{
			return EditorGUILayout.TextField(labelName, (string)value);
		}
		else if(fieldType == typeof(int))
		{
			if(value == null)
			{
				value = default(int);
			}
			return EditorGUILayout.IntField(labelName, (int)value);
		}
		else if(fieldType == typeof(float))
		{
			if(value == null)
			{
				value = default(float);
			}
			return EditorGUILayout.FloatField(labelName, (float)value);
		}
		else if(fieldType == typeof(Vector2))
		{
			if(value == null)
			{
				value = default(Vector2);
			}
			return EditorGUILayout.Vector2Field(labelName, (Vector2)value);
		}
		else if(fieldType == typeof(Vector3))
		{
			if(value == null)
			{
				value = default(Vector3);
			}
			return EditorGUILayout.Vector3Field(labelName, (Vector3)value);
		}
		else if(fieldType == typeof(Vector2Int))
		{
			if(value == null)
			{
				value = default(Vector2Int);
			}
			return EditorGUILayout.Vector2IntField(labelName, (Vector2Int)value);
		}
		else if(fieldType == typeof(Vector3Int))
		{
			if(value == null)
			{
				value = default(Vector3Int);
			}
			return EditorGUILayout.Vector3IntField(labelName, (Vector3Int)value);
		}
		else if(typeof(EntityModel) == fieldType)
		{
			EntityModel target = value as EntityModel;
			MonoBaseView view = null;
			if(target != null && !target.IsDestroyed)
			{
				view = MVCUtil.GetView<MonoBaseView>(target);
			}

			view = EditorGUILayout.ObjectField(labelName, view, typeof(MonoBaseView), true) as MonoBaseView;

			if(view != null)
			{
				return MVCUtil.GetModel<EntityModel>(view);
			}
			else
			{
				return null;
			}
		}

		return null;
	}
}