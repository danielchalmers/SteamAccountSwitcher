namespace SteamAccountSwitcher
{
	public record class SteamAccount
	{
		public string ID { get; set; }

		public string Name { get; set; }

		public string Alias { get; set; }

		public override string ToString() => Alias ?? Name;
	}
}