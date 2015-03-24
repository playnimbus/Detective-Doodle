using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Analytics  {

    public static bool DisableEventsAnalytics = false;

    public enum GameModes{Detective, PropHunt};
    public static GameModes CurrentGameMode;

    public enum PlayerRoles { Bystander, Murderer, Detective };

    public static int DeathOrder = 0;
    public static int ObjectsLooted = 0;
    public static int NumberOfPlayers = 0;
    public static float RoundTimer = 0;
    public static int CluesFound = 0;

    // Game analytics event titles
    private const string LENGTH_OF_ROUND = "Length of Round";
    private const string NUMBER_OF_PLAYERS = "Number of Players";
    private const string NUMBER_OF_PLAYERS_KILLED = "Number of Players Killed";
    private const string TOTAL_OBJECTS_LOOTED = "Total Objects Looted";
    private const string TOTAL_CLUES_FOUND = "Total Clues Found";
    private const string BYSTANDER_VICTORY = "Bystander Victory";
    private const string DEATH_NUMBER = "Death Number";
    private const string GA_TRUE = "true";
    private const string GA_FALSE = "false";
    private const string OBJECTS_LOOTED = "Object Looted";
    private const string PLAYER_ACCUSED = "Player Accused";


    public static void Initialize(Analytics.GameModes gameMode, int numPlayers)
    {
        DeathOrder = 0;
        ObjectsLooted = 0;
        NumberOfPlayers = 0;
        RoundTimer = 0;
        CluesFound = 0;

        CurrentGameMode = gameMode;
        NumberOfPlayers = numPlayers;
    }

    //call when win condition is reached by either team
    public static void SendGameModeEnd(bool bystanderVictory)
    {
        if (DisableEventsAnalytics == false)
        {
            GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + LENGTH_OF_ROUND, RoundTimer);
            GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + NUMBER_OF_PLAYERS, NumberOfPlayers);
            GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + NUMBER_OF_PLAYERS_KILLED, DeathOrder);
            GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + TOTAL_OBJECTS_LOOTED, ObjectsLooted);
            GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + TOTAL_CLUES_FOUND, CluesFound);

            if (bystanderVictory){
                GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + BYSTANDER_VICTORY + ":" + GA_TRUE, System.Convert.ToSingle(bystanderVictory));
            }else{
                GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + BYSTANDER_VICTORY + ":" + GA_FALSE, System.Convert.ToSingle(bystanderVictory));
            }
        }
    }

    public static void PlayerDied(PlayerRoles playerRole)
    {
        DeathOrder++;
        if (DisableEventsAnalytics == false){
            GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + playerRole.ToString() + ":" + DEATH_NUMBER + DeathOrder.ToString(), RoundTimer);
        }
    }

    public static void ObjectLooted(bool clueFound)
    {
        ObjectsLooted++;

        if (clueFound){
            CluesFound++;
        }

        if (DisableEventsAnalytics == false){
            GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + OBJECTS_LOOTED, System.Convert.ToSingle(clueFound));
        }
    }

    public static void PlayerAccused(bool isMurderer)
    {
        if (DisableEventsAnalytics == false)
        {
            if (isMurderer)
            {
                GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + PLAYER_ACCUSED + ":" + GA_TRUE, RoundTimer);
            }
            else
            {
                GA.API.Design.NewEvent(CurrentGameMode.ToString() + ":" + PLAYER_ACCUSED + ":" + GA_FALSE, RoundTimer);
            }
        }
    }
}
