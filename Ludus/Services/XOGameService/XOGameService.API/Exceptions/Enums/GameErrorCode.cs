namespace XOGameService.API.Exceptions.Enums
{
    public enum GameErrorCode
    {
        GameNotFound = 10001,
        NotYourTurn = 10002,
        NotParticipant = 10003,
        CellTaken = 10004,
        GameAlreadyFinished = 10005,
        VersionConflict = 10006,
        InvalidCellIndex = 10007
    }
}
