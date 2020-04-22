using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trisha : Char
{

    public int movesHolder;
    public Dictionary<Char,Tile> charMemory = new Dictionary<Char, Tile>();
    public List<Tile> tileMemory = new List<Tile>();

    public override void TurnEffects() {
        if(gm.turn == this.turnOrder) {
            movesHolder = -1;
        }
    }

    public override void Conditions() {

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

            float directionX = 0;
            float directionY = 0;
            foreach(Tile tile in FindObjectsOfType<Tile>()) {
                if(tile.occupation != gm.activeChar) {
                    tile.ResetTile();
                }
                foreach(KeyValuePair<Char,Tile> items in charMemory) {
                    if(items.Key.tile != items.Value) {
                        directionX = items.Key.tile.positionX - items.Value.positionX;
                        directionY = items.Key.tile.positionY - items.Value.positionY;
                        if (gm.GetTile(this.positionX+directionX,this.positionY+directionY).occupation == null) {
                            gm.GetTile(this.positionX+directionX,this.positionY+directionY).Movable();
                        }
                    }
                }
            }
            charMemory.Clear();
            tileMemory.Clear();

        }
    }

    public override void Attack() {
        int alliesInTarget = 0;
        int oppsInTarget = 0;
        foreach (Tile tile in FindObjectsOfType<Tile>()) {
            if(gm.Meelee(this.tile,tile)) {
                tile.Hittable();
                if (tile.occupation?.team == this.team) {
                    alliesInTarget += 1;
                }
                if(tile.occupation) {
                    if (tile.occupation.team != this.team) {
                        oppsInTarget += 1;
                    }
                }
            }
        }
        foreach (Char character in FindObjectsOfType<Char>()) {
            if (character.tile.hittable == true && alliesInTarget >= 1 && oppsInTarget == 1 && character.team != this.team) {
                character.Hittable();
            }
        }
        alliesInTarget = 0;
        oppsInTarget = 0;

    }
    
    public override void Skill() {
        foreach (Tile tile in FindObjectsOfType<Tile>()) {
            if(gm.Meelee(this.tile,tile) && (tile.occupation?.team == this.team || tile.occupation == null)) {
                tile.Movable();
                if(tile.occupation != null && !(charMemory.ContainsKey(tile.occupation))) {
                    charMemory.Add(tile.occupation,tile);
                    foreach(Tile destination in FindObjectsOfType<Tile>()) {
                        if(gm.Distance(tile.occupation,destination) == 1 && destination.occupation == null) {
                            tileMemory.Add(destination);
                        }
                    }
                }
                
            }
        }
    }
    
}
