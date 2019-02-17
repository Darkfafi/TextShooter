using System;
using UnityEngine;

namespace Vertification
{
	public static class Vertifier
	{
		public static void GenericVerification(string testName, params TestStepData[] testSteps)
		{
			Debug.LogFormat("<color='olive'>-- Vertification {0} Start --</color>", testName);

			for(int i = 0; i < testSteps.Length; i++)
			{
				string c = i % 2 == 0 ? "navy" : "teal";
				TestStepData testStep = testSteps[i];
				Debug.LogFormat("<color=" + c + "> [{0}] Test {1} Started </color>", i, testStep.StepName);
				TestResult testResult = testStep.StepCall();
				if(!testResult.TestSucceeded)
				{
					Debug.LogErrorFormat("<color='red'> [{0}] Test {1} Failed! Error: " + testResult.TestMessage + "</color>", i, testStep.StepName);
					break;
				}
				Debug.LogFormat("<color=" + c + "> [{0}] Test {1} Succeeded" + (string.IsNullOrEmpty(testResult.TestMessage) ? "" : ": " + testResult.TestMessage) + "</color>", i, testStep.StepName);
			}

			Debug.LogFormat("<color='olive'>-- Vertification {0} Ended --</color>", testName);
		}

		public struct TestStepData
		{
			public string StepName;
			public Func<TestResult> StepCall;

			public TestStepData(string testStepName, Func<TestResult> testStepCall)
			{
				StepName = testStepName;
				StepCall = testStepCall;
			}
		}

		public struct TestResult
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
}