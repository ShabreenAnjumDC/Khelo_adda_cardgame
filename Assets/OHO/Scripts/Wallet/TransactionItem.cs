using UnityEngine;
using UnityEngine.UI;

public class TransactionItem : MonoBehaviour
{
	int transactionId, userId;
	string amount, category, createdAt, updatedAt;
	public string typeOfTransaction;
	[SerializeField] Text categoryText, amountText, dateText;

	// Start is called before the first frame update
	void Start()
	{

	}

	public void SetTransactionDetails(int transactionId, int userId, string typeOfTransaction,
		string amount, string category, string createdAt, string updatedAt)
	{
		this.transactionId = transactionId;
		this.userId = userId;
		this.typeOfTransaction = typeOfTransaction;
		this.amount = amount;
		this.category = category;
		this.createdAt = createdAt;
		this.updatedAt = updatedAt;
	}

	public void ShowTransactionDetails()
	{
		categoryText.text = category;
		if (typeOfTransaction.Equals("Deposit") || typeOfTransaction.Equals("Cashback"))
		{
			amountText.text = "+Rs." + amount;
		}
		else if (typeOfTransaction.Equals("Withdraw"))
		{
			amountText.text = "-Rs." + amount;
		}
		dateText.text = createdAt.Substring(0, 10);
	}
}
