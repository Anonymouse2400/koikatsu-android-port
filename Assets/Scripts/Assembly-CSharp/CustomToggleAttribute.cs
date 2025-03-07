using UnityEngine;

public class CustomToggleAttribute : PropertyAttribute
{
	public readonly string title;

	public CustomToggleAttribute()
	{
		title = string.Empty;
	}

	public CustomToggleAttribute(string _title)
	{
		title = _title;
	}
}
