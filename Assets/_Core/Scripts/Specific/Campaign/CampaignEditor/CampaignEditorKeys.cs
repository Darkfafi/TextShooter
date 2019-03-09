using System.Collections.Generic;
using RDP.SaveLoadSystem;
using System;

namespace GameEditor
{
	public class CampaignEditorKeys : ISaveableLoad
	{
		public event Action<string> KeyAddedEvent;
		public event Action<string> KeyRemovedEvent;

		public const string STORAGE_KEYS_KEY = "KeysKey";

		public string[] AllKeys
		{
			get
			{
				return _allKeys.ToArray();
			}
		}

		private List<string> _allKeys = new List<string>();

		public void Save(IStorageSaver saver)
		{
			saver.SaveValues(STORAGE_KEYS_KEY, AllKeys);
		}

		public void Load(IStorageLoader loader)
		{
			_allKeys.AddRange(loader.LoadValues<string>(STORAGE_KEYS_KEY));
		}

		public void LoadingCompleted()
		{

		}

		public bool HasKey(string key)
		{
			return _allKeys.Contains(key);
		}

		public string[] GetKeysExcluding(params string[] excludingKeys)
		{
			List<string> returnValue = new List<string>(AllKeys);
			for(int i = excludingKeys.Length - 1; i >= 0; i--)
			{
				returnValue.Remove(excludingKeys[i]);
			}
			return returnValue.ToArray();
		}

		public void AddKey(string key)
		{
			if(!HasKey(key))
			{
				_allKeys.Add(key);

				if(KeyAddedEvent != null)
				{
					KeyAddedEvent(key);
				}
			}
		}

		public void RemoveKey(string key)
		{
			if(HasKey(key))
			{
				_allKeys.Remove(key);

				if(KeyRemovedEvent != null)
				{
					KeyRemovedEvent(key);
				}
			}
		}
	}
}