using MLAPI.Serialization;

public struct LobbyPlayerState : INetworkSerializable {
    public ulong ClientId;
    public string PlayerName;
    public bool IsReady;
    public bool IsKing;

    public LobbyPlayerState(ulong clientId, string playerName, bool isReady, bool isKing)
    {
        ClientId = clientId;
        PlayerName = playerName;
        IsReady = isReady;
        IsKing = isKing;
    }

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref ClientId);
        serializer.Serialize(ref PlayerName);
        serializer.Serialize(ref IsReady);
        serializer.Serialize(ref IsKing);
    }
}
