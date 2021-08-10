using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State { Start, PlayerTurn, PlayerMove, EnemyMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleManager player;
    [SerializeField] BattleManager enemy;
    [SerializeField] Hud playerHud;
    [SerializeField] Hud enemyHud;
    [SerializeField] BattleDialogue dialogueBox;
    [SerializeField] List<Image> image;

    int currentAction;
    int currentAbility;
    State state;

    private void Start()
    {
        StartCoroutine(SetupBattle());
        Debug.Log("Ready");
        //image[0].gameObject.SetActive(true);
    }

    public IEnumerator SetupBattle()
    {
        player.Setup();
        enemy.Setup();
        playerHud.SetData(player.Character);
        enemyHud.SetData(enemy.Character);

        dialogueBox.SetAbility(player.Character.charAbility);

        yield return dialogueBox.TypeDialogue($"{enemy.Character.CharacterBase.GetName} challenges you!");

        PlayerTurn();
    }

    void PlayerTurn()
    {
        state = State.PlayerTurn;
        int x = Random.Range(1, 10);
        switch (x)
        {
            case 1:
                StartCoroutine(dialogueBox.TypeDialogue("Wonder what's for lunch."));
                break;

            case 2:
                StartCoroutine(dialogueBox.TypeDialogue($"You felt {enemy.Character.CharacterBase.GetName} crawling at your skin."));
                break;

            case 3:
                StartCoroutine(dialogueBox.TypeDialogue("Praise be to Lord Prosciutto!"));
                break;

            case 4:
                StartCoroutine(dialogueBox.TypeDialogue("Just Monika."));
                break;

            case 5: case 6: case 7: case 8: case 9: case 10:
                StartCoroutine(dialogueBox.TypeDialogue("It is your turn."));
                break;
        }
        dialogueBox.EnableActionSelect(true);
    }

    IEnumerator EnemyTurn()
    {
        state = State.EnemyMove;

        var ability = enemy.Character.GetAbilityForEnemy();
        yield return dialogueBox.TypeDialogue($"{enemy.Character.CharacterBase.GetName} used {ability.Ability.GetName}");

        var damageDetail = player.Character.TakeDamage(ability, player.Character);
        yield return playerHud.UpdateHealth();
        yield return ShowDamage(damageDetail);

        if (damageDetail.Fainted)
        {
            yield return dialogueBox.TypeDialogue($"{player.Character.CharacterBase.GetName} is downed");
        }
        else
        {
            PlayerTurn();
        }
    }

    IEnumerator ShowDamage(DamageDetail damageDetail)
    {
        if (damageDetail.Critical > 1f)
            yield return dialogueBox.TypeDialogue("Strikes right through the heart!");
        
        if(damageDetail.Element > 1f)
        {
            yield return dialogueBox.TypeDialogue("It's super effective!");
        } 
        else if (damageDetail.Element < 1f)
        {
            yield return dialogueBox.TypeDialogue("It's not effective.");
        }


    }
    void PlayerMove()
    {
        state = State.PlayerMove;
        dialogueBox.EnableActionSelect(false);
        dialogueBox.EnableDialogue(false);
        dialogueBox.EnableAbilitySelector(true);
    }

    IEnumerator PlayerAttack()
    {
        state = State.Busy;

        var ability = player.Character.charAbility[currentAbility];
        yield return dialogueBox.TypeDialogue($"{player.Character.CharacterBase.GetName} used {ability.Ability.GetName}");

        Debug.Log(ability.Ability.GetName);

        var damageDetail = enemy.Character.TakeDamage(ability, player.Character);
        yield return enemyHud.UpdateHealth();
        yield return ShowDamage(damageDetail);

        if (damageDetail.Fainted)
        {
            yield return dialogueBox.TypeDialogue($"{enemy.Character.CharacterBase.GetName} is downed");
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    private void Update()
    {
        if (state == State.PlayerTurn)
        {
            ActionSelection();
        } else if (state == State.PlayerMove)
        {
            AbilitySelection();
        }

    }

    void ActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 2)
            {
                ++currentAction;
            }

        } else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                --currentAction;
            }

        }
        dialogueBox.UpdateActionSelect(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (currentAction)
            {
                case 0:
                    //Attack
                    PlayerMove();
                    break;

                case 1:
                    //Item
                    break;

                case 2:
                    //Run
                    break;

            }
        }

    }
    void AbilitySelection()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentAbility > 0)
            {
                --currentAbility;
            }

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentAbility < player.Character.charAbility.Count - 1)
            {
                ++currentAbility;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAbility > 1)
            {
                currentAbility -= 3;
            }

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAbility < player.Character.charAbility.Count - 3)
            {
                currentAbility += 3;
            }
        }

        dialogueBox.UpdateAbilitySelect(currentAbility, player.Character.charAbility[currentAbility]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableAbilitySelector(false);
            dialogueBox.EnableDialogue(true);
            var ability = player.Character.charAbility[currentAbility];

            AnimationManager(ability.Ability.GetName);

            StartCoroutine(PlayerAttack());
        }
    }

    void AnimationManager(string abilityName)
    {
        switch (abilityName)
        {
            case "Fire\nBreath":
                StartCoroutine(PlayAttackAnimation(0));
                break;

            case "Yosafire\nPunch":
                StartCoroutine(PlayAttackAnimation(1));
                break;

            case "Slash":
                StartCoroutine(PlayAttackAnimation(2));
                break;

            case "Bite":
                StartCoroutine(PlayAttackAnimation(3));
                break;

            case "Wink":
                StartCoroutine(PlayAttackAnimation(4));
                break;
        }
    }

    private IEnumerator PlayAttackAnimation(int index)
    {
        image[index].gameObject.SetActive(true);

        yield return new WaitForSeconds(1.3f);

        Debug.Log("Animation completed");
        image[index].gameObject.SetActive(false);
    }

}
