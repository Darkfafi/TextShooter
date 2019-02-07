using System;
using UnityEditor;
using UnityEngine;

public static class Verification
{
	public const string VERTIFICATION_MENU_LOCATION = "Vertification";

	// Vertification Editor Calls

	[MenuItem(VERTIFICATION_MENU_LOCATION + "/Enemies")]
	public static void VerifyEnemyDatabase()
	{
		GenericVerification("Enemies", 
			new TestStepData("EnemyViews", () =>
			{
				StaticDatabase<EnemyData> database;

				try
				{
					database = EnemyDatabaseParser.ParseXml(DatabaseContents.GetEnemyDatabaseText());
				}
				catch(Exception e)
				{
					return new TestResult(false, e.Message);
				}

				string message;
				bool success = EnemyDatabaseVertification.VertifyViews(database, out message);

				if(success)
					message = null;

				return new TestResult(success, message);

			})
		);

	}

	// Background Vertification Seciton

	private static void GenericVerification(string testName, params TestStepData[] testSteps)
	{
		Debug.LogFormat("<color='olive'>-- Vertification {0} Start --</color>", testName);
		
		for(int i = 0; i < testSteps.Length; i++)
		{
			string c = i % 2 == 0 ? "navy" : "teal";
			TestStepData testStep = testSteps[i];
			Debug.LogFormat("<color="+c+"> [{0}] Test {1} Started </color>", i, testStep.StepName);
			TestResult testResult = testStep.StepCall();
			if(!testResult.TestSucceeded)
			{
				Debug.LogErrorFormat("<color='red'> [{0}] Test {1} Failed! Error: " + testResult.TestMessage + "</color>", i, testStep.StepName);
				break;
			}
			Debug.LogFormat("<color="+c+ "> [{0}] Test {1} Succeeded" + (string.IsNullOrEmpty(testResult.TestMessage) ? "" : ": " + testResult.TestMessage) + "</color>", i, testStep.StepName);
		}

		Debug.LogFormat("<color='olive'>-- Vertification {0} Ended --</color>", testName);
	}

	private struct TestStepData
	{
		public string StepName;
		public Func<TestResult> StepCall;

		public TestStepData(string testStepName, Func<TestResult> testStepCall)
		{
			StepName = testStepName;
			StepCall = testStepCall;
		}
	}

	private struct TestResult
	{
		public bool TestSucceeded;
		public string TestMessage;

		public TestResult(bool testSucceeded, string testMessage)
		{
			TestSucceeded = testSucceeded;
			TestMessage = testMessage;
		}
	}
}
