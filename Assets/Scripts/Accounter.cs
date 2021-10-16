using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Accounter : MonoBehaviour
{
    [Header("Monny")]
    public Slider MonnySlider;
    public int MaxAmmount;
    private int CurrentAmmount = 100;

    // Start is called before the first frame update
    void Start()
    {
        MonnySlider.maxValue = MaxAmmount;
        
    }

    // Update is called once per frame
    void Update()
    {
        MonnySlider.value = CurrentAmmount;
    }

    public void PayBill(int ammount)
    {
        CurrentAmmount -= ammount;
        if (ammount < 0) CurrentAmmount = 0;

    }
    public int GetCurrentMonnyAmmount()
    {
        return CurrentAmmount;
    }
}
