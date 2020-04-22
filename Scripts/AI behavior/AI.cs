using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{

    private GameMaster gm;
    private bool aiTurn;

    public List<Char> enemies;
    public Char mausoleum;
    public Char zombie;

    public GameObject zombieCard;

    public GameObject generatePrefab;
    private GameObject generateEffect;


    void Start() {

        this.aiTurn = false;
        gm = FindObjectOfType<GameMaster>();
        //PlaceTeam();
        //foreach(Char enemy in enemies) {PlaceChar(enemy);}
        //PlaceChar(mausoleum);

        //foreach(Char enemy in enemies) {
        //    GameObject generate = Instantiate(generatePrefab,enemy.transform.position,Quaternion.identity,enemy.transform);
        //    Destroy(generate,4f);
        //}
    }

    void Update() {


        if(gm.canPassTurn) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                this.aiTurn = true;
            }
        }

        if((gm.turn == 2 || gm.turn == 4 || gm.turn == 6) && this.aiTurn == true) {
            this.aiTurn = false;
            gm.UpdateBoard();
            StartCoroutine(BeginTurn());

        }
        if(mausoleum!=null) {
            if(mausoleum.hittable && enemies.Count>0) {
                mausoleum.hittable = false;
                mausoleum.ResetChar();
            }
        }

        
        if(gm.hitCount2>0 && enemies.Count>0) {
            gm.hitCount2=0;
            enemies.Remove(gm.lastHitted);
            gm.lastHitted.tile.occupation = null;
            Destroy(gm.lastHitted.gameObject);
            gm.SetActiveChar();
            gm.UpdateConditions();
            if(gm.activeChar.moveActivations > 0) {
                gm.activeChar.MoveActive();
            }
        }
        if(gm.hitCount2>0 && enemies.Count == 0) {
            gm.hitCount2=0;
            if(gm.lifePoints2[2].activeSelf) {
                gm.lifePoints2[2].SetActive(false);
            } else if (gm.lifePoints2[1].activeSelf) {
                gm.lifePoints2[1].SetActive(false);
            } else if (gm.lifePoints2[0].activeSelf) {
                gm.lifePoints2[0].SetActive(false);
                Destroy(gm.lastHitted.gameObject);
                gm.Team1Over();
            }
            
        }
        
    }


    private IEnumerator BeginTurn() {
        gm.canPassTurn = false;
        while (true) {
            if(enemies.Count>4) {
                int zombieNumber = 0;
                foreach(Char human in GetPlayers()) {
                    foreach(Char zombie in enemies) {
                        if(gm.Distance(zombie,human)==1) {
                            zombieNumber+=1;
                            if(zombieNumber>1) {
                                zombie.tile.Active();
                                yield return new WaitForSeconds(1f);
                                gm.Hit(human.team);
                                yield return new WaitForSeconds(1f);
                                zombie.tile.ResetTile();
                                if(gm.turn == 6) {
                                    StartCoroutine(Mausoleum());
                                } else {
                                    gm.PassTurn();
                                    gm.canPassTurn = true;
                                }
                                yield break;
                            }
                            
                        }
                    }
                }
            }
            if(enemies.Count > 0) {
                
                Char character = enemies[Random.Range(0,enemies.Count)];
                List<Char> players = GetPlayers();

                int j=0;
                while(j<10) {
                    if(!CanSeek(character,players) && MinDistance(character.tile)>1) {
                        character = enemies[Random.Range(0,enemies.Count)];
                    } else {break;}
                    j+=1;
                }
                character.tile.Active();
                bool aiAttacked = false;
                int aiMoves = 2;
                Tile currentPosition = character.tile;


                yield return new WaitForSeconds(1f);
                foreach(Char human in players) {
                    if(gm.Distance(character,human) == 1) {
                        gm.Hit(human.team);
                        aiAttacked = true;
                        yield return new WaitForSeconds(1f);
                        break;
                    }
                }
                if(!aiAttacked) {
                    character.tile.ResetTile();
                    Seek(character,players);
                    character.tile.Active();
                }
                if(currentPosition != character.tile) {aiMoves-=1;currentPosition=character.tile; yield return new WaitForSeconds(1f);}
                if(!aiAttacked && aiMoves == 1) {
                    foreach(Char human in players) {
                        if(gm.Distance(character,human) == 1) {
                            gm.Hit(human.team);
                            aiAttacked = true;
                            yield return new WaitForSeconds(1f);
                            break;
                        }
                    }
                }
                if(!aiAttacked) {
                    character.tile.ResetTile();
                    Seek(character,players);
                    character.tile.Active();
                }
                if(currentPosition != character.tile) {aiMoves-=1;currentPosition=character.tile; yield return new WaitForSeconds(1f);}
                if(!aiAttacked && aiMoves == 0) {
                    foreach(Char human in players) {
                        if(gm.Distance(character,human) == 1) {
                            gm.Hit(human.team);
                            aiAttacked = true;
                            yield return new WaitForSeconds(1f);
                            break;
                        }
                    }
                }
                character.tile.ResetTile();
            }
            if(gm.turn == 6) {
                StartCoroutine(Mausoleum());
            } else {
                gm.PassTurn();
                gm.canPassTurn = true;
            }
            break;
        }

    }

/* your team is randomly placed!
see if theres adj. yes, hit, return. no, seek().. se if theres adj. yes, hit, return, no seek()... see if theres adj, yes, hit, return.
seek() = take 4 directions, eliminate those occupied, check if this is your last move and will leave you diagonal do warrior,
if so, remove, check which remaining brings you closer, move.
mausoleum = doesnt lose turn... if theres no zombie, make zombie... if theres no unprotected, dont protect, if seek returns null for all, dont make them seek.
*/
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

    public void DestinateAI(Char character, Tile tile) {
        PlaceChar(character);
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

    private IEnumerator Mausoleum() {
        gm.canPassTurn = false;
        mausoleum.tile.Active();
        while(true) {
            List<Char> players = GetPlayers();

            bool seekPossible = false;
            foreach(Char enemy in enemies) {
                seekPossible = CanSeek(enemy,players);
                break;
            }

            yield return new WaitForSeconds(1f);
            if(enemies.Count==0 || !seekPossible) {
                GenerateZombie();
                yield return new WaitForSeconds(1f);
                break;
            }
            
            int choice = Random.Range(0,2);
            if(choice == 0) {
                GenerateZombie();
                yield return new WaitForSeconds(1f);
                break;
            }
            if(choice == 1) {
                MakeSeek();
                yield return new WaitForSeconds(1f);
                break;
            }
            
        }
        mausoleum.tile.ResetTile();
        gm.PassTurn();
        gm.canPassTurn = true;

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

    public void GenerateZombie() {
        List<Tile> candidates = new List<Tile>();
        foreach(Tile tile in FindObjectsOfType<Tile>()) {
            if(tile.occupation==null) {
                candidates.Add(tile);
            }
        }
        Tile destine = candidates[Random.Range(0,candidates.Count)];
        Char newEnemy = Instantiate(zombie,new Vector3(4.15f,-4.2f,-8f),Quaternion.identity);
        DestinateAI(newEnemy,destine);
        generateEffect = Instantiate(generatePrefab,newEnemy.transform.position,Quaternion.identity,newEnemy.transform);
        Destroy(generateEffect,4f);
        enemies.Add(newEnemy);
        newEnemy.team=2;

        newEnemy.card = zombieCard;


    }

    private void MakeSeek() {
        List<Char> players = GetPlayers();
        foreach(Char enemy in enemies) {
            Seek(enemy,players);
        }
        
    }

}
