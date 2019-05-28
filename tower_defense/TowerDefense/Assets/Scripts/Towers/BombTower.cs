using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTower : Tower
{
    private void Start()
    {
        ElementType = Element.BOMB;
    }
}
