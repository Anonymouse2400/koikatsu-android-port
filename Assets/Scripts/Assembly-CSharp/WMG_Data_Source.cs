using System;
using System.Collections.Generic;
using UnityEngine;

public class WMG_Data_Source : MonoBehaviour
{
	public enum WMG_DataSourceTypes
	{
		Single_Object_Multiple_Variables = 0,
		Multiple_Objects_Single_Variable = 1,
		Single_Object_Single_Variable = 2
	}

	public enum WMG_VariableTypes
	{
		Not_Specified = 0,
		Field = 1,
		Property = 2,
		Property_Field = 3,
		Field_Field = 4
	}

	public WMG_DataSourceTypes dataSourceType;

	public List<object> dataProviders = new List<object>();

	public object dataProvider;

	public List<WMG_VariableTypes> variableTypes = new List<WMG_VariableTypes>();

	public WMG_VariableTypes variableType;

	public List<string> variableNames;

	public string variableName;

	private List<string> varNames1 = new List<string>();

	private string varName1;

	private List<string> varNames2 = new List<string>();

	private string varName2;

	private void Start()
	{
		if (variableNames == null)
		{
			variableNames = new List<string>();
		}
		parseStrings();
	}

	private void parseStrings()
	{
		varNames1.Clear();
		varNames2.Clear();
		for (int i = 0; i < variableNames.Count; i++)
		{
			string stringPart = string.Empty;
			string stringPart2 = string.Empty;
			parseString(variableNames[i], ref stringPart, ref stringPart2);
			varNames1.Add(stringPart);
			varNames2.Add(stringPart2);
		}
		parseString(variableName, ref varName1, ref varName2);
	}

	private void parseString(string inputString, ref string stringPart1, ref string stringPart2)
	{
		string[] array = inputString.Split('.');
		stringPart1 = string.Empty;
		stringPart2 = string.Empty;
		if (array.Length >= 2)
		{
			stringPart1 = array[0];
			stringPart2 = array[1];
		}
	}

	public void setVariableNames(List<string> variableNames)
	{
		this.variableNames.Clear();
		foreach (string variableName in variableNames)
		{
			this.variableNames.Add(variableName);
		}
		parseStrings();
	}

	public void setVariableName(string variableName)
	{
		this.variableName = variableName;
		parseString(variableName, ref varName1, ref varName2);
	}

	public void addVariableNameToList(string variableName)
	{
		variableNames.Add(variableName);
		parseStrings();
	}

	public void removeVariableNameFromList(string variableName)
	{
		variableNames.Remove(variableName);
		parseStrings();
	}

	public void setDataProviders<T>(List<T> dataProviderList)
	{
		dataProviders.Clear();
		foreach (T dataProvider in dataProviderList)
		{
			dataProviders.Add(dataProvider);
		}
	}

	public void setDataProvider<T>(T dataProvider)
	{
		this.dataProvider = dataProvider;
	}

	public void addDataProviderToList<T>(T dataProvider)
	{
		dataProviders.Add(dataProvider);
	}

	public bool removeDataProviderFromList<T>(T dataProvider)
	{
		return dataProviders.Remove(dataProvider);
	}

	public List<T> getData<T>()
	{
		if (dataSourceType == WMG_DataSourceTypes.Multiple_Objects_Single_Variable)
		{
			List<T> list = new List<T>();
			{
				foreach (object dataProvider in dataProviders)
				{
					list.Add(getDatum<T>(dataProvider, variableName, variableType, varName1, varName2));
				}
				return list;
			}
		}
		if (dataSourceType == WMG_DataSourceTypes.Single_Object_Multiple_Variables)
		{
			List<T> list2 = new List<T>();
			for (int i = 0; i < variableNames.Count; i++)
			{
				string text = variableNames[i];
				WMG_VariableTypes varType = WMG_VariableTypes.Not_Specified;
				if (i < variableTypes.Count)
				{
					varType = variableTypes[i];
				}
				if (i >= varNames1.Count)
				{
					parseStrings();
				}
				list2.Add(getDatum<T>(this.dataProvider, text, varType, varNames1[i], varNames2[i]));
			}
			return list2;
		}
		if (dataSourceType == WMG_DataSourceTypes.Single_Object_Single_Variable)
		{
			try
			{
				return (List<T>)WMG_Reflection.GetField(this.dataProvider.GetType(), variableName).GetValue(this.dataProvider);
			}
			catch (Exception)
			{
				return new List<T>();
			}
		}
		return new List<T>();
	}

	public T getDatum<T>()
	{
		if (dataSourceType == WMG_DataSourceTypes.Single_Object_Single_Variable)
		{
			return getDatum<T>(dataProvider, variableName, variableType, varName1, varName2);
		}
		return default(T);
	}

	private T getDatum<T>(object dp, string variableName, WMG_VariableTypes varType, string vName1, string vName2)
	{
		switch (varType)
		{
		case WMG_VariableTypes.Field:
			try
			{
				return (T)WMG_Reflection.GetField(dp.GetType(), variableName).GetValue(dp);
			}
			catch (Exception)
			{
				return default(T);
			}
		case WMG_VariableTypes.Property:
			try
			{
				return (T)WMG_Reflection.GetProperty(dp.GetType(), variableName).GetValue(dp, null);
			}
			catch (Exception)
			{
				return default(T);
			}
		case WMG_VariableTypes.Property_Field:
			try
			{
				object value4 = WMG_Reflection.GetProperty(dp.GetType(), vName1).GetValue(dp, null);
				return (T)WMG_Reflection.GetField(value4.GetType(), vName2).GetValue(value4);
			}
			catch (Exception)
			{
				return default(T);
			}
		case WMG_VariableTypes.Field_Field:
			try
			{
				object value3 = WMG_Reflection.GetField(dp.GetType(), vName1).GetValue(dp);
				return (T)WMG_Reflection.GetField(value3.GetType(), vName2).GetValue(value3);
			}
			catch (Exception)
			{
				return default(T);
			}
		case WMG_VariableTypes.Not_Specified:
			try
			{
				return (T)WMG_Reflection.GetField(dp.GetType(), variableName).GetValue(dp);
			}
			catch
			{
				try
				{
					return (T)WMG_Reflection.GetProperty(dp.GetType(), variableName).GetValue(dp, null);
				}
				catch
				{
					try
					{
						object value = WMG_Reflection.GetProperty(dp.GetType(), vName1).GetValue(dp, null);
						return (T)WMG_Reflection.GetField(value.GetType(), vName2).GetValue(value);
					}
					catch
					{
						try
						{
							object value2 = WMG_Reflection.GetField(dp.GetType(), varName1).GetValue(dp);
							return (T)WMG_Reflection.GetField(value2.GetType(), vName2).GetValue(value2);
						}
						catch (Exception)
						{
							return default(T);
						}
					}
				}
			}
		default:
			return default(T);
		}
	}
}
