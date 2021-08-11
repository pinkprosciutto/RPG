using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> WalkUpSprite;
    [SerializeField] List<Sprite> WalkDownSprite;
    [SerializeField] List<Sprite> WalkLeftSprite;
    [SerializeField] List<Sprite> WalkRightSprite;
    //parameter
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    //states
    CustomSpriteAnimator WalkUp;
    CustomSpriteAnimator WalkDown;
    CustomSpriteAnimator WalkLeft;
    CustomSpriteAnimator WalkRight;

    //reference
    SpriteRenderer spriteRenderer;

    CustomSpriteAnimator currentAnimation;
    bool wasMovingPreviously;
    public float frameRate;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        WalkUp = new CustomSpriteAnimator(WalkUpSprite, spriteRenderer, frameRate);
        WalkDown = new CustomSpriteAnimator(WalkDownSprite, spriteRenderer, frameRate);
        WalkLeft = new CustomSpriteAnimator(WalkLeftSprite, spriteRenderer, frameRate);
        WalkRight = new CustomSpriteAnimator(WalkRightSprite, spriteRenderer, frameRate);

        currentAnimation = WalkDown;
    }

    void Update()
    {
        var previousAnimation = currentAnimation;

        if (MoveY == 1)
            currentAnimation = WalkUp;
        else if (MoveY == -1)
            currentAnimation = WalkDown;
        else if (MoveX == -1)
            currentAnimation = WalkLeft;
        else if (MoveX == 1)
            currentAnimation = WalkRight;

        if (currentAnimation != previousAnimation || IsMoving != wasMovingPreviously)
            currentAnimation.Start();

        if (IsMoving)
            currentAnimation.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnimation.Frames[1];

        wasMovingPreviously = IsMoving;
    }


}
