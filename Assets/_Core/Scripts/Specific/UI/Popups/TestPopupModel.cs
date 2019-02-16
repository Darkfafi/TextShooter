using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPopupModel : BasePopupModel
{
	public override string PopupModelID
	{
		get
		{
			return "TestPopup";
		}
	}

	public string TestText
	{
		get; private set;
	}

	public TestPopupModel(string testText)
	{
		TestText = testText;
	}
}
