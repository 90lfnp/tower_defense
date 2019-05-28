using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPerfab;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    [SerializeField]
    private Text priceTxt;


    public GameObject TowerPerfab
    {
        get
        {
            return towerPerfab;
        }
    }

    public Sprite Sprite
    {
        get
        {
            return sprite; 
        }
    }

    public int Price
    {
        get
        {
            return price;
        }
    }

    private void Start()
    {
        priceTxt.text = Price + "<color=yellow>$</color>";

        GameManager.Instance.Changed += new CurrencyChanged(PriceCheck);
    }

    private void PriceCheck()
    {
        if (price <= GameManager.Instance.Currency)
        {
            GetComponent<Image>().color = Color.white;
            priceTxt.color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = Color.gray;
            priceTxt.color = Color.gray;
        }
    }
}
