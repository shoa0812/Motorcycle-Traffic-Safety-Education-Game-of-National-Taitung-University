using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NumberIncreaser : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    int counter;

    public void ButtonPressed()
    {
        counter++;
        numberText.text = counter + "";
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
