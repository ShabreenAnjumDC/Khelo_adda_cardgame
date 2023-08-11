using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimation : MonoBehaviour
{
    public static UIAnimation instance;

    public Image[] allImages;

    void Awake()
    {
        instance = this;    
    }

    public IEnumerator PanelAnimation()
    {
        for (int i = 0; i < allImages.Length; i++)
        {
            if (i != 0)
            {
                yield return new WaitForSeconds(0.10f);
            }

            allImages[i].GetComponentInParent<CanvasGroup>().DOFade(1, 0.25f);
            allImages[i].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.4f);

            if (allImages[i].transform.GetChild(1).GetComponent<Animator>() != null)
            {
                allImages[i].transform.GetChild(1).GetComponent<Animator>().SetBool("tokenAnim", true);
            }
        }
    }
}
