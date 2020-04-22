using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiblingsAI : MonoBehaviour
{

    private GameMaster gm;
    private bool aiTurn;

    public Char yin;
    public Char yang;

    private bool lastYin;
    private bool lastYang;

    public List<Tile> yinTiles;
    public List<Tile> yangTiles;

    public List<Char> depressed;
    public List<Char> needy;
    public Char activeNeedy;

    public GameObject depressedPrefab;
    public GameObject needyPrefab;


    void Start() {

        this.aiTurn = false;
        this.lastYang = false;
        this.lastYin = false;
        this.needy = new List<Char>();
        this.depressed = new List<Char>();
        this.activeNeedy = null;
        gm = FindObjectOfType<GameMaster>();
    }

    void Update() {


        if(gm.canPassTurn) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                this.aiTurn = true;
            }
        }

        if(gm.turn == 2 && !this.lastYang && !this.lastYin && this.aiTurn == true) {
            this.aiTurn = false;
            this.lastYin = true;
            gm.UpdateBoard();
            StartCoroutine(Yin());


        }

        if((gm.turn == 2 || gm.turn == 4 || gm.turn == 6) && this.aiTurn == true) {
            this.aiTurn = false;
            gm.UpdateBoard();
            if(this.lastYin) {
                this.lastYin = false;
                this.lastYang = true;
                StartCoroutine(Yang());
            } else {
                this.lastYang = false;
                this.lastYin = true;
                StartCoroutine(Yin());
            }

        }
        
        if(gm.chooseTurnOrder == 2 && !gm.startGame) {
            PlaceEnemy(yin,yinTiles);
            gm.chooseTurnOrder += 1;
        }
        if(gm.chooseTurnOrder == 4 && !gm.startGame) {
            PlaceEnemy(yang,yangTiles);
            gm.chooseTurnOrder += 1;
        }
        if(gm.chooseTurnOrder == 6 && !gm.startGame) {
            gm.chooseTurnOrder += 1;
            gm.placingDone = true;
            gm.startGame = true;
            gm.teamPanels.SetActive(true);
            gm.canPassTurn = true;
            gm.PassTurn();
        }

        if(this.activeNeedy != null && gm.activeChar != this.activeNeedy) {
            this.needy.Remove(this.activeNeedy);
            Destroy(this.activeNeedy.gameObject.transform.Find("NeedyEffect(Clone)").gameObject);
            this.activeNeedy = null;

        }

        if(gm.activeChar != null && gm.activeChar.turnOrder == gm.turn) {

            if(yin.tile.movable) {
                yin.tile.ResetTile();
            }
            if((yang.hittable||yin.hittable)&&(gm.activeChar.whiteMage||gm.activeChar.darkMage)) {
                yin.ResetChar();
                yang.ResetChar();
            }

            if(this.depressed.Contains(gm.activeChar) && gm.passive) {
                this.depressed.Remove(gm.activeChar);
                Destroy(gm.activeChar.gameObject.transform.Find("DepressedEffect(Clone)").gameObject);
                gm.EndPassive();
            }
            if(this.needy.Contains(gm.activeChar)) {
                this.activeNeedy = gm.activeChar;
                
                if(gm.activeChar.isSkilling || gm.activeChar.isAttacking) {
                    int meelees = 0;
                    foreach(Char human in GetPlayers()) {
                        if(human == gm.activeChar) {
                            continue;
                        } else if(gm.Meelee(human.tile,gm.activeChar.tile)) {
                            meelees += 1;
                        }
                    }
                    if(meelees == 0) {
                        gm.activeChar.isSkilling = false;
                        gm.activeChar.isAttacking = false;
                        gm.UpdateBoard();
                        gm.SetActiveChar();
                        if(gm.activeChar.moveActivations > 0) {
                            gm.activeChar.MoveActive();
                        }
                    }
                }
                
            }
            
        }

        

        
    }

    private void PlaceEnemy(Char character, List<Tile> possibilities) {
        List<Tile> iposs = new List<Tile>(possibilities);
        foreach(Tile tile in iposs) {
            if(tile.occupation != null) {
                possibilities.Remove(tile);
            }
        }
        DestinateAI(character,possibilities[Random.Range(0,possibilities.Count)]);
    }

    /*private IEnumerator Yin() {
        gm.canPassTurn = false;

        Char character = yin;

        List<Char> players = GetPlayers();
        bool usedAction = false;

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
        }
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
        }



        
        gm.PassTurn();
        gm.canPassTurn = true;

    }*/


    private void NoSeek(Char character, List<Char> players) {
        
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
            if(SumDistance(adj,players)<SumDistance(character.tile,players)) {adjTiles.Remove(adj);}
            if(gm.Distance(yin,adj)<gm.Distance(yin,character)) {
                DestinateAI(character,adj);
                return;
            }
            if(gm.Distance(yin,adj)>gm.Distance(yin,character)) {
                adjTiles.Remove(adj);
            }

        }
        if(adjTiles.Count == 0) {return;}

        Tile destine = adjTiles[Random.Range(0,adjTiles.Count)];
        DestinateAI(character,destine);

    }

    private float SumDistance(Tile tile, List<Char> players) {
        float sum = 0f;
        foreach(Char human in players) {
            sum += gm.Distance(human,tile);
        }
        return sum;        
    }
    private float SumDistanceHuman(Tile tile, List<Char> players, Char candidate) {
        float sum = 0f;
        foreach(Char human in players) {
            if(human != candidate) {
                sum += gm.Distance(human,tile);
            }
            
        }
        return sum;        
    }

    private void MoveHuman(Char character, List<Char> players) {

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

            if(SumDistanceHuman(adj,players,character)<SumDistanceHuman(character.tile,players,character) && !character.warrior && gm.Meelee(adj,yin.tile)) {
                DestinateAI(character,adj);
            }
            if(SumDistanceHuman(adj,players,character)<SumDistanceHuman(character.tile,players,character) && character.warrior && gm.Meelee(character.tile,yin.tile) && !gm.Meelee(adj,yin.tile)) {
                DestinateAI(character,adj);
            }
            if(character.warrior && gm.Meelee(character.tile,yin.tile) && !gm.Meelee(adj,yin.tile)) {
                DestinateAI(character,adj);
            }
            if(SumDistanceHuman(adj,players,character)>SumDistanceHuman(character.tile,players,character)) {
                adjTiles.Remove(adj);
            }

        }
        if(adjTiles.Count == 0) {return;}

        DestinateAI(character,adjTiles[Random.Range(0,adjTiles.Count)]);

    }
    private void MoveYin(Char character, List<Char> players, bool attack) {

        List<Tile> adjTiles = new List<Tile>();
        if(gm.GetAdjacent(character.tile,"up")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"up")); }
        if(gm.GetAdjacent(character.tile,"down")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"down")); }
        if(gm.GetAdjacent(character.tile,"left")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"left")); }
        if(gm.GetAdjacent(character.tile,"right")!=null) { adjTiles.Add(gm.GetAdjacent(character.tile,"right")); }
        List<Tile> iadjTiles = new List<Tile>(adjTiles);

        foreach(Tile adj in iadjTiles) {
            if(adj.occupation != null) {adjTiles.Remove(adj);}
        }

        if(attack) {
            iadjTiles = new List<Tile>(adjTiles);
            foreach(Tile adj in iadjTiles) {
                if(gm.Distance(character,yang)<=gm.Distance(yang,adj)) {
                    adjTiles.Remove(adj);
                }

                foreach(Char human in players) {
                    if(gm.Meelee(character.tile,human.tile) && MinDistance(adj)<MinDistance(character.tile)) {
                        adjTiles.Remove(adj);
                    }
                }
                if(MinDistance(adj)<MinDistance(character.tile)) {
                    DestinateAI(character,adj);
                    return;
                }

            }
            if(adjTiles.Count == 0) {return;}

            DestinateAI(character,adjTiles[Random.Range(0,adjTiles.Count)]);
            return;
        }


        iadjTiles = new List<Tile>(adjTiles);
        foreach(Tile adj in iadjTiles) {

            foreach(Char human in players) {
                if(MinDistance(character.tile)==1 && MinDistance(adj)==2 && gm.Meelee(adj,human.tile)) {
                    DestinateAI(character,adj);
                    return;
                }
            }
            foreach(Char human in players) {
                if(MinDistance(adj)==2 && gm.Meelee(adj,human.tile)) {
                    DestinateAI(character,adj);
                    return;
                }
            }
            foreach(Char human in players) {
                if(MinDistance(character.tile)==2 && gm.Meelee(character.tile,human.tile)) {
                    return;
                }
            }

            if(!GetsNearer(character,adj)) {
                adjTiles.Remove(adj);
            }
            if(GetsNearer(character,adj) && gm.Distance(yang,adj)<gm.Distance(character,yang)) {
                DestinateAI(character,adj);
                return;
            }
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

    private IEnumerator Yang() {
        gm.canPassTurn = false;

        Char character = yang;
        character.tile.Active();

        List<Char> players = GetPlayers();
        bool usedAction = false;

        yield return new WaitForSeconds(2f);

        Tile yinPos = yin.tile;
        MoveYin(yin,players, false);
        if(yinPos != yin.tile) {yield return new WaitForSeconds(2f);}

        Tile yangPos = yang.tile;
        character.tile.ResetTile();
        NoSeek(character,players);
        if(yangPos != yang.tile) {character.tile.Active(); yield return new WaitForSeconds(2f);}
        yangPos = yang.tile;
        character.tile.ResetTile();
        NoSeek(character,players);
        if(yangPos != yang.tile) {character.tile.Active(); yield return new WaitForSeconds(2f);}

        character.tile.Active();
        foreach(Char human in players) {
            if(gm.Meelee(yin.tile,human.tile)) {
                gm.Hit(human.team);
                usedAction = true;
                yield return new WaitForSeconds(2f);
                MoveYin(yin,players,true);
                break;

            }
        }

        if(!usedAction) {
            List<Char> candidatesSkill = new List<Char>();
            float dist = 0f;
            while(candidatesSkill.Count == 0 && !usedAction) {
                dist += 1;
                foreach(Char human in players) {
                    if(gm.Distance(human,character.tile) == dist) {
                        candidatesSkill.Add(human);
                        if(human.warrior && !needy.Contains(human)) {
                            this.needy.Add(human);
                            Instantiate(needyPrefab,human.transform.position,Quaternion.identity,human.transform);
                            candidatesSkill.Clear();
                            usedAction = true;
                            break;
                        }
                    }
                }
            }
            List<Char> ihuman = new List<Char>(candidatesSkill);
            foreach(Char human in ihuman) {
                if(needy.Contains(human)) {
                    candidatesSkill.Remove(human);
                }
            }
            if(candidatesSkill.Count>0) {
                Char target = candidatesSkill[Random.Range(0,candidatesSkill.Count)];
                this.needy.Add(target);
                Instantiate(needyPrefab,target.transform.position,Quaternion.identity,target.transform);
                candidatesSkill.Clear();
            }
            
        }
        character.tile.ResetTile();
        gm.PassTurn();
        gm.canPassTurn = true;

    }


    private IEnumerator Yin() {
        gm.canPassTurn = false;

        Char character = yin;
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
            if(human.warrior) {
                candidatesPassive.Clear();
                candidatesPassive.Add(human);
                break;
            }
        }


        if(candidatesPassive.Count>0) {
            MoveHuman(candidatesPassive[Random.Range(0,candidatesPassive.Count)],players);
            yield return new WaitForSeconds(2f);
        }

        foreach(Char human in players) {
            List<Char> others = new List<Char>();
            foreach(Char ihuman in players) {
                if(ihuman != human) {
                    others.Add(ihuman);
                }
            }
            if(gm.Meelee(human.tile,others[0].tile) && gm.Meelee(human.tile,others[1].tile)) {
                gm.Hit(human.team);
                usedAction = true;
                yield return new WaitForSeconds(2f);
                break;
            }
        }

        if(!usedAction) {
            List<Char> candidatesSkill = new List<Char>();
            float idist = 0f;
            while(candidatesSkill.Count == 0 && !usedAction) {
                idist += 1;
                foreach(Char human in players) {
                    if(gm.Distance(human,character.tile) == idist) {
                        candidatesSkill.Add(human);
                    }
                }
            }
            List<Char> ihuman = new List<Char>(candidatesSkill);
            foreach(Char human in ihuman) {
                if(depressed.Contains(human)) {
                    candidatesSkill.Remove(human);
                }
            }
            if(candidatesSkill.Count>0) {
                Char target = candidatesSkill[Random.Range(0,candidatesSkill.Count)];
                this.depressed.Add(target);
                Instantiate(depressedPrefab,target.transform.position,Quaternion.identity,target.transform);
                candidatesSkill.Clear();
            }
            
        }
        character.tile.ResetTile();
        gm.PassTurn();
        gm.canPassTurn = true;

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
