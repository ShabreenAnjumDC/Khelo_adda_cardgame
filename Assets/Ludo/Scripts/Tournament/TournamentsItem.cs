using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TournamentsItem : MonoBehaviour
{
    public TMP_Text enterAmountTMP;
    public TMP_Text playersTMP;
    public TMP_Text tournyNameTMP;
    public TMP_Text regButtonTMP;
    public TMP_Text amountTMP;
    public TMP_Text startsTMP;
    public TMP_Text startsLabel;
    public TMP_Text durationTMP;
    public TMP_Text playersOutOfTMP;
    public Button regButton;
    public Image regButtonImage;
    public string id;
    public enum callFromEnum { upcoming, registered, finished }
    public string callFrom = string.Empty; // upcoming, registered, finished
    public string gameTypeId;


    public void TimerStart(DateTime tournDateTime, int totalSeconds, TMP_Text startsTMPTemp, DateTime serverCurrentTime)
    {
        StartCoroutine(TournamentStartTimer(tournDateTime, totalSeconds, startsTMPTemp, serverCurrentTime,(myReturnValue) =>
        {
            if (myReturnValue)
            {
                Debug.Log("Timer Complete");
                if (callFrom == "upcoming")
                {
                    regButton.onClick.RemoveAllListeners();
                    startsTMP.text = "";
                }
                else if (callFrom == "registered")
                {
                    regButton.onClick.RemoveAllListeners();
                    //startsLabel.text = "ENDED";
                    startsTMP.text = "";
                }
            }
        }));
    }



    //Coroutine for timer of gameStart
    //finalEndDateTime, totalSecondsScheduleDateFinalEnd, startsTMPTemp, today
    IEnumerator TournamentStartTimer(DateTime tournDateTime, int totalSeconds, TMP_Text startsTMPTemp, DateTime serverCurrentTime, System.Action<bool> callback)
    {
        //Debug.Log("Schedule time is coming in " + Math.Abs(differenceScheduleDate.Days) + "D " + Math.Abs(differenceScheduleDate.Hours) + "H " + Math.Abs(differenceScheduleDate.Minutes) + "M " + Math.Abs(differenceScheduleDate.Seconds) + "S ");

        int totalSecondsAbs = Math.Abs(totalSeconds);
        DateTime tempServerCurrentTime = serverCurrentTime;

        for (int i = totalSecondsAbs; i >= 0; i--)     //(int i = totalSecondsAbs; i <= 0; i++)  //for (int i = (totalSeconds * -1); i >= 0; i--)
        {
            TimeSpan difference1 = tournDateTime.Subtract(tempServerCurrentTime); //(DateTime.Now); //DateTime.Now.Subtract(tournDateTime);
            // difference1.Days difference1.TotalSeconds

            if (i == 0)
            {
                if (difference1.Days > 0)
                    startsTMPTemp.text = difference1.Days.ToString("D2") + "D " + difference1.Hours.ToString("D2") + "H " + difference1.Minutes.ToString("D2") + "M " + difference1.Seconds.ToString("D2") + "S ";
                else
                    startsTMPTemp.text = difference1.Hours.ToString("D2") + "H " + difference1.Minutes.ToString("D2") + "M " + difference1.Seconds.ToString("D2") + "S ";
 
                //startsTMPTemp.text = (tournDateTime.ToLocalTime()).ToString(@"HH':'mm");
                yield return null;
                callback(true);
                break;
            }
            else
            {
                try
                {
                    if (difference1.Days > 0)
                        startsTMPTemp.text = difference1.Days.ToString("D2") + "D " + difference1.Hours.ToString("D2") + "H " + difference1.Minutes.ToString("D2") + "M " + difference1.Seconds.ToString("D2") + "S ";
                    else
                        startsTMPTemp.text = difference1.Hours.ToString("D2") + "H " + difference1.Minutes.ToString("D2") + "M " + difference1.Seconds.ToString("D2") + "S ";
                }
                catch (Exception e)
                {
                    Debug.Log("error in timerText: " + e.Message);
                }
                yield return new WaitForSecondsRealtime(1f);
                tempServerCurrentTime = tempServerCurrentTime.AddSeconds(1f);
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
