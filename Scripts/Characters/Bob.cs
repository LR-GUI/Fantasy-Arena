using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : Char
{

    public override void AlternativeAbilities() {
        hasAlternativeMoveSkill = true;
    }

    public override void TurnEffects() {

    }

    public override void Conditions() {

    }

    public override void Attack() {

        int charsInTarget = 0;
        foreach (Tile tile in FindObjectsOfType<Tile>()) {
            if(gm.Meelee(this.tile,tile)) {
                tile.Hittable();
                if (tile.occupation != null) {
                    charsInTarget += 1;
                }
            }
        }
        foreach (Char character in FindObjectsOfType<Char>()) {
            if (character.tile.hittable == true && charsInTarget >= 3 && character.team != this.team) {
                character.Hittable();
            }
        }
        charsInTarget = 0;


    }
    
    public override void Skill() {
        foreach (Tile tile in FindObjectsOfType<Tile>()) {
            if(gm.Meelee(this.tile,tile) && !(tile.occupation?.team == this.team)) {
                tile.Movable();
            }
        }
    }

    public override void AlternativeMove(Char character) {
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            tile.ResetTile();
        }

        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if (gm.Meelee(this.tile,tile) && tile.occupation == null && (gm.Distance(character,tile) == 1 || gm.GetDiagonal(character.tile,"upperRight") == tile || gm.GetDiagonal(character.tile,"upperLeft") == tile || gm.GetDiagonal(character.tile,"lowerRight") == tile|| gm.GetDiagonal(character.tile,"lowerLeft") == tile)) {
                tile.Movable();
            }
        }
    }
    
}
