﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ForceLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ForceReady()
    {
        GameStateManager.Instance.GameState = GameState.GameBegin;
    }
}
