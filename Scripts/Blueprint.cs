

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Blueprint
{
    public string itemName;
    public int numberOfItemsToProduce;
    public string Req1;
    public int Req1amount;
    public string Req2;
    public int Req2amount;

    public Blueprint(string itemName, int numberOfItemsToProduce, string req1, int req1amount, string req2, int req2amount)
    {
        this.itemName = itemName;
        this.numberOfItemsToProduce = numberOfItemsToProduce;
        this.Req1 = req1;
        this.Req1amount = req1amount;
        this.Req2 = req2;
        this.Req2amount = req2amount;
    }
}