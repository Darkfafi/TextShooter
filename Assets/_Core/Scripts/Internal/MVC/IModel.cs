public interface IModel
{
	IAbstractController LinkingController
	{
		get;
	}
	MethodPermitter MethodPermitter
	{
		get;
	}
	void SetupModel(IAbstractController controller);
	void Destroy();
}
