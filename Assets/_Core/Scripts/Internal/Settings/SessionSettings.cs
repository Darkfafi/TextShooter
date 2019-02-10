using System;
using System.Collections.Generic;

public static class SessionSettings
{
	private static Dictionary<Type, ISettings> _requestedSettings = new Dictionary<Type, ISettings>();

	public static T Request<T>() where T : class, ISettings
	{
		return Request(typeof(T)) as T;
	}

	public static ISettings Request(Type settingsType)
	{
		if(!_requestedSettings.ContainsKey(settingsType))
		{
			_requestedSettings.Add(settingsType, Activator.CreateInstance(settingsType) as ISettings);
		}

		return _requestedSettings[settingsType];
	}

	public static void Reset<T>() where T : class, ISettings
	{
		Reset(typeof(T));
	}

	public static void Reset(Type settingsType)
	{
		ISettings settings;
		if(_requestedSettings.TryGetValue(settingsType, out settings))
		{
			settings.Reset();
		}
	}

	public static void Reset()
	{
		foreach(var pair in _requestedSettings)
		{
			Reset(pair.Key);
		}
	}

	public static void Clear(bool resetBeforeClear = true)
	{
		if(resetBeforeClear)
			Reset();

		_requestedSettings.Clear();
	}
}

public interface ISettings
{
	void Reset();
}
