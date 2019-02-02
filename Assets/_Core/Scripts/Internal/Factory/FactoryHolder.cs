using System;
using System.Collections.Generic;

public class FactoryHolder
{
	private Dictionary<Type, IFactory> _factoriesCreated = new Dictionary<Type, IFactory>();
	private List<IFactoryCreator> _registeredFactoryCreators = new List<IFactoryCreator>();

	public T GetFactory<T>() where T : IFactory
	{
		IFactory factory;
		Type factoryType = typeof(T);
		if(!_factoriesCreated.TryGetValue(factoryType, out factory))
		{
			for(int i = 0, c = _registeredFactoryCreators.Count; i < c; i++)
			{
				factory = _registeredFactoryCreators[i].CreateFactoryIfAble(factoryType);
				if(factory != null)
				{
					_factoriesCreated.Add(factoryType, factory);
					factory.Setup(this);
					break;
				}
			}
		}

		if(factory == null)
			throw new Exception(string.Format("Factory of Type `{0}` could not be created due to no IFactoryCreator being able to", factoryType));

		return (T)factory;
	}

	public void RegisterFactoryCreator(IFactoryCreator creator)
	{
		if(_registeredFactoryCreators.Contains(creator))
			return;

		_registeredFactoryCreators.Add(creator);
	}

	public void UnregisterFactoryCreator(IFactoryCreator creator)
	{
		_registeredFactoryCreators.Remove(creator);
	}

	public void Clean()
	{
		_factoriesCreated.Clear();
		_registeredFactoryCreators.Clear();
		_factoriesCreated = null;
		_registeredFactoryCreators = null;
	}
}

public interface IFactory<RETURN, DATA> : IFactory
{
	RETURN Create(DATA data);
}

public interface IFactory
{
	void Setup(FactoryHolder factoryHolder);
}

public interface IFactoryCreator
{
	IFactory CreateFactoryIfAble(Type factoryType);
}