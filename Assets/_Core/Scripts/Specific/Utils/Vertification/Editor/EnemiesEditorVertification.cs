using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Vertification
{
	public class EnemiesEditorVertification : MonoBehaviour
	{
		[MenuItem(VertificationGlobals.VERTIFICATION_MENU_LOCATION + "/Enemies")]
		public static void VerifyEnemyDatabase()
		{
			EnemyDatabaseVertification.Run();
		}
	}
}