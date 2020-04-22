using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fae : Char
{
    public Char enchantedChar;
    public GameObject enchantPrefab;
    private GameObject enchantEffect;
    public override void AlternativeAbilities() {
        hasAlternativeMoveSkill = true;
    }

    public override void TurnEffects() {
        if(enchantedChar!=null) {
            if((gm.turn == enchantedChar.turnOrder + 1 && enchantedChar.turnOrder < 6 ) || (gm.turn == 1 && enchantedChar.turnOrder == 6)) {
                if(!(gm.Meelee(this.tile,enchantedChar.tile))) {
                    gm.Hit(enchantedChar.team);
                }
                Destroy(enchantEffect);
                enchantedChar = null;
            }
        }
        
    }

    public override void Conditions() {

    }

    public override void Attack() {
        float iDistance = 1f;
        bool search = true;
        while(search) {
            foreach(Char character in FindObjectsOfType<Char>()) {
                if(gm.Distance(this,character) == iDistance) {
                    if(character.team != this.team) {
                        character.tile.Targeted();
                    }
                    search = false;
                }
            }
            iDistance += 1;
        }

    }
    
    public override void Skill() {
        float iDistance = 1f;
        bool search = true;
        while(search) {
            foreach(Char character in FindObjectsOfType<Char>()) {
                if(gm.Distance(this,character) == iDistance) {
                    character.tile.Targeted();
                    search = false;
                }
            }
            iDistance += 1;
        }
    }

    public override void AlternativeMove(Char character) {
        
        if(this.isAttacking) {
            enchantEffect = Instantiate(enchantPrefab,character.transform.position,Quaternion.identity,character.transform);
            enchantedChar = character;

            // Targeted Attack
            gm.EndAttack();
        }
        
        if(this.isSkilling) {
            Tile charPosition = character.tile;
            gm.DestinateMove(character,this.tile);
            gm.DestinateMove(this,charPosition);

            // Targeted Skill
            gm.EndSkill();
        }

    }
    
}
