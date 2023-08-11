using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_GameConstant : MonoBehaviour
{
    //public const string BASE_URL = "http://13.233.156.52";
    public const string BASE_URL = "http://3.111.178.138";
    public const string API_URL = BASE_URL + ":4000/api/v1";
    public const string SOCKET_URL = BASE_URL + ":5000/socket.io"; //":3003";
    public const string TOURNAMENT_SOCKET_URL = BASE_URL + ":3002/socket.io";

    public const float NETWORK_CHECK_DELAY = 2f;
    public const int API_RETRY_LIMIT = 5;
    public const int API_TIME_OUT_LIMIT = 50;

    public static string[] GAME_URLS =
    {
        API_URL + "/signup",
        API_URL + "/verify-signup",
        API_URL + "/send-otp",
        API_URL + "/resendOtp/", //mobileno
        API_URL + "/login",
        API_URL + "/verify-otp",
        API_URL + "/forgot-password",
        API_URL + "/user/exist",
        API_URL + "/user/profile",
        API_URL + "/user/", //to update user details /user/id
        API_URL + "/user/send-otp/", // for email verification /user/send-otp/email
        API_URL + "/user/verify-otp/", // for email verify
        API_URL + "/game/type",
        API_URL + "/game/varient",
        API_URL + "/game",  //?type=1&varient=2
        API_URL + "/lobby",
        API_URL + "/lobby/players",
        API_URL + "/removePlayer",
        API_URL + "/game",
        API_URL + "/getGame",
        API_URL + "/game/history",
        API_URL + "/user/profile-picture",
        API_URL + "/game/leaderboard",
        API_URL + "/game/upcomming/tournaments/", //:userId
        API_URL + "/game/registered/tournaments/", //:userId
        API_URL + "/game/finished/tournaments/", //:userId
        API_URL + "/user/game/info/", //:userId

    };
}

[System.Serializable]
public enum L_RequestType
{
    RegisterGame,
    VerifyRegOtp,
    SendRegOtp,
    ResendOtp,
    Login,
    VerifyFPWOtp,
    ForgotPassword,
    CheckUserExist,
    Profile,
    UpdateUser,  // /user/id
    EmailSendOtp,
    EmailVerifyOtp,
    GameType,
    GameVarient,
    GameVarientForGameId,
    Lobby,
    LobbyPlayers,
    RemovePlayer,
    Game,
    GetGame,
    GameHistory,
    UploadProfilePicture,
    Leaderboard,
    TournamentUpcoming,
    TournamentRegistered,
    TournamentFinished,
    GameInfo,
}
