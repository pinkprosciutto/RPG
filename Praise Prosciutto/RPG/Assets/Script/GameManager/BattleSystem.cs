using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State { Start, ActionSelect, AbilitySelect, PerformAction, RunningTurn, Busy, PartyScreen, BattleOver}
public enum CombatAction { Move, SwitchCharacter, Escape }

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
    State? previousState; //nullable

    public void StartBattle(CharacterParty characters, CharacterManager enemy)
    {
        characterParty = characters;
        _enemy = enemy;
        StartCoroutine(SetupBattle());

        //image[0].gameObject.SetActive(true);
    }

    public void HandleUpdate()
    {
        if (state == State.ActionSelect)
        {
            ActionSelection();
        }
        else if (state == State.AbilitySelect)
        {
            AbilitySelection();
        }
        else if (state == State.PartyScreen)
        {
            HandlePartySelect();
        }

    }

    public IEnumerator SetupBattle()
    {
        player.Setup(characterParty.HealthyCharacter());
        enemy.Setup(_enemy);

        dialogueBox.SetAbility(player.Character.charAbility);

        partyScreen.Initialize();

        yield return dialogueBox.TypeDialogue($"{enemy.Character.CharacterBase.GetName} challenges you!");

        ActionSelector();
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
                StartCoroutine(dialogueBox.TypeDialogue($"Mogege."));
                break;

            case 3:
                StartCoroutine(dialogueBox.TypeDialogue("Praise be to Prosciutto!"));
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

    IEnumerator RunTurn(CombatAction playerAction)
    {
        state = State.RunningTurn;

        if (playerAction == CombatAction.Move)
        {
            player.Character.CurrentAbility = player.Character.charAbility[currentAbility];
            enemy.Character.CurrentAbility = enemy.Character.GetAbilityForEnemy();

            //who moves first
            bool playerGoesFirst = player.Character.Speed >= enemy.Character.Speed;

            var firstChar = (playerGoesFirst) ? player : enemy;
            var secondChar = (playerGoesFirst) ? enemy : player;

            var secondUnit = secondChar.Character;

            //first turn
            yield return InitiateMove(firstChar, secondChar, firstChar.Character.CurrentAbility);
            yield return RunAfterTurn(firstChar);
            if (state == State.BattleOver) yield break;

            if (secondUnit.HP > 0) //if character is not toasted, execute below
            {
                //second turn
                yield return InitiateMove(secondChar, firstChar, secondChar.Character.CurrentAbility);
                yield return RunAfterTurn(secondChar);
                if (state == State.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == CombatAction.SwitchCharacter)
            {
                var selectedMember = characterParty.Characters[currentMember];
                state = State.Busy;
                yield return SwitchCharacter(selectedMember);
            }
            //Enemy's turn
            var enemyAbility = enemy.Character.GetAbilityForEnemy();
            yield return InitiateMove(enemy, player, enemyAbility);
            yield return RunAfterTurn(enemy);
            if (state == State.BattleOver) yield break;
        }

        if (state != State.BattleOver)
            ActionSelection();
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
        yield return dialogueBox.TypeDialogue($"{source.Character.CharacterBase.GetName} used {ability.Ability.GetName}.");

        if (CheckIfAttackHits(ability, source.Character, target.Character))
        {
            if (ability.Ability.Category == AbilityCategory.Status)
            {
                yield return RunAbilityEffect(ability.Ability.Effects, source.Character, target.Character, ability.Ability.Target);
            }
            else
            {
                target.HitAnimation();
                var damageDetail = target.Character.TakeDamage(ability, source.Character);
                yield return target._Hud.UpdateHealth();
                yield return ShowDamage(damageDetail);
            }

            if(ability.Ability.SecondaryEffects != null && ability.Ability.SecondaryEffects.Count > 0 && target.Character.HP > 0)
            {
                foreach (var secondary in ability.Ability.SecondaryEffects)
                {
                    var random = UnityEngine.Random.Range(1, 101);
                    if (random <= secondary.Chance)
                    {
                        yield return RunAbilityEffect(secondary, source.Character, target.Character, secondary.Target);
                    }
                }
            }

            //dead
            if (target.Character.HP <= 0)
            {
                yield return dialogueBox.TypeDialogue($"{target.Character.CharacterBase.GetName} is toasted!");
                target.DeathAnimation();
                yield return new WaitForSeconds(2f);

                CheckBattleOver(target);
            }
        }
        else
        {
            yield return dialogueBox.TypeDialogue($"{source.Character.CharacterBase.GetName}'s attack missed!");
        }

    }
    bool CheckIfAttackHits(AbilityManager ability, CharacterManager source, CharacterManager target)
    {
        float accuracy = ability.Ability.Accuracy;
        return UnityEngine.Random.Range(1, 101) <= accuracy; //return true if the criteria is met
    }
    IEnumerator RunAbilityEffect(AbilityEffect effects, CharacterManager source, CharacterManager target, AbilityTarget abilityTarget)
    {
        //Stat changing
        if (effects.Boosts != null)
        {
            if (abilityTarget == AbilityTarget.Self)
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

    IEnumerator RunAfterTurn(BattleManager source)
    {
        if (state == State.BattleOver) yield break; //won't continue taking status damage after battle is over
        yield return new WaitUntil(() => state == State.RunningTurn); //pause until state goes back to runningturn in part selection screen
        //display damage dealt by status
        source.Character.OnAfterTurn();
        yield return ShowStatusChange(source.Character);
        yield return source._Hud.UpdateHealth();
        //if character dies after status
        if (source.Character.HP <= 0)
        {
            yield return dialogueBox.TypeDialogue($"{source.Character.CharacterBase.GetName} is toasted!");
            source.DeathAnimation();
            yield return new WaitForSeconds(2f);

            CheckBattleOver(source);
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
                OpenPartyScreen();
            else
                BattleOver(false);
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
                ++currentMember;

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMember > 0)
                --currentMember;
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
                partyScreen.SetMessage("That's the same character, stoopid.");
                return;
            }
            dialogueBox.EnableActionSelect(false);
            partyScreen.gameObject.SetActive(false);

            if (previousState == State.ActionSelect)
            {
                previousState = null;
                StartCoroutine(RunTurn(CombatAction.SwitchCharacter));
            }
            else
            {
                state = State.Busy;
                StartCoroutine(SwitchCharacter(selectedMember));
            }
        } 
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelector();
        }
    }

    IEnumerator SwitchCharacter(CharacterManager newCharacter)
    {
        if (player.Character.HP > 0)
        {
            yield return dialogueBox.TypeDialogue($"{player.Character.CharacterBase.GetName} retreats!");
            yield return new WaitForSeconds(1f);
        }

        player.Setup(newCharacter);
        dialogueBox.SetAbility(newCharacter.charAbility);
        yield return dialogueBox.TypeDialogue($"{newCharacter.CharacterBase.GetName} is out!");

        state = State.RunningTurn;
    }

    void ActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 2)
                ++currentAction;

        } else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
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
                    previousState = state;
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
                --currentAbility;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentAbility < player.Character.charAbility.Count - 1)
                ++currentAbility;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAbility > 1)
                currentAbility -= 3;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAbility < player.Character.charAbility.Count - 3)
                currentAbility += 3;
        }

        dialogueBox.UpdateAbilitySelect(currentAbility, player.Character.charAbility[currentAbility]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableAbilitySelector(false);
            dialogueBox.EnableDialogue(true);
            StartCoroutine(RunTurn(CombatAction.Move));

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
