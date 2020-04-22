using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public float offsetX;
    public float offsetY;
    public float scale;

    public float positionX;
    public float positionY;
    public Char occupation;

    private SpriteRenderer rend;
    public Color walkableColor;
    public Color hittableColor;
    public Color movableColor;
    public Color targetedColor;
    public Color turnColor;
    private GameMaster gm;

    public bool walkable;
    public bool hittable;
    public bool movable;
    public bool targeted;
    public bool placeable;

    void Start()
    {
        this.positionX = Mathf.Round((this.transform.position.x + offsetX)/scale);
        this.positionY = Mathf.Round((this.transform.position.y + offsetY)/scale);
        rend = GetComponent<SpriteRenderer>();
        gm = FindObjectOfType<GameMaster>();
        SetOccupation();

        ResetTile();
        
        
    }

    private void OnMouseDown() {
        if(this.walkable) {
            gm.DestinateMove(gm.activeChar,this);
        }

        if(this.movable && gm.movingChar != null) {
            gm.DestinateMove(gm.movingChar,this);
            if(gm.passive) {gm.EndPassive();}
            if(gm.activeChar.isSkilling) {gm.EndSkill();}
        }

        if(this.placeable) {
            gm.PlaceCharacter(this);
        }
    }

    public void SetOccupation() {
        this.occupation = null;
        foreach(Char occupant in FindObjectsOfType<Char>()) {
            if(occupant.positionX == this.positionX && occupant.positionY == this.positionY) {
                this.occupation = occupant;
            }
        }
    }

    public void Walkable() {
        rend.color = walkableColor;
        this.walkable = true;
    }
    public void Hittable() {
        if (!(this.occupation?.team == gm.activeChar.team)) {
            rend.color = hittableColor;
            this.hittable = true;
        }
    }
    public void Movable() {
        rend.color = movableColor;
        this.movable = true;
    }
    public void Placeable() {
        rend.color = movableColor;
        this.placeable = true;
    }
    public void Active() {
        rend.color = turnColor;
    }
    public void Targeted() {
        rend.color = targetedColor;
        this.targeted = true;
    }
    public void ResetTile() {
        this.walkable = false;
        this.hittable = false;
        this.movable = false;
        this.targeted = false;
        this.placeable = false;
        rend.color = Color.white;
    }

    // Deprecated function
    /*public void ExecuteMove() {
        float directionX;
        float directionY;
        if(this.positionX > gm.activeChar.positionX) {directionX = 1;} else if(this.positionX < gm.activeChar.positionX) {directionX = -1;} else {directionX = 0;}
        if(this.positionY > gm.activeChar.positionY) {directionY = 1;} else if(this.positionY < gm.activeChar.positionY) {directionY = -1;} else {directionY = 0;}
        gm.activeChar.transform.position = new Vector3(gm.activeChar.transform.position.x+directionX*scale,gm.activeChar.transform.position.y+directionY*scale,-gm.activeChar.turnOrder);
        gm.activeChar.UpdatePosition();
        gm.UpdateBoard();

        gm.activeChar.moveActivations -= 1;
        if(gm.activeChar.moveActivations > 0) {
            gm.activeChar.MoveActive();
        }
        
        this.Active();
    }*/
}
