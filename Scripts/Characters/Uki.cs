using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uki : Char
{

    public bool isPolar;

    public List<Tile> isAcross = new List<Tile>();

    public override void AlternativeAbilities() {
        hasAlternativeMoveSkill = true;
    }

    public override void AlternativeMove(Char character) {
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            tile.ResetTile();
        }
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if(gm.Distance(character,tile)==1 && tile.occupation == null) {
                tile.Movable();
            }

            if(character.positionX == 1) {
                gm.GetTile(6,character.positionY).Movable();
            }
            if(character.positionX == 6) {
                gm.GetTile(1,character.positionY).Movable();
            }
            if(character.positionY == 1) {
                gm.GetTile(character.positionX,6).Movable();
            }
            if(character.positionY == 6) {
                gm.GetTile(character.positionX,1).Movable();
            }

        }
    }

    public override void MoveActive() {
        this.moveActing = true;
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if (gm.Distance(this,tile) == 1 && tile.occupation == null) {
                tile.Walkable();
            }
        }

        if(this.positionX == 1 && gm.GetTile(6,this.positionY).occupation==null) {
            gm.GetTile(6,this.positionY).Walkable();
            if(!isAcross.Contains(gm.GetTile(6,this.positionY))) {
                isAcross.Add(gm.GetTile(6,this.positionY));
            }
        }
        if(this.positionX == 6 && gm.GetTile(1,this.positionY).occupation==null) {
            gm.GetTile(1,this.positionY).Walkable();
            if(!isAcross.Contains(gm.GetTile(1,this.positionY))) {
                isAcross.Add(gm.GetTile(1,this.positionY));
            }
        }
        if(this.positionY == 1 && gm.GetTile(this.positionX,6)) {
            gm.GetTile(this.positionX,6).Walkable();
            if(!isAcross.Contains(gm.GetTile(this.positionX,6))) {
                isAcross.Add(gm.GetTile(this.positionX,6));
            }
        }
        if(this.positionY == 6 && gm.GetTile(this.positionX,1).occupation==null) {
            gm.GetTile(this.positionX,1).Walkable();
            if(!isAcross.Contains(gm.GetTile(this.positionX,1))) {
                isAcross.Add(gm.GetTile(this.positionX,1));
            }
        }
        

    }
    
    public override void TurnEffects() {
        isPolar = false;
    }

    public override void Conditions() {
        if(isAcross.Count>0) {
            if(isAcross.Contains(this.tile)) {
                isPolar = true;
            }
            isAcross.Clear();
        }
    }

    public override void Attack() {
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if(gm.Distance(this,tile)<=3 && this.positionX == tile.positionX) {
                tile.Hittable();
            }
            if(gm.Distance(this,tile)<=3 && this.positionY == tile.positionY) {
                tile.Hittable();
            }
        }
        foreach(Char character in FindObjectsOfType<Char>()) {
            if (character.tile.hittable == true && isPolar && character.team != this.team && (character.positionX==1||character.positionX==6||character.positionY==1||character.positionY==6) ) {
                character.Hittable();
            }
        }
    }
    
    public override void Skill() {
        foreach(Char character in FindObjectsOfType<Char>()) {
            if(gm.Distance(this,character)<=3 && this.positionX == character.positionX && character!=this) {
                character.tile.Movable();
            }
            if(gm.Distance(this,character)<=3 && this.positionY == character.positionY && character!=this) {
                character.tile.Movable();
            }

            if((this.positionX==1||this.positionX==6||this.positionY==1||this.positionY==6)&&(character.positionX==1||character.positionX==6||character.positionY==1||character.positionY==6)&&character!=this) {
                character.tile.Movable();
            }
        }
    }


}
