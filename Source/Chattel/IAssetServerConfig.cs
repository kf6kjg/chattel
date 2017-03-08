namespace Chattel {
	public interface IAssetServerConfig {
		AssetServerType Type { get; }
		string Name { get; }
	}

	public enum AssetServerType {
		WHIP,
		CF
	}
}