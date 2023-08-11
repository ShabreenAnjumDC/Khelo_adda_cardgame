using System;
using System.Collections.Generic;
using LitJson;
//using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class WalletScreen : MonoBehaviour
{
	public static WalletScreen instance;

	[SerializeField] Text walletBalanceText, winAmountText, depositAmountText, bonusAmountText;
	[SerializeField] GameObject transactionsPanel;
	[SerializeField] Transform transactionScrollContent;
	[SerializeField] GameObject depositTransactionItemPrefab, withdrawalTransactionItemPrefab, cashbackTransactionItemPrefab;
	[SerializeField] GameEvents walletGameEvent;

	List<TransactionItem> transactionItems = new List<TransactionItem>();
	public GameObject UniWebViewObject;
	UniWebView UniWebView;


	private void Awake()
    {
		instance = this;
		UniWebView = UniWebViewObject.GetComponent<UniWebView>();
	}

    public void InitialiseWebView(string payLink)
	{
		UniWebViewObject.SetActive(true);
		UniWebView.SetShowSpinnerWhileLoading(true);
		UniWebView.SetSpinnerText("Loading. Please do not close or go back.");
		UniWebView.Load(payLink);
		UniWebView.Show(true);
	}

	public void CloseWebView()
	{
		UniWebViewObject.SetActive(false);
		UniWebView.Hide(true);
	}

	private void OnEnable()
	{
		UniWebView.OnMessageReceived += (view, message) => {
			Debug.Log("Message " + message.Path);
			MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.AddCash);
			Debug.Log("Message AddCashScreenDestroyed");
			if (message.Path.Equals("close"))
			{
				GetWalletDetails();
				UniWebViewObject.SetActive(false);
			}
		};

		transactionsPanel.SetActive(false);
		GetWalletDetails();
		walletGameEvent.OnGameEventInvoked += OnWalletEventInvoked;
	}

	private void OnDisable()
	{
		walletGameEvent.OnGameEventInvoked -= OnWalletEventInvoked;
	}

	private void OnWalletEventInvoked(string serverResponse)
	{
		JsonData data = JsonMapper.ToObject(serverResponse);

		walletBalanceText.text = depositAmountText.text = (data["data"]["real_amount"]).ToString();
		winAmountText.text = (data["data"]["win_amount"]).ToString();
		bonusAmountText.text = (data["data"]["bonus_amount"]).ToString();
		
	}

	private void GetWalletDetails()
	{
		StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/get-wallet", WalletDetailsResponse));
	}

	private void WalletDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
	{
		print("Wallet Response : " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		if (data["statusCode"].ToString() == "200")
		{
			walletBalanceText.text = depositAmountText.text = "<size=21>₹</size> " + (data["data"]["real_amount"]).ToString();
			winAmountText.text = "<size=21>₹</size> " + (data["data"]["win_amount"]).ToString();
			bonusAmountText.text = "<size=21>₹</size> " + (data["data"]["bonus_amount"]).ToString();
		}
		else
		{
			Debug.LogError(data["error"].ToString());
		}
	}

	public void RemovePreviousTransactionItems()
	{
		for (int i = 0; i < transactionScrollContent.childCount; i++)
		{
			Destroy(transactionScrollContent.GetChild(i).gameObject);
		}

		transactionItems.Clear();
	}

	public void GetTransactionDetails()
	{
		StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/my-transactions", TransactionDetailsResponse));
	}

	private void TransactionDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
	{
		print("Transaction Details Response : " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		for (int i = 0; i < data["data"].Count; i++)
		{
			TransactionItem transactionItem = CreateTransactionItems((data["data"][i]["other_type"]).ToString());
			transactionItems.Add(transactionItem);
			transactionItem.SetTransactionDetails((int)data["data"][i]["transaction_id"],
					(int)data["data"][i]["user_id"],
					data["data"][i]["other_type"].ToString(),
					data["data"][i]["amount"].ToString(),
					data["data"][i]["category"].ToString(),
					data["data"][i]["createdAt"].ToString(),
					data["data"][i]["updatedAt"].ToString());

			transactionItem.ShowTransactionDetails();

		}
	}

	private TransactionItem CreateTransactionItems(string typeOfTransaction)
	{
		TransactionItem transactionItem;
		switch (typeOfTransaction) 
		{
			case "Deposit":
				transactionItem = Instantiate(depositTransactionItemPrefab, transactionScrollContent).
					GetComponent<TransactionItem>();
				break;
			case "Withdraw":
				transactionItem = Instantiate(withdrawalTransactionItemPrefab, transactionScrollContent).
					GetComponent<TransactionItem>();
				break;
			case "Cashback":
				transactionItem = Instantiate(cashbackTransactionItemPrefab, transactionScrollContent).
					GetComponent<TransactionItem>(); 
				break;
			default:
				transactionItem = Instantiate(depositTransactionItemPrefab, transactionScrollContent).
					GetComponent<TransactionItem>();
				break;

		}
		
		return transactionItem;
	}

	public void OnDepositToggleChanged(Toggle toggle)
	{
		foreach (var item in transactionItems)
		{
			if (item.typeOfTransaction.Equals("Deposit"))
			{
				item.gameObject.SetActive(toggle.isOn);
			}
		}
	}

	public void OnWithdrawToggleChanged(Toggle toggle)
	{
		foreach (var item in transactionItems)
		{
			if (item.typeOfTransaction.Equals("Withdraw"))
			{
				item.gameObject.SetActive(toggle.isOn);
			}
		}
	}

	public void OnCashbackToggleChanged(Toggle toggle)
	{
		foreach (var item in transactionItems)
		{
			if (item.typeOfTransaction.Equals("Cashback"))
			{
				item.gameObject.SetActive(toggle.isOn);
			}
		}
	}

	public void WithdrawUserCash()
	{
		string requestData = "{\"amount\":\"" + "10" + "\"}";

		WebServices.instance.SendRequest(RequestType.WithdrawRequest, requestData, true, WithdrawCashResponse);
	}

	void WithdrawCashResponse(RequestType requestType, string serverResponse, bool isErrorMessage, string errorMessage)
	{
		Debug.Log("Response => Withdraw: " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);	

		if (data["statusCode"].ToString() == "200")
		{
			//MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
			////MainMenuController.instance.ShowMessage(data["message"].ToString());
			//InitialiseWebView(data["data"].ToString());
		}
		else
		{
			Debug.LogError(data["message"].ToString());
			//MainMenuController.instance.ShowMessage(data["error"].ToString());
		}
	}
}
