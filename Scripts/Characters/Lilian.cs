using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lilian : Char
{
    public int movesHolder;

    public Tile blockA;
    public Tile blockB;

    public override void TurnEffects() {
        if(gm.turn == this.turnOrder) {
            movesHolder = -1;
        }
    }

    public override void Conditions() {
        if(this.isAttacking) {
            this.moveActivations -= 1;
        }

        if(movesHolder > 0 || this.movable) {
            this.moveActivations = movesHolder + 1;
            if(this.movable) {this.moveActivations -= 1;}
            movesHolder = -1;
            gm.UpdateBoard();
            gm.SetActiveChar();
            if(this.moveActivations > 0) {
                this.MoveActive();
            }
        }

        if(this.isSkilling) {
            movesHolder = this.moveActivations;
            this.moveActivations = 0;
            this.Movable();
            gm.movingChar = this;
            this.ChooseMove();

        }
    }

    public override void Attack() {

        foreach (Tile tile in FindObjectsOfType<Tile>()) {
            if(gm.Distance(this.tile,tile) == 6 && Mathf.Abs(this.positionX-tile.positionX)==Mathf.Abs(this.positionY-tile.positionY) && this.moveActivations > 0) {

                if ( (this.positionX<=3 && this.positionY>=3)||(this.positionX>=3 && this.positionY<=3) ) {
                    blockA = gm.GetTile(Mathf.Min(this.tile.positionX,tile.positionX)+1,Mathf.Min(this.tile.positionY,tile.positionY)+2);
                    blockB = gm.GetTile(Mathf.Min(this.tile.positionX,tile.positionX)+2,Mathf.Min(this.tile.positionY,tile.positionY)+1);
                }
                if ( (this.positionX<=3 && this.positionY<=3)||(this.positionX>=3 && this.positionY>=3) ) {
                    blockA = gm.GetTile(Mathf.Min(this.tile.positionX,tile.positionX)+1,Mathf.Min(this.tile.positionY,tile.positionY)+1);
                    blockB = gm.GetTile(Mathf.Min(this.tile.positionX,tile.positionX)+2,Mathf.Min(this.tile.positionY,tile.positionY)+2);
                }
                
                if (blockA.occupation == null && blockB.occupation == null) {
                    tile.Hittable();
                }
            }
        }
        foreach (Char character in FindObjectsOfType<Char>()) {
            if (character.tile.hittable == true && character.team != this.team) {
                character.Hittable();
            }
        }


    }
    
    public override void Skill() {
        float iDistance = 1f;
        bool search = true;
        while(search) {
            foreach(Char character in FindObjectsOfType<Char>()) {
                if(gm.Distance(this,character) == iDistance /*&& character.team == this.team*/) {
                    character.tile.Movable();
                    search = false;
                }
            }
            iDistance += 1;
        }
        /*float iDistance2 = 1f;
        bool search2 = true;
        while(search2) {
            foreach(Char character in FindObjectsOfType<Char>()) {
                if(gm.Distance(this,character) == iDistance2 && character.team != this.team) {
                    character.tile.Movable();
                    search2 = false;
                }
            }
            iDistance2 += 1;
        }*/
    }
    
}
