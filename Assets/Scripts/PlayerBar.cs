using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BarType
{
    healthBar,
    manaBar
}
public class PlayerBar : MonoBehaviour
{

    private Slider slider;
    public BarType type;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();

        switch (type)
        {
            case BarType.healthBar:
                slider.maxValue = PlayerController.MAX_HEALTH; // Al ser una constante no se accede por componente
                break;
            case BarType.manaBar:
                slider.value = PlayerController.MAX_MANA; // Al ser una constante no se accede por componente
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case BarType.healthBar:
                slider.value = GameObject.Find("Player").GetComponent<PlayerController>().GetHealth();
                break;
            case BarType.manaBar:
                slider.value = GameObject.Find("Player").GetComponent<PlayerController>().GetMana();
                break;
        }
    }
}
