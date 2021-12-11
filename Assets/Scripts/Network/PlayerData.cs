public struct PlayerData {

    public string PlayerName { get; private set; }
    public ulong ClientId { get; private set; }
    public bool IsKing { get; set; }
    public PlayerInventory pInv{get; set;}

    public bool Finished { get; set; }

    public PlayerData(string playerName, ulong clientId, bool isKing = false, bool finished = false)
    {
        PlayerName = playerName;
        pInv = new PlayerInventory();
        ClientId = clientId;
        IsKing = isKing;
        Finished = finished;
    }
}
