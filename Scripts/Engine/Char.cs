using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Char : MonoBehaviour
{
    public float offsetX;
    public float offsetY;
    public float space;

    public bool warrior;
    public bool rogue;
    public bool whiteMage;
    public bool darkMage;
    public bool minion;
    public bool place;
    public int moveActivations;
    public bool moveActing;

    public int team;
    public int turnOrder;
    public bool hittable;
    public Color hittableColor;
    public bool movable;
    public Color movableColor;
    public bool targeted;
    public Color targetedColor;
    private SpriteRenderer rend;

    public float positionX;
    public float positionY;

    public Tile tile;

    public GameMaster gm;

    public bool isAttacking;
    public bool isSkilling;
    public bool attacked;
    public bool usedSkill;
    public bool usedAction;

    public bool hasAlternativeMoveSkill;

    public bool placeSelected;
    public bool isSet;

    public GameObject card;
    public Sprite iconImage;
    public Sprite cardImage;




    public abstract void Attack();
    public abstract void Skill();
    public abstract void TurnEffects();
    public abstract void Conditions();

    public virtual void AlternativeMove(Char character) {}

    void Awake() {
        this.positionX = Mathf.Round((this.transform.position.x + offsetX)/space);
        this.positionY = Mathf.Round((this.transform.position.y + offsetY)/space);
        this.moveActivations = 2;
        this.moveActing = false;
        this.isAttacking = false;
        this.isSkilling = false;
        this.attacked = false;
        this.usedSkill = false;
        this.usedAction = false;
        this.placeSelected = false;
        this.isSet = false;
        AlternativeAbilities();
        rend = GetComponent<SpriteRenderer>();
        gm = FindObjectOfType<GameMaster>();
        ResetChar();
        SetTile();



    }

    public virtual void AlternativeAbilities() {
        this.hasAlternativeMoveSkill = false;
    }

    private void OnMouseDown() {

        if(gm.startGame == false && this.team % 2 == gm.chooseTurnOrder % 2 && !this.isSet) {
            foreach(Char character in FindObjectsOfType<Char>()) {
                character.ResetChar();
            }
            foreach(Tile tile in FindObjectsOfType<Tile>()) {
                tile.ResetTile();
                if(tile.occupation == null && this.team % 2 == 1 && tile.positionX <= (gm.chooseTurnOrder-1)/2 + 1) {
                    tile.Placeable();
                }
                if(tile.occupation == null && this.team % 2 == 0 && tile.positionX >= 7 - gm.chooseTurnOrder/2) {
                    tile.Placeable();
                }
            }
            this.Placeable();
        }
        
        
        if(gm.activeChar == this && gm.passive) {
            gm.EndPassive();
        }
        if(this.hittable) {
            gm.lastHitted = this;
            gm.Hit(this.team);
            if(gm.passive) {
                gm.EndPassive();
            }
            if(gm.activeChar.isAttacking) {
                gm.UpdateConditions();
                gm.EndAttack();
            }
        }
        
        if(this.isSet && this.tile!=null) {

            if(this.tile.movable) {
                this.Movable();
                gm.movingChar = this;
                this.ChooseMove();
            }

            if(this.tile.targeted) {
                this.Targeted();
                gm.activeChar.AlternativeMove(this);
            }

        }
        

    }


    private void OnMouseOver() {
        if(true/*Input.GetMouseButtonDown(1)*/) {
            gm.ShowCard(this);
        }
    }

    public void SetTile() {
        this.tile = null;
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if (tile.positionX == this.positionX && tile.positionY == this.positionY) {
                this.tile = tile;
            }
        }
    }

    public void ChooseMove() {

        if(gm.activeChar.isSkilling && gm.activeChar.hasAlternativeMoveSkill) {
            
            gm.activeChar.AlternativeMove(this);
        } else {
            foreach(Tile tile in FindObjectsOfType<Tile>()) {
                if(tile.occupation != gm.activeChar) {
                    tile.ResetTile();
                }
                if (gm.Distance(this,tile) == 1 && tile.occupation == null) {
                    tile.Movable();
                }
            }   
        }
    }

    public virtual void MoveActive() {
        this.moveActing = true;
        
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if (gm.Distance(this,tile) == 1 && tile.occupation == null) {
                tile.Walkable();
            }
        }

    }
    public void UpdatePosition() {
        this.positionX = Mathf.Round((this.transform.position.x + offsetX)/space);
        this.positionY = Mathf.Round((this.transform.position.y + offsetY)/space);
    }

    public void ResetChar() {
        this.hittable = false;
        this.movable = false;
        this.targeted = false;
        this.placeSelected = false;
        if(rend!=null) {rend.color = Color.white;}
        
    }

    public void Hittable() {
        this.hittable = true;
        rend.color = hittableColor;

    }

    public void Movable() {
        this.movable = true;
        rend.color = movableColor;
    }

    public void Targeted() {
        this.targeted = true;
        rend.color = targetedColor;
    }

    public void Placeable() {
        this.placeSelected = true;
        rend.color = movableColor;
    }

    public void Passive() {
        gm.passive = true;
        if(warrior) {
            foreach(Tile tile in FindObjectsOfType<Tile>()) {
                if(gm.Meelee(this.tile,tile)){
                    tile.Hittable();
                    if(tile.occupation?.team != this.team) {
                        tile.occupation?.Hittable();
                    }
                }
            }
        }

        if(rogue) {
            foreach(Tile tile in FindObjectsOfType<Tile>()) {
                if(gm.Meelee(this.tile,tile)){
                    tile.Movable();
                }
            }

        }

        if(whiteMage) {
            float iwDistance = 1f;
            bool wsearch = true;
            while(wsearch) {
                foreach(Char character in FindObjectsOfType<Char>()) {
                    if(character.team == this.team && gm.Distance(this,character) == iwDistance) {
                        character.tile.Movable();
                        wsearch = false;
                    }
                }
                iwDistance += 1;
            }

        }

        if(darkMage) {
            float idDistance = 1f;
            bool dsearch = true;
            while(dsearch) {
                foreach(Char character in FindObjectsOfType<Char>()) {
                    if(character.team != this.team && gm.Distance(this,character) == idDistance) {
                        character.tile.Movable();
                        dsearch = false;
                    }
                }
                idDistance += 1;
            }
        }
    }
}
