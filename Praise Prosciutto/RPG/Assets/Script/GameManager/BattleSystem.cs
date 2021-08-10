using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State { Start, ActionSelect, AbilitySelect, PerformAction, Busy, PartyScreen, BattleOver}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleManager player;
    [SerializeField] BattleManager enemy;
    
    [SerializeField] BattleDialogue dialogueBox;
    [SerializeField] List<Image> image;
    [SerializeField] PartyScreen partyScreen;

    CharacterParty characterParty;
    CharacterManager _enemy;
    public Status _Status { get; set; }
    public event Action<bool> OnBattleOver;
    public bool canPlayAnimation = true;

    int currentAction;
    int currentAbility;
    int currentMember;
    State state;

    public void StartBattle(CharacterParty characters, CharacterManager enemy)
    {
        characterParty = characters;
        _enemy = enemy;
        StartCoroutine(SetupBattle());

        //image[0].gameObject.SetActive(true);
    }

    public IEnumerator SetupBattle()
    {
        player.Setup(characterParty.HealthyCharacter());
        enemy.Setup(_enemy);

        dialogueBox.SetAbility(player.Character.charAbility);

        partyScreen.Initialize();

        yield return dialogueBox.TypeDialogue($"{enemy.Character.CharacterBase.GetName} challenges you!");

        StartFirst();
    }

    void StartFirst()
    {
        if (player.Character.Speed >= enemy.Character.Speed)
        {
            ActionSelector();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    void ActionSelector()
    {
        state = State.ActionSelect;
        int x = UnityEngine.Random.Range(1, 10);
        switch (x)
        {
            case 1:
                StartCoroutine(dialogueBox.TypeDialogue("Yummy Prosciutto."));
                break;

            case 2:
                StartCoroutine(dialogueBox.TypeDialogue($"You felt {enemy.Character.CharacterBase.GetName} crawling at your skin."));
                break;

            case 3:
                StartCoroutine(dialogueBox.TypeDialogue("Praise be to Lord Prosciutto!"));
                break;

            case 4:
                StartCoroutine(dialogueBox.TypeDialogue("Just Prosciutto."));
                break;

            case 5: case 6: case 7: case 8: case 9: case 10:
                StartCoroutine(dialogueBox.TypeDialogue("It is your turn."));
                break;
        }
        dialogueBox.EnableActionSelect(true);
    }

    IEnumerator EnemyTurn()
    {
        state = State.PerformAction;

        var ability = enemy.Character.GetAbilityForEnemy();

        yield return InitiateMove(enemy, player, ability);
        if (state == State.PerformAction)
        {
            ActionSelector();
        }
    }

    IEnumerator PlayerAttack()
    {
        state = State.PerformAction;

        var ability = player.Character.charAbility[currentAbility];
     
        yield return InitiateMove(player, enemy, ability);

        if (state == State.PerformAction)
        {
            StartCoroutine(EnemyTurn());
        }

    }

    void BattleOver(bool victory)
    {
        state = State.BattleOver;
        characterParty.Characters.ForEach(p => p.OnBattleOver()); //reset stats of every characters
        OnBattleOver(victory);
    }

    IEnumerator InitiateMove(BattleManager source, BattleManager target, AbilityManager ability)
    {
        bool canMove = source.Character.OnBeforeMove();

        if (!canMove)
        {
            yield return ShowStatusChange(source.Character);
            yield return source._Hud.UpdateHealth();
            yield break;
        }
        else
        {
            if (source.IsPlayer)
                AnimationManager(ability.Ability.GetName);
        }
        yield return ShowStatusChange(source.Character);

        ability.Amount--;
        yield return dialogueBox.TypeDialogue($"{source.Character.CharacterBase.GetName} used {ability.Ability.GetName}");

        

        if(ability.Ability.Category == AbilityCategory.Status)
        {
            yield return RunAbilityEffect(ability, source.Character, target.Character);
        }
        else
        {
            target.HitAnimation();
            var damageDetail = target.Character.TakeDamage(ability, source.Character);
            yield return target._Hud.UpdateHealth();
            yield return ShowDamage(damageDetail);
        }
        
        //dead
        if (target.Character.HP <= 0)
        {
            yield return dialogueBox.TypeDialogue($"{target.Character.CharacterBase.GetName} is toasted");
            target.DeathAnimation();
            yield return new WaitForSeconds(2f);

            CheckBattleOver(target);
        }
        //display damage dealt by status
        source.Character.OnAfterTurn();
        yield return ShowStatusChange(source.Character);
        yield return source._Hud.UpdateHealth();
        //if character dies after status
        if (source.Character.HP <= 0)
        {
            yield return dialogueBox.TypeDialogue($"{source.Character.CharacterBase.GetName} is toasted");
            source.DeathAnimation();
            yield return new WaitForSeconds(2f);

            CheckBattleOver(source);
        }
    }
    IEnumerator RunAbilityEffect(AbilityManager ability, CharacterManager source, CharacterManager target)
    {
        var effects = ability.Ability.Effects;

        //Stat changing
        if (effects.Boosts != null)
        {
            if (ability.Ability.Target == AbilityTarget.Self)
            {
                source.ApplyBoost(effects.Boosts);
            }
            else
            {
                target.ApplyBoost(effects.Boosts);
            }
        }

        //Status effect
        if (effects.StatusEffect != StatusID.none) //has no status 
        {
            target.SetStatus(effects.StatusEffect);
        }
        yield return ShowStatusChange(source);
        yield return ShowStatusChange(target);
    }
    IEnumerator ShowStatusChange(CharacterManager character)
    {
        while (character.StatusChange.Count > 0)
        {
            var message = character.StatusChange.Dequeue(); //pull message out
            yield return dialogueBox.TypeDialogue(message);
        }
    }
    void CheckBattleOver(BattleManager toastedChar)
    {
        if (!toastedChar.IsPlayer)
            _Status = null;

        if (toastedChar.IsPlayer)
        {
            var nextChar = characterParty.HealthyCharacter();
            //send out next character if there is one
            if (nextChar != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
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
    void MoveSelection()
    {
        state = State.AbilitySelect;
        dialogueBox.EnableActionSelect(false);
        dialogueBox.EnableDialogue(false);
        dialogueBox.EnableAbilitySelector(true);
    }

    public void HandleUpdate()
    {
        if (state == State.ActionSelect)
        {
            ActionSelection();
        } else if (state == State.AbilitySelect)
        {
            AbilitySelection();
        } else if (state == State.PartyScreen)
        {
            HandlePartySelect();
        }

    }

    void OpenPartyScreen()
    {
        state = State.PartyScreen;
        partyScreen.SetPartyData(characterParty.Characters);
        partyScreen.gameObject.SetActive(true);
    }

    void HandlePartySelect()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMember < 3)
            {
                ++currentMember;
            }

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMember > 0)
            {
                --currentMember;
            }
        }

        partyScreen.UpdateMemberSelect(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = characterParty.Characters[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessage("The character is toasted.");
                return;
            }   
            if (selectedMember == player.Character)
            {
                partyScreen.SetMessage("That's the same character, stoopid");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = State.Busy;
            StartCoroutine(SwitchCharacter(selectedMember));
        } 
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelector();
        }
    }

    IEnumerator SwitchCharacter(CharacterManager newCharacter)
    {
        bool currentToastedChar = true;
        if (player.Character.HP > 0)
        {
            currentToastedChar = false;
            yield return dialogueBox.TypeDialogue($"{player.Character.CharacterBase.GetName} retreats!");
            yield return new WaitForSeconds(1f);
        }

        player.Setup(newCharacter);
        dialogueBox.SetAbility(newCharacter.charAbility);
        yield return dialogueBox.TypeDialogue($"{newCharacter.CharacterBase.GetName} is out!");

        if (currentToastedChar)
        {
            StartFirst();
        } else
        {
            StartCoroutine(EnemyTurn());
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
                    MoveSelection();
                    break;

                case 1:
                    //Party
                    OpenPartyScreen();
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

            //AnimationManager(ability.Ability.GetName);

            StartCoroutine(PlayerAttack());
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableAbilitySelector(false);
            dialogueBox.EnableActionSelect(true);
            dialogueBox.EnableDialogue(true);
            ActionSelector();
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
