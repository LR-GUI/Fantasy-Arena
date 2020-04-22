using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : Char
{
    // Move deprecated by changes in the card
    /*public override void MoveActive() {
        this.moveActing = true;
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if (gm.Distance(this,tile) == 1 && tile.occupation == null) {
                tile.Walkable();
            }
            if (gm.Distance(this,tile) == 1 && tile.occupation != null) {
                foreach(Tile pass in FindObjectsOfType<Tile>()) {
                    if (gm.Distance(tile,pass) == 1 && pass.occupation == null) {
                        pass.Walkable();
                    }
                    if(gm.Distance(tile,pass) == 1 && pass.occupation != null && pass.occupation != this) {
                        foreach(Tile pass2 in FindObjectsOfType<Tile>()) {
                            if (gm.Distance(pass,pass2) == 1 && pass2.occupation == null) {
                                pass2.Walkable();
                            }
                            if(gm.Distance(pass,pass2) == 1 && pass2.occupation != null && pass2.occupation != this && pass2.occupation != pass.occupation) {
                                foreach(Tile pass3 in FindObjectsOfType<Tile>()) {
                                    if (gm.Distance(pass2,pass3) == 1 && pass3.occupation == null) {
                                        pass3.Walkable();
                                    }
                                    if (gm.Distance(pass2,pass3) == 1 && pass3.occupation != null && pass3.occupation != this && pass3.occupation != pass2.occupation && pass3.occupation != pass.occupation) {
                                        foreach(Tile pass4 in FindObjectsOfType<Tile>()) {
                                            if (gm.Distance(pass3,pass4) == 1 && pass4.occupation == null) {
                                                pass4.Walkable();
                                            }
                                            if (gm.Distance(pass4,pass3) == 1 && pass4.occupation != null && pass4.occupation != this && pass4.occupation != pass2.occupation && pass4.occupation != pass.occupation && pass4.occupation != pass3.occupation) {
                                                foreach(Tile pass5 in FindObjectsOfType<Tile>()) {
                                                    if (gm.Distance(pass4,pass5) == 1 && pass5.occupation == null) {
                                                        pass5.Walkable();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

    }*/

    public Dictionary<Char,Tile> tunnelled = new Dictionary<Char, Tile>();
    public List<Char> isTunnelled = new List<Char>();

    public override void AlternativeAbilities() {
        hasAlternativeMoveSkill = true;
    }

    public override void MoveActive() {
        this.moveActing = true;
        tunnelled.Clear();
        foreach(Tile ctile in FindObjectsOfType<Tile>()) {
            if (gm.Distance(this,ctile) == 1 && ctile.occupation == null) {
                ctile.Walkable();
            }
            if(gm.Distance(this,ctile) == 1 && ctile.occupation != null) {
                if(ctile == gm.GetAdjacent(this.tile,"up")) {
                    int i = 1;
                    while(ctile.positionY + i <= 6) {
                        if(gm.GetTile(ctile.positionX,ctile.positionY+i).occupation == null) {
                            gm.GetTile(ctile.positionX,ctile.positionY+i).Walkable();
                            for(int j = 0; j < i; j++ ) {
                                tunnelled.Add(gm.GetTile(ctile.positionX,ctile.positionY+j).occupation,gm.GetTile(ctile.positionX,ctile.positionY+i));
                            }
                            break;
                        }
                        i++;
                    }
                }
                if(ctile == gm.GetAdjacent(this.tile,"down")) {
                    int i = 1;
                    while(ctile.positionY - i >= 1) {
                        if(gm.GetTile(ctile.positionX,ctile.positionY-i).occupation == null) {
                            gm.GetTile(ctile.positionX,ctile.positionY-i).Walkable();
                            for(int j = 0; j < i; j++ ) {
                                tunnelled.Add(gm.GetTile(ctile.positionX,ctile.positionY-j).occupation,gm.GetTile(ctile.positionX,ctile.positionY-i));
                            }
                            break;
                        }
                        i++;
                    }
                }
                if(ctile == gm.GetAdjacent(this.tile,"right")) {
                    int i = 1;
                    while(ctile.positionX + i <= 6) {
                        if(gm.GetTile(ctile.positionX+i,ctile.positionY).occupation == null) {
                            gm.GetTile(ctile.positionX+i,ctile.positionY).Walkable();
                            for(int j = 0; j < i; j++ ) {
                                tunnelled.Add(gm.GetTile(ctile.positionX+j,ctile.positionY).occupation,gm.GetTile(ctile.positionX+i,ctile.positionY));
                            }
                            break;
                        }
                        i++;
                    }
                }
                if(ctile == gm.GetAdjacent(this.tile,"left")) {
                    int i = 1;
                    while(ctile.positionX - i >= 1) {
                        if(gm.GetTile(ctile.positionX-i,ctile.positionY).occupation == null) {
                            gm.GetTile(ctile.positionX-i,ctile.positionY).Walkable();
                            for(int j = 0; j < i; j++ ) {
                                tunnelled.Add(gm.GetTile(ctile.positionX-j,ctile.positionY).occupation,gm.GetTile(ctile.positionX-i,ctile.positionY));
                            }
                            break;
                        }
                        i++;
                    }
                }
            }
        }

    }

    public override void TurnEffects() {
        isTunnelled.Clear();
    }

    public override void Conditions() {

        if(tunnelled.Count != 0 && !this.isAttacking) {
            foreach(KeyValuePair<Char,Tile> items in tunnelled) {
                if(items.Value == this.tile && !(isTunnelled.Contains(items.Key))) {
                    isTunnelled.Add(items.Key);
                }
            }
            tunnelled.Clear();
            
            
        }
    }

    public override void Attack() {
        foreach(Char character in isTunnelled) {
            if(character.team != this.team && isTunnelled.Count >= 2) {
                character.Hittable();
                character.tile.Hittable();
            }
        }
    }

    public override void Skill() {
        foreach(Char character in FindObjectsOfType<Char>()) {
            if(character != this && isTunnelled.Contains(character)) {
                character.tile.Movable();
            }
        }
    }

    public override void AlternativeMove(Char character) {

        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            tile.ResetTile();
        }

        foreach(Tile tile in FindObjectsOfType<Tile>()) {

            if (gm.Distance(character,tile) == 1 && tile.occupation == null) {
                tile.Movable();
            }
            if(gm.Distance(character,tile) == 1 && tile.occupation != null) {
                if(tile == gm.GetAdjacent(character.tile,"up")) {
                    int i = 1;
                    while(tile.positionY + i <= 6) {
                        if(gm.GetTile(tile.positionX,tile.positionY+i).occupation == null) {
                            gm.GetTile(tile.positionX,tile.positionY+i).Movable();
                            
                            break;
                        }
                        i++;
                    }
                }
                if(tile == gm.GetAdjacent(character.tile,"down")) {
                    int i = 1;
                    while(tile.positionY - i >= 1) {
                        if(gm.GetTile(tile.positionX,tile.positionY-i).occupation == null) {
                            gm.GetTile(tile.positionX,tile.positionY-i).Movable();
                            
                            break;
                        }
                        i++;
                    }
                }
                if(tile == gm.GetAdjacent(character.tile,"right")) {
                    int i = 1;
                    while(tile.positionX + i <= 6) {
                        if(gm.GetTile(tile.positionX+i,tile.positionY).occupation == null) {
                            gm.GetTile(tile.positionX+i,tile.positionY).Movable();
                            
                            break;
                        }
                        i++;
                    }
                }
                if(tile == gm.GetAdjacent(character.tile,"left")) {
                    int i = 1;
                    while(tile.positionX - i >= 1) {
                        if(gm.GetTile(tile.positionX-i,tile.positionY).occupation == null) {
                            gm.GetTile(tile.positionX-i,tile.positionY).Movable();
                            
                            break;
                        }
                        i++;
                    }
                }
            }
                
                // Deprecated by changes in the card 
                /*
            if(tile.occupation == null) {
            if (gm.Distance(character,tile) <= 2 && gm.Diagonal(character,tile) == null) {
                    tile.Movable();
                }

                if(gm.Diagonal(character,tile) == "upperRight" && (gm.GetAdjacent(character.tile,"up").occupation == null || gm.GetAdjacent(character.tile,"right").occupation == null)) {
                    tile.Movable();
                }
                if(gm.Diagonal(character,tile) == "upperLeft" && (gm.GetAdjacent(character.tile,"up").occupation == null || gm.GetAdjacent(character.tile,"left").occupation == null)) {
                    tile.Movable();
                }
                if(gm.Diagonal(character,tile) == "lowerRight" && (gm.GetAdjacent(character.tile,"down").occupation == null || gm.GetAdjacent(character.tile,"right").occupation == null)) {
                    tile.Movable();
                }
                if(gm.Diagonal(character,tile) == "lowerLeft" && (gm.GetAdjacent(character.tile,"down").occupation == null || gm.GetAdjacent(character.tile,"left").occupation == null)) {
                    tile.Movable();
                }

                if(tile == gm.GetTile(character.positionX+2,character.positionY+1) && gm.GetAdjacent(character.tile,"up").occupation == null && gm.GetTile(character.positionX+1,character.positionY+1).occupation != null) {
                    tile.Movable();
                }
                if(tile == gm.GetTile(character.positionX-2,character.positionY+1) && gm.GetAdjacent(character.tile,"up").occupation == null && gm.GetTile(character.positionX-1,character.positionY+1).occupation != null) {
                    tile.Movable();
                }
                if(tile == gm.GetTile(character.positionX+2,character.positionY-1) && gm.GetAdjacent(character.tile,"down").occupation == null && gm.GetTile(character.positionX+1,character.positionY-1).occupation != null) {
                    tile.Movable();
                }
                if(tile == gm.GetTile(character.positionX-2,character.positionY-1) && gm.GetAdjacent(character.tile,"down").occupation == null && gm.GetTile(character.positionX-1,character.positionY-1).occupation != null) {
                    tile.Movable();
                }

                Oh god..is. there's much more... this is busted
            }*/
            

        }
    }

    
}
