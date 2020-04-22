using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorAI : MonoBehaviour
{

    private GameMaster gm;
    private bool aiTurn;

    public List<Char> lackeys;
    public Char horror;

    public List<Tile> firstLackeyTiles;
    public List<Tile> secondLackeyTiles;
    public List<Tile> horrorTiles;

    public List<Char> frightened;

    public GameObject healPrefab;
    private GameObject healEffect;
    public GameObject fearPrefab;
    private GameObject fearEffect;
    public GameObject horrorPrefab;
    private GameObject horrorEffect;


    void Start() {

        this.aiTurn = false;
        this.frightened = new List<Char>();
        gm = FindObjectOfType<GameMaster>();
    }

    void Update() {


        if(gm.canPassTurn) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                this.aiTurn = true;
            }
        }

        if((gm.turn == 2 || gm.turn == 4) && this.aiTurn == true) {
            this.aiTurn = false;
            gm.UpdateBoard();
            StartCoroutine(LackeyTurn());

        }
        if(gm.turn == 6 && this.aiTurn == true) {
            this.aiTurn = false;
            gm.UpdateBoard();
            StartCoroutine(Horror());
        }
        
        if(gm.chooseTurnOrder == 2 && !gm.startGame) {
            PlaceEnemy(lackeys[0],firstLackeyTiles);
            gm.chooseTurnOrder += 1;
        }
        if(gm.chooseTurnOrder == 4 && !gm.startGame) {
            PlaceEnemy(lackeys[1],secondLackeyTiles);
            gm.chooseTurnOrder += 1;
        }
        if(gm.chooseTurnOrder == 6 && !gm.startGame) {
            PlaceEnemy(horror,horrorTiles);
            gm.chooseTurnOrder += 1;
            gm.placingDone = true;
            gm.startGame = true;
            gm.teamPanels.SetActive(true);
            gm.canPassTurn = true;
            gm.PassTurn();
        }

        if(gm.activeChar != null && gm.activeChar.turnOrder == gm.turn) {
            if(this.frightened.Contains(gm.activeChar)) {
                this.frightened.Remove(gm.activeChar);
                Destroy(gm.activeChar.gameObject.transform.Find("FearEffect(Clone)").gameObject);
                if(gm.Distance(gm.activeChar,lackeys[0])==1 || gm.Distance(gm.activeChar,lackeys[1])==1) {
                    gm.activeChar.moveActivations -= 1;
                }
            }
            
        }

        
        if(lackeys.Count>0) {
            if(lackeys[0].hittable || lackeys[1].hittable) {
                lackeys[0].ResetChar();
                lackeys[1].ResetChar();
            }
            if(horror.tile!=null && horror.tile.movable) {
                horror.tile.ResetTile();
            }
        }
        
        

        
    }

    private void PlaceEnemy(Char character, List<Tile> possibilities) {
        int choice = Random.Range(0,possibilities.Count);
        DestinateAI(character,possibilities[choice]);
    }

    private IEnumerator LackeyTurn() {
        gm.canPassTurn = false;

        Char character = lackeys[(gm.turn-2)/2];
        character.tile.Active();

        List<Char> players = GetPlayers();
        bool usedSkill = false;

        yield return new WaitForSeconds(1f);

        foreach(Char human in players) {
            if(gm.Meelee(character.tile,human.tile)) {
                gm.Hit(human.team);
                yield return new WaitForSeconds(1f);
                break;
            }
        }

        foreach(Char human in players) {
            if(gm.Distance(character,human)==1 && !this.frightened.Contains(human)) {
                Fear(character,players);
                usedSkill = true;
                yield return new WaitForSeconds(1f);
                break;
            }
        }
        if(CanSeek(character,players)) {
            character.tile.ResetTile();
            Seek(character,players);
            character.tile.Active();
            yield return new WaitForSeconds(1f);
        }

        if(!usedSkill) {
            foreach(Char human in players) {
                if(gm.Distance(character,human)==1 && !this.frightened.Contains(human)) {
                    Fear(character,players);
                    usedSkill = true;
                    yield return new WaitForSeconds(1f);
                    break;
                }
            }
        }/*
        if(CanSeek(character,players)) {
            Seek(character,players);
            yield return new WaitForSeconds(1f);
        }

        if(!usedSkill) {
            foreach(Char human in players) {
                if(gm.Distance(character,human)==1 && !this.frightened.Contains(human)) {
                    Fear(character,players);
                    usedSkill = true;
                    yield return new WaitForSeconds(1f);
                    break;
                }
            }
        }*/



        character.tile.ResetTile();
        gm.PassTurn();
        gm.canPassTurn = true;

    }

    private void Fear(Char character, List<Char> players) {
        List<Char> candidates = new List<Char>();
        foreach(Char human in players) {
            if(gm.Distance(character,human)==1 && !this.frightened.Contains(human)) {
                candidates.Add(human);
            }
        }
        Char target = candidates[Random.Range(0,candidates.Count)];
        this.frightened.Add(target);
        Instantiate(fearPrefab,target.transform.position,Quaternion.identity,target.transform);
        //effect

    }

    private void Seek(Char character, List<Char> players) {
        foreach(Char human in players) {
            if(gm.Distance(human,character)==1) {return;}
        }
        List<Tile> adjTiles = new List<Tile>();
        if(gm.GetAdjacent(character.tile,"up")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"up")); }
        if(gm.GetAdjacent(character.tile,"down")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"down")); }
        if(gm.GetAdjacent(character.tile,"left")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"left")); }
        if(gm.GetAdjacent(character.tile,"right")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"right")); }
        List<Tile> iadjTiles = new List<Tile>(adjTiles);
        foreach(Tile adj in iadjTiles) {
            if(adj.occupation != null) {adjTiles.Remove(adj);}
        }
        iadjTiles = new List<Tile>(adjTiles);
        foreach(Tile adj in iadjTiles) {
            if(!GetsNearer(character,adj)) {adjTiles.Remove(adj);}
        }
        if(adjTiles.Count == 0) {return;}

        Tile destine = adjTiles[Random.Range(0,adjTiles.Count)];
        DestinateAI(character,destine);

    }
    private void MoveHuman(Char character, List<Char> lackeys) {

        List<Tile> adjTiles = new List<Tile>();
        if(gm.GetAdjacent(character.tile,"up")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"up")); }
        if(gm.GetAdjacent(character.tile,"down")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"down")); }
        if(gm.GetAdjacent(character.tile,"left")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"left")); }
        if(gm.GetAdjacent(character.tile,"right")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"right")); }
        List<Tile> iadjTiles = new List<Tile>(adjTiles);
        foreach(Tile adj in iadjTiles) {
            if(adj.occupation != null) {adjTiles.Remove(adj);}
        }
        iadjTiles = new List<Tile>(adjTiles);
        foreach(Tile adj in iadjTiles) {

            if((gm.Distance(lackeys[0],adj)==1||gm.Distance(lackeys[1],adj)==1)&&gm.Distance(horror,adj)<gm.Distance(character,horror)) {
                DestinateAI(character,adj);
                return;
            }
            if( (gm.Distance(lackeys[0],adj) < gm.Distance(lackeys[1],adj)) && gm.Distance(lackeys[0],adj)<gm.Distance(lackeys[0],character) ) {
                DestinateAI(character,adj);
                return;
            }
            if( (gm.Distance(lackeys[0],adj) > gm.Distance(lackeys[1],adj)) && gm.Distance(lackeys[1],adj)<gm.Distance(lackeys[1],character) ) {
                DestinateAI(character,adj);
                return;
            }


            if(gm.Distance(lackeys[0],adj)>gm.Distance(lackeys[0],character) && gm.Distance(lackeys[1],adj)>gm.Distance(lackeys[1],character)) {adjTiles.Remove(adj);}
        }
        if(adjTiles.Count == 0) {return;}

        DestinateAI(character,adjTiles[Random.Range(0,adjTiles.Count)]);

    }

    public void DestinateAI(Char character, Tile tile) {
        float distanceX = tile.positionX - character.positionX;
        float distanceY = tile.positionY - character.positionY;
        character.transform.position += new Vector3(distanceX*gm.scale,distanceY*gm.scale,0);
        PlaceChar(character);
        gm.UpdateConditions();
    }

    private bool GetsNearer(Char character, Tile adj) {
        if(MinDistance(adj)<MinDistance(character.tile)) {return true;} else {return false;}
    }

    private float MinDistance(Tile position) {
        List<Char> players = GetPlayers();
        float dist = 0f;
        bool search = true;
        while(search) {
            dist += 1;
            foreach(Char human in players) {
                if(gm.Distance(human,position) == dist) {
                    search = false;
                }
            }
        }
        return dist;
    }

    private List<Char> GetPlayers() {
        List<Char> players = new List<Char>();
        foreach(Char character in FindObjectsOfType<Char>()) {
            if(character.team == 1) {
                players.Add(character);
            }
        }
        return players;
    }

    private void PlaceChar(Char character) {
        character.UpdatePosition();
        character.SetTile();
        character.tile.SetOccupation();
        character.isSet = true;
    }

    private IEnumerator Horror() {
        gm.canPassTurn = false;

        Char character = horror;
        character.tile.Active();

        List<Char> players = GetPlayers();
        bool usedAction = false;

        yield return new WaitForSeconds(2f);

        List<Char> candidatesPassive = new List<Char>();
        float dist = 0f;
        while(candidatesPassive.Count == 0) {
            dist += 1;
            foreach(Char human in players) {
                if(gm.Distance(human,character.tile) == dist) {
                    candidatesPassive.Add(human);
                }
            }
        }
        List<Char> icandidates = new List<Char>(candidatesPassive);
        foreach(Char human in icandidates) {
            if(gm.Distance(human,lackeys[0])==1 || gm.Distance(human,lackeys[1])==1) {
                candidatesPassive.Remove(human);
            }
        }
        if(candidatesPassive.Count>0) {
            MoveHuman(candidatesPassive[Random.Range(0,candidatesPassive.Count)],lackeys);
            yield return new WaitForSeconds(2f);
        }

        if(CanTorment(players) && (gm.hitCount2<4||gm.hitCount1==4)) {
            Torment(players);
            usedAction = true;
            yield return new WaitForSeconds(2f);
        }

        if(!usedAction) {
            Seek(lackeys[0],players);
            Seek(lackeys[1],players);
            yield return new WaitForSeconds(2f);
            if(gm.hitCount2==4) {
                healEffect = Instantiate(healPrefab,horror.transform.position,Quaternion.identity,horror.transform);
                Destroy(healEffect,4f);
                gm.hitCount2 -= 1;
                gm.lifePoints2[1].SetActive(true);
                usedAction = true;
                yield return new WaitForSeconds(2f);

            }
        }

        

        List<Tile> targetTile = new List<Tile>();
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if(tile.occupation == null && (gm.Distance(lackeys[0],tile)<3 || gm.Distance(lackeys[1],tile)<3)) {
                targetTile.Add(tile);
            }
        }
        character.tile.ResetTile();
        DestinateAI(character,targetTile[Random.Range(0,targetTile.Count)]);
        character.tile.Active();
        Instantiate(horrorPrefab,horror.transform.position,Quaternion.identity,horror.transform);
        Destroy(horror.gameObject.transform.Find("HorrorEffect(Clone)").gameObject, 4f);
        yield return new WaitForSeconds(2f);

        character.tile.ResetTile();
        gm.PassTurn();
        gm.canPassTurn = true;

    }

    private bool CanTorment(List<Char> players) {
        float dist = 0f;
        List<Char> tormentCandidates = new List<Char>();
        while(tormentCandidates.Count==0) {
            dist += 1;
            foreach(Char human in players) {
                if(gm.Distance(human,horror.tile) == dist) {
                    tormentCandidates.Add(human);
                }
            }
        }
        foreach(Char human in tormentCandidates) {
            if(gm.Distance(human,lackeys[0])==1||gm.Distance(human,lackeys[1])==1) {
                return true;
            }
        }
        return false;
    }

    private void Torment(List<Char> players) {
        float dist = 0f;
        List<Char> tormentCandidates = new List<Char>();
        while(tormentCandidates.Count==0) {
            dist += 1;
            foreach(Char human in players) {
                if(gm.Distance(human,horror.tile) == dist) {
                    tormentCandidates.Add(human);
                }
            }
        }
        foreach(Char human in tormentCandidates) {
            if(gm.Distance(human,lackeys[0])==1||gm.Distance(human,lackeys[1])==1) {
                gm.Hit(human.team);
                return;
            }
        }

    }

    private void PlaceTeam() {

        Tile firstTile = GameObject.Find("Tile26").GetComponent<Tile>();
        Tile secondTile = GameObject.Find("Tile15").GetComponent<Tile>();
        Tile thirdTile = GameObject.Find("Tile16").GetComponent<Tile>();
        List<Char> players = GetPlayers();

        Char first = players[Random.Range(0,3)];
        players.Remove(first);
        first.turnOrder = 1;
        float distanceX = firstTile.positionX - first.positionX;
        float distanceY = firstTile.positionY - first.positionY;
        first.transform.position += new Vector3(distanceX*gm.scale,distanceY*gm.scale,-first.turnOrder);
        PlaceChar(first);

        Char second = players[Random.Range(0,2)];
        players.Remove(second);
        second.turnOrder = 3;
        distanceX = secondTile.positionX - second.positionX;
        distanceY = secondTile.positionY - second.positionY;
        second.transform.position += new Vector3(distanceX*gm.scale,distanceY*gm.scale,-second.turnOrder);
        PlaceChar(second);

        Char third = players[0];
        third.turnOrder = 5;
        distanceX = thirdTile.positionX - third.positionX;
        distanceY = thirdTile.positionY - third.positionY;
        third.transform.position += new Vector3(distanceX*gm.scale,distanceY*gm.scale,-third.turnOrder);
        PlaceChar(third);



    } 

    private bool CanSeek(Char character, List<Char> players) {
        foreach(Char human in players) {
            if(gm.Distance(human,character)==1) {return false;}
        }
        List<Tile> adjTiles = new List<Tile>();
        if(gm.GetAdjacent(character.tile,"up")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"up")); }
        if(gm.GetAdjacent(character.tile,"down")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"down")); }
        if(gm.GetAdjacent(character.tile,"left")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"left")); }
        if(gm.GetAdjacent(character.tile,"right")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"right")); }
        List<Tile> iadjTiles = new List<Tile>(adjTiles);
        foreach(Tile adj in iadjTiles) {
            if(adj.occupation != null) {adjTiles.Remove(adj);}
        }
        iadjTiles = new List<Tile>(adjTiles);
        foreach(Tile adj in iadjTiles) {
            if(!GetsNearer(character,adj)) {adjTiles.Remove(adj);}
        }
        if(adjTiles.Count == 0) {
            return false;
        } else {
            return true;
        }

    }

}
