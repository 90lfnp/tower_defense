using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPerfab;

    [SerializeField]
    private Sprite sprite;

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
}
