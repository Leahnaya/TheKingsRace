public struct PlayerData {

    public string PlayerName { get; private set; }
    public ulong ClientId { get; private set; }
    public bool IsKing { get; set; }
    public PlayerInventory pInv{get; set;}

    public PlayerData(string playerName, ulong clientId, bool isKing = false)
    {
        PlayerName = playerName;
        pInv = new PlayerInventory();
        ClientId = clientId;
        IsKing = isKing;
    }
}
