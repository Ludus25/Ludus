
using MatchmakingService.Entities; 
using System;
using System.Collections.Generic;

namespace MatchmakingService.Entities;

public class Match
{
    public string MatchId { get; private set; } = Guid.NewGuid().ToString();
    public List<PlayerInQueue> Players { get; private set; }
    public string Status { get; private set; } = "ready";

    public Match(IEnumerable<PlayerInQueue> players)
    {
        Players = new List<PlayerInQueue>(players);
    }
}

