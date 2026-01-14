using System;
using System.Collections.Generic;
using UnityEngine;

public enum Power
{
    Portal,
    Inversion,
    Shock
}

[System.Serializable]
public class PowerSprite
{
    public Power power;
    public Sprite sprite;
}

[CreateAssetMenu(menuName = "Game/Power Sprites")]
public class PowerSpritesData : ScriptableObject
{
    public List<PowerSprite> powerSprites;
    private Dictionary<Power, Sprite> dict;

    public Sprite GetSprite(Power power)
    {
        if (dict == null)
        {
            dict = new Dictionary<Power, Sprite>();
            foreach (var item in powerSprites) {
                dict[item.power] = item.sprite;
            }
        }

        return dict[power];
    }
}
