using System;

public class CharInputModel : BaseModel
{
	public event Action<char> InputEvent;

	public void DoCharInput(char charInput)
	{
		if(InputEvent != null)
		{
			InputEvent(charInput);
		}
	}
}
