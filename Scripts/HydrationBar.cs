using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HydrationBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI hydrationCounter;
    public GameObject playerState;
    private float currentHydration, maxHydration;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        currentHydration = Mathf.Clamp(playerState.GetComponent<PlayerState>().currentHydrationPercent, 0, 100);
        maxHydration = Mathf.Clamp(playerState.GetComponent<PlayerState>().maxHydrationPercent, 0, 100);

        float fillValue = currentHydration / maxHydration;
        slider.value = fillValue;

        hydrationCounter.text = currentHydration + "/" + maxHydration;
    }
}