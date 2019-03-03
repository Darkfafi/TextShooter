using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Vertification
{
	public static class MonoPopupsVertification
	{
		public static void Run()
		{
			Vertifier.GenericVerification("Popups",
					new Vertifier.TestStepData("PopupViews", VertifyViews)
				);
		}

		private static Vertifier.TestResult VertifyViews()
		{
			string message = null;
			bool success = true;

			Type[] typesInProject = Assembly.GetAssembly(typeof(PopupIDAttribute)).GetTypes().ToArray();

			List<string> popupIDsWithoutView = new List<string>();

			foreach(Type projectFileType in typesInProject)
			{
				FieldInfo[] fields = projectFileType.GetFields(BindingFlags.Public | BindingFlags.Static).Where(t => t.GetCustomAttributes(typeof(PopupIDAttribute), true).Length > 0 && t.IsLiteral && !t.IsInitOnly).ToArray();
				foreach(FieldInfo popupIDField in fields)
				{
					object v = popupIDField.GetValue(null);
					if(v != null)
					{
						string popupID = v.ToString();
						if(!HasPopupView(popupID))
						{
							success = false;
							popupIDsWithoutView.Add(popupID);
							message = string.Format("Popup with ID {0} has no View inside Resources/{1}", popupID, MonoPopupManagerView.MONO_POPUP_VIEW_PREFAB_RESOURCE_LOCATION);
						}
						else
						{
							Debug.Log(string.Format("<color='green'>Popup {0} validated</color>", popupID));
						}
					}
				}
			}

			return new Vertifier.TestResult(success, message);
		}

		private static bool HasPopupView(string v)
		{
			return MonoPopupManagerView.GetPopupViewPrefab(v) != null;
			
		}
	}
}