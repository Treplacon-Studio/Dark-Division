using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public enum Team
{
    Blue,
    Red
}

public static class TeamManager
{
    public static void AssignTeam(Player player)
    {
        int redTeamCount = GetTeamCount(Team.Red);
        int blueTeamCount = GetTeamCount(Team.Blue);

        Debug.Log($"Red Team Count: {redTeamCount}, Blue Team Count: {blueTeamCount}");

        Team assignedTeam;

        if (redTeamCount < blueTeamCount)
            assignedTeam = Team.Red;
        else if (blueTeamCount < redTeamCount)
            assignedTeam = Team.Blue;
        else
            assignedTeam = (Random.Range(0, 2) == 0) ? Team.Red : Team.Blue;

        Debug.Log($"Assigning {player.NickName} to {assignedTeam} team");

        Hashtable playerProps = new Hashtable
        {
            { "team", assignedTeam }
        };
        player.SetCustomProperties(playerProps);
    }

    public static Team? GetTeam(Player player)
    {
        if (player.CustomProperties.TryGetValue("team", out object team))
            return (Team)team;

        return null;
    }

    public static int GetTeamCount(Team team)
    {
        int count = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Team? playerTeam = GetTeam(player);
            if (playerTeam == team)
                count++;
        }
        return count;
    }


    public static string GetPlayerTeam(Player player)
    {
        Team? team = GetTeam(player);
        if (team.HasValue)
            return team.Value == Team.Red ? "Red" : "Blue";
        else
            return "No Team";
    }
}
