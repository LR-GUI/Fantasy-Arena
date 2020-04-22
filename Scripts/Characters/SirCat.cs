using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SirCat : Char
{

    public GameObject watchPrefab;
    private GameObject watchEffect;

    public override void AlternativeAbilities() {
        hasAlternativeMoveSkill = true;
    }


    void Update() {
        if(this.moveActivations==2 && !usedAction) {
            this.moveActivations=1;
        }
    }

    public override void MoveActive() {
        this.moveActing = true;
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if(gm.Distance(this,tile)==2 && this.positionX == tile.positionX && tile.occupation == null) {
                tile.Walkable();
            }
            if(gm.Distance(this,tile)==2 && this.positionY == tile.positionY && tile.occupation == null) {
                tile.Walkable();
            }
        }
        
        

    }
    
    public override void TurnEffects() {
    }

    public override void Conditions() {
    }

    public override void Attack() {
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if(gm.Distance(this,tile)==1) {
                tile.Hittable();
            }
        }
        foreach(Char character in FindObjectsOfType<Char>()) {
            if (character.tile.hittable == true && character.team != this.team ) {
                character.Hittable();
            }
        }
    }
    
    public override void Skill() {
        this.tile.Targeted();
    }
    public override void AlternativeMove(Char character) {

        if(this.isSkilling) {
            watchEffect = Instantiate(watchPrefab,this.transform.position,Quaternion.identity,this.transform);
            Destroy(watchEffect,4f);
            this.moveActivations += 1;

            // Targeted Skill
            
            this.ResetChar();   // Because he
            this.tile.Active(); // is the target
            
            gm.EndSkill();
        }
        
    }


}
