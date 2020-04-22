using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WodaWan : Char
{

    public bool turnMentor;
    public bool isMentoring;

    public GameObject mentorPrefab;
    private GameObject mentorEffect;

    public override void AlternativeAbilities() {
        hasAlternativeMoveSkill = true;
    }

    public override void MoveActive() {
        this.moveActing = true;
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if (gm.Distance(this,tile) == 2 && Mathf.Abs(tile.positionX-this.positionX) == 1 && Mathf.Abs(tile.positionY-this.positionY) == 1 && tile.occupation == null) {
                tile.Walkable();
            }
        }

    }
    
    public override void TurnEffects() {
        if(isMentoring && gm.turn != this.turnOrder) {
            turnMentor = true;
        } else {
            turnMentor = false;
            isMentoring = false;
            Destroy(mentorEffect);
        }
    }

    public override void Conditions() {
        if(turnMentor && gm.Distance(this,gm.activeChar.tile) == 1 && gm.activeChar?.team == this.team) {
            gm.activeChar.moveActivations += 1;
            turnMentor = false;
        }
    }

    public override void Attack() {
        int alliesInTarget = 0;
        foreach (Tile tile in FindObjectsOfType<Tile>()) {
            if(tile.positionX == 1 || tile.positionX == 6 || tile.positionY == 1 || tile.positionY == 6) {
                tile.Hittable();
                if (tile.occupation?.team == this.team) {
                    alliesInTarget += 1;
                }
            }
        }
        foreach (Char character in FindObjectsOfType<Char>()) {
            if (character.tile.hittable == true && alliesInTarget == 0 && character.team != this.team) {
                character.Hittable();
            }
        }
        alliesInTarget = 0;

    }
    
    public override void Skill() {
        this.tile.Targeted();
    }

    public override void AlternativeMove(Char character) {
        
        if(this.isSkilling) {
            isMentoring = true;
            mentorEffect = Instantiate(mentorPrefab,this.transform.position,Quaternion.identity,this.transform);

            // Targeted Skill
            
            this.ResetChar();   // Because he
            this.tile.Active(); // is the target
            
            gm.EndSkill();
        }
    }
}
