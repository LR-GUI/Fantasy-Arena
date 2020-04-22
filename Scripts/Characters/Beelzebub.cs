using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beelzebub : Char
{
    public Char tormentedChar;
    public GameObject tormentedPrefab;
    private GameObject tormentedEffect;
    public bool startTorment = false;
    public int movesHolder;
    public override void AlternativeAbilities() {
        hasAlternativeMoveSkill = true;
    }

    void Update() {
        if(this.usedAction && startTorment) {
            this.Conditions();
        }
    }

    public override void TurnEffects() {
        if(gm.turn == this.turnOrder && this == gm.activeChar) {
            startTorment = true;
        }
    }

    public override void Conditions() {
        if(this.startTorment && this.usedAction) {
            this.startTorment = false;
            movesHolder = this.moveActivations;
            this.moveActivations = 0;
            foreach(Char character in FindObjectsOfType<Char>()) {
                if(character.team != this.team) {
                    character.tile.Targeted();
                }
            }
        }

    }

    public override void Attack() {
        float iDistance = 1f;
        bool search = true;
        int alliesClosest = 0;
        while(search) {
            foreach(Char character in FindObjectsOfType<Char>()) {
                if(gm.Distance(this,character) == iDistance) {
                    if(character.team == this.team) {
                        alliesClosest += 1;
                    }
                    character.tile.Hittable();
                    search = false;
                }
            }
            iDistance += 1;
        }
        foreach(Char character in FindObjectsOfType<Char>()) {
            if(character.tile.hittable && character.team != this.team && tormentedChar == character && alliesClosest == 0) {
                character.Hittable();
            }
        }

    }
    
    public override void Skill() {
        float iDistance = 1f;
        bool search = true;
        while(search) {
            foreach(Char character in FindObjectsOfType<Char>()) {
                if(gm.Distance(this,character) == iDistance) {
                    if(character.team == this.team) {
                        character.tile.Targeted();
                        search = false;
                    }
                    
                }
            }
            iDistance += 1;
        }
    }

    public override void AlternativeMove(Char character) {
        
        if(this.isSkilling) {
            gm.UpdateBoard();
            this.Movable();
            gm.movingChar = this;

            if(gm.GetAdjacent(character.tile,"up")!=null && gm.GetAdjacent(character.tile,"up").occupation==null) {
                gm.GetAdjacent(character.tile,"up").Movable();
            }
            if(gm.GetAdjacent(character.tile,"down")!=null && gm.GetAdjacent(character.tile,"down").occupation==null) {
                gm.GetAdjacent(character.tile,"down").Movable();
            }
            if(gm.GetAdjacent(character.tile,"left")!=null && gm.GetAdjacent(character.tile,"left").occupation==null) {
                gm.GetAdjacent(character.tile,"left").Movable();
            }
            if(gm.GetAdjacent(character.tile,"right")!=null && gm.GetAdjacent(character.tile,"right").occupation==null) {
                gm.GetAdjacent(character.tile,"right").Movable();
            }
        }

        if(this.usedAction) {
            Destroy(tormentedEffect);
            tormentedChar = character;
            tormentedEffect = Instantiate(tormentedPrefab,character.transform.position,Quaternion.identity,character.transform);
            this.moveActivations = movesHolder;
            movesHolder = -1;
            gm.UpdateBoard();
            gm.SetActiveChar();
            if(this.moveActivations>0) {
                this.MoveActive();
            }
        }

    }
    
}
