public interface IFactory<T, U> where T : EntityModel where U : IFactoryData
{
	T Create(U data);
}

public interface IFactoryData
{

}
