namespace XOGameService.API.Dtos
{
    public class MakeMoveDto
    {
        public required int CellIndex { get; set; }
        public required int Version { get; set; }

    }
}
