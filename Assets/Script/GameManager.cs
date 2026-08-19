using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject gameoverUI;
    public int gridSize = 8;
    public float spacing = 1f;
    public GameObject tilePrefab;
    public GameObject player;
    public TextMeshProUGUI hintSymbol;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundText;
    public float flickerInterval = 0.7f;
    public int flickerCount = 4;
    public float baseCountdownTime = 5f;
    public float recoverDelay = 3f;
    public float initialCorrectProb = 0.25f;
    public float decayPerRound = 0.02f;
    public float minCorrectProb = 0.08f;
    public float countdownDecreasePerRound = 0.3f;
    public float minCountdownTime = 2f;

    private Tile[,] tiles;
    private Tile.SymbolType currentCorrectSymbol;
    private int currentRound = 0;
    private bool isRoundActive = false;
    private bool isGameOver = false;
    private Tile.SymbolType currentHintSymbol;
    private static readonly Tile.SymbolType[] AllSymbols =
        (Tile.SymbolType[])Enum.GetValues(typeof(Tile.SymbolType));

    void Start()
    {
        Application.targetFrameRate = 60;
        GenerateGrid();
        InitializeRandomSymbols();
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        if (!isGameOver && player.transform.position.y < -2f)
        {
            TriggerGameOver();
            gameoverUI.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Start");
    }

    void GenerateGrid()
    {
        tiles = new Tile[gridSize, gridSize];
        float offset = (gridSize - 1) * spacing / 2f;
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 pos = new Vector3(x * spacing - offset, 0, z * spacing - offset);
                GameObject newTile = Instantiate(tilePrefab, pos, Quaternion.identity, this.transform);
                tiles[x, z] = newTile.GetComponent<Tile>();
            }
        }
    }

    void InitializeRandomSymbols()
    {
        foreach (Tile t in tiles)
        {
            if (!t.IsCollapsed())
            {
                t.SetSymbol(GetRandomSymbol());
                t.SetHighlight(false);
            }
        }
        currentHintSymbol = GetRandomSymbol();
        if (hintSymbol != null) hintSymbol.text = GetSymbolString(currentHintSymbol);
    }

    IEnumerator GameLoop()
    {
        while (!isGameOver)
        {
            yield return StartCoroutine(RoundSequence());
        }
    }

    IEnumerator RoundSequence()
    {
        if (isRoundActive || isGameOver) yield break;
        isRoundActive = true;
        currentRound++;

        float correctProb = initialCorrectProb - decayPerRound * (currentRound - 1);
        correctProb = Mathf.Max(minCorrectProb, correctProb);

        currentCorrectSymbol = AllSymbols[UnityEngine.Random.Range(0, AllSymbols.Length)];

        if (roundText != null) roundText.text = "Round " + currentRound;

        yield return StartCoroutine(FlickerRoutine());

        AssignSymbols(correctProb);
        if (hintSymbol != null) hintSymbol.text = GetSymbolString(currentCorrectSymbol);

        float countdownDuration = GetCountdownDuration();
        yield return StartCoroutine(CountdownRoutine(countdownDuration));

        CollapseWrongTiles();

        yield return new WaitForSeconds(recoverDelay);
        RecoverAllTiles();

        isRoundActive = false;
    }

    float GetCountdownDuration()
    {
        float currentProb = initialCorrectProb - decayPerRound * (currentRound - 1);
        if (currentProb > minCorrectProb + 0.001f)
            return baseCountdownTime;

        int roundsAfterMin = currentRound - Mathf.CeilToInt((initialCorrectProb - minCorrectProb) / decayPerRound);
        if (roundsAfterMin < 0) roundsAfterMin = 0;

        float newTime = baseCountdownTime - roundsAfterMin * countdownDecreasePerRound;
        return Mathf.Max(minCountdownTime, newTime);
    }

    IEnumerator FlickerRoutine()
    {
        RandomizeAllTilesRandom();
        currentHintSymbol = GetRandomSymbol();
        if (hintSymbol != null) hintSymbol.text = GetSymbolString(currentHintSymbol);
        yield return new WaitForSeconds(flickerInterval);

        for (int i = 1; i < flickerCount; i++)
        {
            bool isLast = (i == flickerCount - 1);

            foreach (Tile t in tiles)
            {
                if (t.IsCollapsed()) continue;
                Tile.SymbolType newSymbol;
                do { newSymbol = GetRandomSymbol(); }
                while (newSymbol == t.currentSymbol);
                t.SetSymbol(newSymbol);
            }

            Tile.SymbolType newHint;
            do { newHint = GetRandomSymbol(); }
            while (newHint == currentHintSymbol);

            if (isLast)
            {
                while (newHint == currentCorrectSymbol)
                {
                    newHint = GetRandomSymbol();
                    while (newHint == currentHintSymbol)
                        newHint = GetRandomSymbol();
                }
            }

            currentHintSymbol = newHint;
            if (hintSymbol != null) hintSymbol.text = GetSymbolString(currentHintSymbol);
            yield return new WaitForSeconds(flickerInterval);
        }
    }

    Tile.SymbolType GetRandomSymbol()
    {
        return AllSymbols[UnityEngine.Random.Range(0, AllSymbols.Length)];
    }

    void RandomizeAllTilesRandom()
    {
        foreach (Tile t in tiles)
        {
            if (t.IsCollapsed()) continue;
            t.SetSymbol(GetRandomSymbol());
            t.SetHighlight(false);
        }
    }

    void AssignSymbols(float correctProb)
    {
        List<Tile.SymbolType> otherSymbols = new List<Tile.SymbolType>(3);
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (tiles[x, z].IsCollapsed()) continue;

                Tile.SymbolType assignedSymbol;
                if (UnityEngine.Random.value < correctProb)
                {
                    assignedSymbol = currentCorrectSymbol;
                }
                else
                {
                    otherSymbols.Clear();
                    foreach (Tile.SymbolType s in AllSymbols)
                        if (s != currentCorrectSymbol) otherSymbols.Add(s);
                    assignedSymbol = otherSymbols[UnityEngine.Random.Range(0, otherSymbols.Count)];
                }
                tiles[x, z].SetSymbol(assignedSymbol);
                tiles[x, z].SetHighlight(false);
            }
        }
    }

    IEnumerator CountdownRoutine(float duration)
    {
        float remaining = duration;
        int lastShown = -1;
        while (remaining > 0)
        {
            int whole = Mathf.CeilToInt(remaining);
            if (whole != lastShown)
            {
                lastShown = whole;
                if (timerText != null) timerText.text = whole.ToString() + "s";
            }
            remaining -= Time.deltaTime;
            yield return null;
        }
        if (timerText != null) timerText.text = "0s";
    }

    void CollapseWrongTiles()
    {
        foreach (Tile t in tiles)
        {
            if (t.currentSymbol != currentCorrectSymbol && !t.IsCollapsed())
                t.Collapse();
        }
        foreach (Tile t in tiles)
        {
            if (t.currentSymbol == currentCorrectSymbol && !t.IsCollapsed())
                t.SetHighlight(true, Color.green);
        }
    }

    void RecoverAllTiles()
    {
        foreach (Tile t in tiles)
        {
            if (t.IsCollapsed()) t.Recover();
        }
        foreach (Tile t in tiles)
        {
            t.SetHighlight(false);
        }
    }

    void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        isRoundActive = false;
        StopAllCoroutines();
        if (timerText != null) timerText.text = "GAME OVER";
    }

    string GetSymbolString(Tile.SymbolType type)
    {
        switch (type)
        {
            case Tile.SymbolType.Cross: return "×";
            case Tile.SymbolType.Circle: return "●";
            case Tile.SymbolType.Square: return "■";
            case Tile.SymbolType.Triangle: return "▲";
            default: return "?";
        }
    }
}