using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_GameConstant : MonoBehaviour
{
    public const string BASE_URL = "http://43.205.191.90"; //"http://3.111.178.138"; //"http://15.206.57.137"; //"http://65.0.179.149";
    public const string API_URL = BASE_URL + ":4000/api/v1";
    public const string BASE_URL_SOCKET = BASE_URL; //"http://3.111.178.138"; //"http://65.0.179.149"; // "http://65.1.1.163";
    public const string SOCKET_URL = BASE_URL_SOCKET + ":3001/socket.io";
    public const string LOBBY_SOCKET_URL = BASE_URL_SOCKET + ":4000/socket.io";  //http://3.111.178.138:4000/api/v1:3001/socket.io

    public const float NETWORK_CHECK_DELAY = 2f;
    public const int API_RETRY_LIMIT = 5;
    public const int API_TIME_OUT_LIMIT = 50;

    public const bool enableLog = true;
    public const bool enableErrorLog = true;

    #region ANIMATIONS
    public const float CARD_ANIMATION_DURATION = 0.27f;
    #endregion

    public static string[] GAME_URLS =
    {
        API_URL + "/poker/games",
        API_URL + "/poker/tables/",
        API_URL + "/poker/hand-history/",
        API_URL + "/poker/leaderboard/",
        API_URL + "/poker/game-result/"
        //API_URL + "/game",  //?type=1&varient=2
        //API_URL + "/lobby",
        //API_URL + "/lobby/players",
        //API_URL + "/removePlayer",
        //API_URL + "/game",
        //API_URL + "/getGame",
        //API_URL + "/game/history",
    };
}

[System.Serializable]
public enum P_RequestType
{
    PokerGameList,
    PokerTableList,
    PokerHandHistory,
    PokerLeaderboard,
    RealtimeResult
    //GameVarient,
    //GameVarientForGameId,
    //Lobby,
    //LobbyPlayers,
    //RemovePlayer,
    //Game,
    //GetGame,
    //GameHistory
}
