public struct PlayerData {

    public string PlayerName { get; private set; }
    public ulong ClientId { get; private set; }
    public bool IsKing { get; set; }

    public PlayerData(string playerName, ulong clientId, bool isKing = false)
    {
        PlayerName = playerName;
        ClientId = clientId;
        IsKing = isKing;
    }
}
