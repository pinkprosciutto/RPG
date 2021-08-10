using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
   
    public CharacterManager Character { get; set; }
    [SerializeField] bool isPlayer;
    [SerializeField] Hud hud;

    public bool IsPlayer
    {
        get { return isPlayer; }
    }

    public Hud _Hud
    {
        get { return hud; }
    }

    Image img;
    public Sprite toast;
    Color originalColor;

    void Awake()
    {
        img = GetComponent<Image>();
        originalColor = img.color;
    }

    public void Setup(CharacterManager character)
    {
        Character = character;
        img.sprite = Character.CharacterBase.CharacterSprite;

        hud.SetData(character);
        
    }

    public void HitAnimation()
    {
        StartCoroutine(Blink(5, 0.005f));
    }

    public void DeathAnimation()
    {
        //img.color = Color.black;
        img.sprite = toast;
    }

    IEnumerator Blink(int numBlink, float blinkFrequency)
    {
        img.color = Color.gray;
        //wait for a bit
        yield return new WaitForSeconds(blinkFrequency);
        for (int x = 0; x < numBlink * 2; x++)
        {
            //toggle renderer
            img.color = Color.gray;
            //wait for a bit
            yield return new WaitForSeconds(blinkFrequency);
            img.color = originalColor;
        }

        img.color = originalColor;
    }
}
