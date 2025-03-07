public class ChaFilePreview
{
	public byte sex { get; set; }

	public string lastname { get; set; }

	public string firstname { get; set; }

	public string fullname
	{
		get
		{
			return lastname + " " + firstname;
		}
	}

	public string nickname { get; set; }

	public int personality { get; set; }

	public byte bloodType { get; set; }

	public byte birthMonth { get; set; }

	public byte birthDay { get; set; }

	public byte clubActivities { get; set; }

	public float voicePitch { get; set; }

	public ChaFilePreview()
	{
		sex = 0;
		lastname = string.Empty;
		firstname = string.Empty;
		nickname = string.Empty;
		personality = 0;
		bloodType = 0;
		birthMonth = 0;
		birthDay = 0;
		clubActivities = 0;
		voicePitch = 1f;
	}
}
