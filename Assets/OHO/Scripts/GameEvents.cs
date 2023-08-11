using UnityEngine;

[CreateAssetMenu(fileName = "New Game Event", menuName = "ScriptableObjects/Create Game Event")]
public class GameEvents : ScriptableObject
{
	public event System.Action<string> OnGameEventInvoked;

	public void RaiseEvent(string response)
	{
		OnGameEventInvoked?.Invoke(response);
	}
}
