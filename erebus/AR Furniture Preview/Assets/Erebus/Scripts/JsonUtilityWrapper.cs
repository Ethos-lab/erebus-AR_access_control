// Reference code : https://gist.github.com/Haosvit/fc6e30b331375b44a9b289fba85103c3#file-jsonhelper-cs
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public static class JsonUtilityWrapper
{
	public static List<T> FromJson<T>(string json)
	{
		ListWrapper<T> wrapper = JsonUtility.FromJson<ListWrapper<T>>(json);
		return wrapper.Data;
	}
	public static string ToJson<T>(List<T> array, bool prettyPrint = false)
	{
		ListWrapper<T> wrapper = new ListWrapper<T>();
		wrapper.Data = array;
		return JsonUtility.ToJson(wrapper, prettyPrint);
	}

	[Serializable]
	private class ListWrapper<T>
	{
		public List<T> Data;
	}
}