using System;
using UnityEngine;

namespace Rules.Progressor
{
	public class ProgressorRules
	{
		public ProgressorTypeInfo[] ProgressorTypes;

		private static ProgressorRules _instance;

		public static ProgressorRules GetRules()
		{
			if(_instance == null)
			{
				_instance = JsonUtility.FromJson<ProgressorRules>(ResourceLocator.Locate<TextAsset>("progressorRules", "Rules").text);
			}

			return _instance;
		}

		private ProgressorRules()
		{

		}
	}

	[Serializable]
	public struct ProgressorTypeInfo
	{
		public string Type;
		public string ValueType;
		public string ValueOptions;
		public string Description;
	}
}