using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(StatsControllerScript))]
[RequireComponent(typeof(TransitionController))]
[RequireComponent(typeof(TextController))]
public class GridScript : MonoBehaviour
{
    private TextController textController;
    private TransitionController transitionController;
    private StatsControllerScript statsController;
    
    private readonly string mapURL = "https://jobfair.nordeus.com/jf24-fullstack-challenge/test";
    
    private int[,] tileHeights;
    private IslandTileScript[,] tiles;
    
    private Dictionary<int, float> islandSizes;
    private List<SortedSet<int>> sameIslands;
    
    private float maxIslandSize = -1f;
    
    private float lives;

    [Header("Settings")]
    [SerializeField] private bool timed;
    [SerializeField] private bool clutter;
    
    public static bool online = true;

    [Header("Prefabs")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject clutterPrefab;

    [Header("Colors")]
    [SerializeField] private Color[] heightmapColors;
    [SerializeField] private Color waterColor;
    [SerializeField] private Color heartRed, heartBlack;
    
    [Header("Graphics")]
    [SerializeField] private Sprite[] clutterSprites; // [0, 3] - BEACH, [4, 8] - FOREST
    [Space(5)]
    [SerializeField] private SpriteRenderer[] heartRenderers;

    [Header("UI")] 
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetButton;
    
    [Header("Audio")]
    [SerializeField] private AudioControllerScript audioController;
    
    [Header("Timed")]
    [SerializeField] private Transform timerBar;
    [SerializeField] private float tickLength = 1f;
    [SerializeField] private float tickPenalty = 1f;
    [SerializeField] private float tickPenaltyScale = 1f;
    [SerializeField] private float missPenalty = 1f;
    [SerializeField] private float missPenaltyScale = 1f;

    private static int timed_score = 0;
    
    private bool timerActive = false;
    private float lastTimer;
    private bool finishedLevel = false;
    private static readonly int ColorHash = Shader.PropertyToID("_Color");

    void Start()
    {
        finishedLevel = false;

        tileHeights = new int[30, 30];
        tiles = new IslandTileScript[30, 30];

        islandSizes = new Dictionary<int, float>();
        sameIslands = new List<SortedSet<int>>();
        
        textController = GetComponent<TextController>();
        transitionController = GetComponent<TransitionController>();
        statsController = GetComponent<StatsControllerScript>();

        backButton.onClick.AddListener(ButtonBackClick);
        resetButton.onClick.AddListener(ButtonResetClick);
        
        if (timed)
        {
            lives = 10;
            lastTimer = Time.time;

            tickPenalty *= Mathf.Pow(tickPenaltyScale, timed_score);
            missPenalty *= Mathf.Pow(missPenaltyScale, timed_score);
            
            textController.SetScoreText(timed_score);
        }
        else
        {
            lives = 3;
            heartRenderers[0].color = heartRenderers[1].color = heartRenderers[2].color = heartRed;

            statsController.AddPlayedGame(online);
            textController.SetSuccessText(0);
        }

        if (online)
        {
            StartCoroutine(GetMapMatrix());
        }
        else
        {
            GenerateMapMatrix();
            PlaceTiles();
            
            transitionController.ShowScreen();
            if(timed) timerActive = true;
        }
        
    }

    private IEnumerator GetMapMatrix()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(mapURL);
        
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string[] heightStrings = webRequest.downloadHandler.text.Split(' ', '\n');

            try
            {
                for(int i = 0; i < 30; i++)
                for (int j = 0; j < 30; j++)
                {
                    tileHeights[i, j] = Convert.ToInt32(heightStrings[30 * i + j]);
                }
                
                PlaceTiles();
            
                transitionController.ShowScreen();
                if(timed) timerActive = true;
            }
            catch (Exception e)
            {
                PlaceTiles();
            
                transitionController.ShowScreen();
                transitionController.ShowTitle("ERROR");
            }

        }
        else
        {
            PlaceTiles();
            
            transitionController.ShowScreen();
            transitionController.ShowTitle("ERROR");
        }
    }
    private void GenerateMapMatrix()
    {
        int[,] map = new int[30,30];
        float xOrigin = Random.Range(-100f, 100f), yOrigin = Random.Range(-100f, 100f);
        float waterLevel = Random.Range(.45f, .6f), perlinScale = Random.Range(4f, 8f);

        for (int i = 0; i < 30; i++)
        for (int j = 0; j < 30; j++)
        {
            float point = Mathf.PerlinNoise(
                xOrigin + i/30f * perlinScale,
                yOrigin + j/30f * perlinScale
            );
            
            point = Mathf.Clamp(point, 0, 1);
            
            if (point < waterLevel)
            {
                map[i,j] = 0;
                continue;
            }

            float heightf = (point - waterLevel)/(1 - waterLevel) * 1000;
            heightf = Mathf.Clamp(heightf, 0, 1000);
            map[i,j] = Mathf.CeilToInt(heightf);
        }
        
        tileHeights = map;
    }
    
    private void Update()
    {
        if (!timed) return;
        if (!timerActive) { lastTimer = Time.time; return; }

        if (!(Time.time - lastTimer > tickLength)) return;
        
        lastTimer = Time.time;
        lives -= tickLength*tickPenalty;
        if (lives <= 0)
        {
            timerBar.localScale = new Vector3(1, 0, 1);
            timerActive = false;

            if (finishedLevel) return;
            finishedLevel = true;
                    
            int highScore = StatsControllerScript.GetHighScore(online);
            if (highScore < timed_score)
            {
                audioController.PlaySuccess();
                statsController.AddHighScore(online, timed_score);
                transitionController.ShowTitle("HIGH SCORE");
            }
            else
            {
                audioController.PlayFailure();
                transitionController.ShowTitle("FAILURE");
            }

        }
        else
        {
            timerBar.localScale = new Vector3(1, lives, 1);
            timerBar.position = new Vector3(timerBar.position.x, timerBar.position.y, -(10 - lives)/2);
        }
    }

    private void PlaceTiles()
    {
        int cid = 0;

        for(int i = 0; i < 30; i++)
        for(int j = 0; j < 30; j++)
        {
            // add script to tiles[,]
            tiles[i,j] =
                Instantiate(
                    tilePrefab,
                    new Vector3(i - 30/2 + 0.5f, 0, j - 30/2 + 0.5f),
                    new Quaternion(),
                    transform)
                    .GetComponent<IslandTileScript>();

            // add heights to each tile
            tiles[i, j].height = Mathf.Clamp(tileHeights[i,j],0,1000);
            
            if (tiles[i,j].height == 0) // if a water tile
            {
                tiles[i, j].transform.localScale = new Vector3(1, 0.25f, 1);
                
                tiles[i, j].transform.GetComponent<MeshRenderer>().material.SetColor(ColorHash, waterColor);
            }
            else // if a land tile
            {
                // position and scale
                tiles[i, j].transform.localScale = new Vector3(1, 4 * tiles[i,j].height / 1000f, 1);

                // determine color
                int closestColorIndex = Mathf.FloorToInt(((tiles[i,j].height-1) / 1000f) * heightmapColors.Length);
                Color nextColor = heightmapColors[closestColorIndex];
                if (closestColorIndex != (heightmapColors.Length - 1))
                {
                    // mix height colors
                    int singleColorExtent = (1000 / heightmapColors.Length);
                    nextColor = Color.Lerp(
                        heightmapColors[closestColorIndex],
                        heightmapColors[closestColorIndex + 1],
                        (1f*tiles[i,j].height % singleColorExtent)/singleColorExtent
                    );
                }
                tiles[i, j].transform.GetComponent<MeshRenderer>().material.SetColor(ColorHash, nextColor);
                
                // determine clutter
                if (clutter && Random.value < .06f && tiles[i,j].height < 700)
                {
                    Transform clutterprefab = Instantiate(
                        clutterPrefab,
                        tiles[i,j].transform.position + new Vector3(
                            Random.Range(-0.25f, 0.25f),
                            tiles[i,j].transform.localScale.y + 5f,
                            Random.Range(-0.25f, 0.25f)
                            ),
                        Quaternion.Euler(90, -90, 0),
                        tiles[i,j].transform
                        ).transform;

                    var sprite = clutterprefab.GetComponent<SpriteRenderer>();
                    if(Random.value < .5f) sprite.flipX = true;

                    sprite.sprite = tiles[i, j].height < 300 ? clutterSprites[Random.Range(0,4)] : clutterSprites[Random.Range(6,9)];
                }

                // determine islandID
                int islandIDup = -1, islandIDleft = -1;
                if(i > 0) islandIDup = tiles[i - 1, j].islandID;
                if(j > 0) islandIDleft = tiles[i, j - 1].islandID;
                
                if (islandIDup != -1)
                {
                    tiles[i, j].islandID = islandIDup;
                    if (islandIDleft != -1 && islandIDleft != islandIDup)
                    {
                        SortedSet<int> x = new SortedSet<int>();
                        
                        foreach (var set in sameIslands)
                        {
                            if (set.Contains(islandIDleft) && !set.Contains(islandIDup))
                            {
                                x.UnionWith(set);
                                set.Clear();
                            }
                        }
                        
                        foreach (var set in sameIslands)
                        {
                            if (set.Contains(islandIDup)) set.UnionWith(x);
                        }
                    }
                    
                }
                else if (islandIDleft != -1)
                {
                    tiles[i, j].islandID = islandIDleft;
                    if (islandIDup != -1 && islandIDleft != islandIDup)
                    {
                        SortedSet<int> x = new SortedSet<int>();
                        
                        foreach (var set in sameIslands)
                        {
                            if (set.Contains(islandIDup) && !set.Contains(islandIDleft))
                            {
                                x.UnionWith(set);
                                set.Clear();
                            }
                        }
                        
                        foreach (var set in sameIslands)
                        {
                            if (set.Contains(islandIDleft)) set.UnionWith(x);
                        }
                    }
                }
                else
                {
                    tiles[i, j].islandID = cid++;
                    sameIslands.Add(new SortedSet<int>());
                    sameIslands[^1].Add(tiles[i,j].islandID);
                }
            }
        }
        
        // determine final islandID and island sizes
        Dictionary<int, List<int>> islandSizeLists = new Dictionary<int, List<int>>();

        foreach (IslandTileScript island in tiles)
        {
            foreach (var i in sameIslands)
            {
                if (i.Contains(island.islandID))
                {
                    island.islandID = i.Min;
                    if (!islandSizeLists.ContainsKey(island.islandID))
                    {
                        islandSizeLists.Add(island.islandID, new List<int>());
                    }
                    islandSizeLists[island.islandID].Add(island.height);
                }
            }
        }

        // find the tallest island
        foreach (var x in islandSizeLists)
        {
            float s = 0;
            foreach (var size in x.Value)
            {
                s += size;
            }

            s /= x.Value.Count;
            islandSizes[x.Key] = s;

            if (s > maxIslandSize) maxIslandSize = s;
        }
    }

    public void CheckClickedIsland(int id)
    {
        if(!transitionController.MapInputEnabled()) return;
        
        if (id == -1) //Debug.Log("WATER!!!");
        {
            audioController.PlayWaterClick();
            return;
        }
        
        if (islandSizes[id] >= maxIslandSize) //Debug.Log("CORRECT! :)");
        {
            if (timed) // TIMED
            {
                timerActive = false;
                timed_score++;
                
                audioController.PlayCorrectClick();
                transitionController.ChangeToScene(SceneManager.GetActiveScene().name);
            }
            else // NOT TIMED
            {
                textController.SetSuccessText(100);

                statsController.AddGuess(online, 100f);
                statsController.AddWonGame(online);
                
                audioController.PlaySuccess();
                transitionController.ShowTitle("SUCCESS");
            }
        }
        else //Debug.Log("INCORRECT! :(");
        {
            foreach (var tile in tiles) // blank out the island since its a wrong guess
            {
                if (tile.islandID == id)
                {
                    tile.transform.GetComponent<MeshRenderer>().enabled = false;
                    tile.islandID = -1;
                }
            }
            
            if (timed) // TIMED
            {
                lives -= tickLength*missPenalty;

                if (lives > 0)
                {
                    audioController.PlayWrongClick();
                    
                    timerBar.localScale = new Vector3(1, lives, 1);
                    timerBar.position = new Vector3(timerBar.position.x, timerBar.position.y, -(10 - lives)/2);
                }
                else
                {
                    if (finishedLevel) return;
                    finishedLevel = true;
                    
                    timerBar.localScale = new Vector3(1, 0, 1);

                    int highScore = StatsControllerScript.GetHighScore(online);
                    if (highScore < timed_score)
                    {
                        audioController.PlaySuccess();
                        statsController.AddHighScore(online, timed_score);
                        transitionController.ShowTitle("HIGH SCORE");
                    }
                    else
                    {
                        audioController.PlayFailure();
                        transitionController.ShowTitle("FAILURE");
                    }
                }
            }
            else // NOT TIMED
            {
                float success = (1 - (maxIslandSize - islandSizes[id]) / maxIslandSize) * 100;
                statsController.AddGuess(online, success);
                textController.SetSuccessText(success);
                
                lives -= 1;
                heartRenderers[Mathf.CeilToInt(lives)].color = heartBlack;
                
                if (lives > 0)
                {
                    audioController.PlayWrongClick();
                    return;
                }
                else
                {
                    audioController.PlayFailure();
                    transitionController.ShowTitle("FAILURE");
                    return;
                }
            }

        }
    }

    private void ButtonBackClick()
    {
        if (!transitionController.UIInputEnabled()) return;
        
        timed_score = 0;
        
        audioController.PlayButtonClick();
        transitionController.ChangeToScene("MenuScreen");
    }
    private void ButtonResetClick()
    {
        if (!transitionController.UIInputEnabled()) return;
        
        timed_score = 0;
        
        audioController.PlayButtonClick();
        transitionController.ChangeToScene(SceneManager.GetActiveScene().name);

    }
}
