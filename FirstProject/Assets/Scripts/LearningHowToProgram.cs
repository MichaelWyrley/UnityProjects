using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningHowToProgram : MonoBehaviour
{
    float speed = 5.0f;
    double mana = 15.5;
    int health = 100;
    string playerName = "Warrior";
    bool isDead = true;
    char oneChar = 'a';
    

    private void Start() {
        int a = 10;
        int b = 5;
        int c = a + b;

        calculateTwoNumbers();
        calculateTwoNumbers(a,b);
        Debug.Log("The sum is: " + returnSum(a,b));
    

        Debug.Log("A + B = " + c);

        if (a >= 10){
            Debug.Log("a is >= 10");
        } else if (a >= 20 && a <= 30){
            Debug.Log("a is between 20 and 30");
        }

        for(int i = 0; i <= 10; i++){
            Debug.Log("This is " + i + " of the loop");
        }

        StartCoroutine(ExecuteSomething());

        Player warrior = new Player();

    }

    void calculateTwoNumbers(){
        Debug.Log("Printed From Function");
    }
    void calculateTwoNumbers(float a, float b){
        Debug.Log("The sum of a and b is: " + (a+b));
    }

    float returnSum(float a,float b) {
        return a+b;
    }

    IEnumerator ExecuteSomething(){
        yield return new WaitForSeconds(2f);
        Debug.Log("Something is executed");
        
    }
    
} 