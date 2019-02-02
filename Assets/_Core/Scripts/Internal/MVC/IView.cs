public interface IView
{
	IMethodPermitter LinkingController
	{
		get;
	}
	void PreSetupView(IMethodPermitter controller);
	void SetupView();
	void DestroyView();
	void PreDestroyView();
}
