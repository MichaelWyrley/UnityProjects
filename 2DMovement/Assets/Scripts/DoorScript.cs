using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    private List<Coin> coins;
    private int noCoins;
    private int destroyed = 0;

    public void coinCollect(){
        destroyed ++;
        print(destroyed);
    }
    // Start is called before the first frame update
    void Start()
    {
        coins = new List<Coin> (FindObjectsOfType<Coin> ());
        noCoins = coins.Count;
    }

    public void LastCoin() {
        if(noCoins == destroyed){
            GetComponent<Collider>().enabled = false;
            // change to blend
            GetComponent<Shape>().operation = Shape.Operation.Blend;
        }
    }
}
