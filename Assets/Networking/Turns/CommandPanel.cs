﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CommandPanel : MonoBehaviour {

    private string playerName;
    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value.Trim(); }
    }

    private bool connected = false;
    public bool IsConnected
    {
        get { return connected; }
    }

    [SerializeField]
    private float timerMax = 10.0f;
    private float timerCurrent = 0.0f;

    // the chosen character for this controller. null indicates no character selected yet
    [SerializeField]
    private GameObject character = null;
    public GameObject Character
    {
        get { return character; }
        set { character = value; }
    }

    private bool moveSelectEnabled;
		public bool MoveSelectEnabled
	{
		get {return moveSelectEnabled; }
		set {moveSelectEnabled = value;}
	}

	[SerializeField]
	private GameObject unit;
	private CharacterBehavior unitBehavior;
	public GameObject SelectedUnit
	{
		get {return unit; }
		set
        {
            unit = value;
			Reset();
        }
	}

	[SerializeField]
	private float distanceBetweenTiles = 1f;
	
	private List<Vector3> queuedPath = new List<Vector3>();

	// Use this for initialization
	void Start () {

        //this.SelectedUnit = FindObjectOfType<CharacterBehavior> ().gameObject;
        //this.SelectedUnit = unit;
        MoveSelectEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
        TickDown();
	}

    // call this on the GET heartbeat rule
    public void Pulse()
    {
        connected = true;
        timerCurrent = timerMax;
        Debug.Log(PlayerName + " has a pulse!");
    }

    void TickDown()
    {
        if (timerCurrent >= 0)
        {
            timerCurrent -= Time.deltaTime;
            if (timerCurrent < 0)
            {
                connected = false;
                try
                {
                    SendMessageUpwards("OnPlayerHasDisconnected", playerName, SendMessageOptions.RequireReceiver);
                }
                catch
                {
                    Debug.Log("CATASTROPHIC ERROR: Server missing???");
                }
                Debug.Log(playerName + " has disconnected");
            }
        }
    }

	void Reset()
    {
        if (unit != null)
        {
            unitBehavior = unit.GetComponent<CharacterBehavior>();
            //		movesLeft.text = (unitBehavior.getMoveStat ()).ToString();
            queuedPath.Clear();
        }
	}

    // TODO: Add functionality!
    public void SpawnPlayerCharacter()
    {

    }
    // This will need to be reworked to accomodate for non 1x1 tiles (we're using 6x6 now but I'd be a fool to hardcode it again)
	public void PathSelection(TileBehavior tile){
		Vector3 tilePos = tile.transform.position;
        if(unit != null && tile.IsPathable)
		if (moveSelectEnabled) {
			/*
		if (currentPos > unit.GetComponent<CharacterBehavior> ().getMoveStat ()) {
			return;
		}
		*/
			// if the list is empty, set the start at where you're at, idiot
			if(queuedPath.Count == 0)
				queuedPath.Add(new Vector3(unit.transform.position.x, 0f, unit.transform.position.z));
			int currentPos = queuedPath.Count;
			// checks the distance between the selected tiles 
			float dx = Mathf.Abs(queuedPath [currentPos - 1].x - tilePos.x);
			float dz = Mathf.Abs(queuedPath [currentPos - 1].z - tilePos.z);
			//if you click on an already-existant tile, remove everything past there
			if (queuedPath.Contains (tilePos)) {
				int index = queuedPath.IndexOf (tilePos);
				for (int i = queuedPath.Count-1; i > index; i--) {
					Debug.Log ("Removing: " + queuedPath[i].x + "," + queuedPath[i].z);
					queuedPath.RemoveAt (i);
				}
			} else if ((((dx == distanceBetweenTiles && dz == 0f) || (dx == 0f && dz == distanceBetweenTiles)) 
			            && (dx + dz <= distanceBetweenTiles))
			            && queuedPath.Count-1 < unitBehavior.MoveStat){
				queuedPath.Add (tilePos); 
			}

			foreach (Vector3 v in queuedPath) {
				Debug.Log ("Next Step: " + v.x + "," + v.z);
			}

			UpdateMovesLeft();
		}
	}

	public void CommitToMove(){

		Movement move = new Movement (unit, queuedPath, true);
		StartCoroutine(Execute (move));
	}

	void UpdateMovesLeft(){
//		movesLeft.text = (unitBehavior.getMoveStat() - (queuedPath.Count-1)).ToString();
	}

	public void TargetAttack(GameObject target){
		float dx = Mathf.Abs(unit.transform.position.x - target.transform.position.x);
		float dz = Mathf.Abs(unit.transform.position.z - target.transform.position.z);
		Debug.Log ("Targeting: " + dx + " + " + dz);
		Vector3 distanceBetween = target.transform.position - unit.transform.position;
		RaycastHit hit = new RaycastHit();
		if ((dx + dz) <= 7.0f && 
		    Physics.Raycast(unit.transform.position, (distanceBetween).normalized, out hit, distanceBetween.magnitude)) {
			if(hit.collider.gameObject.Equals(target)){
				Debug.DrawLine(unit.transform.position,target.transform.position,Color.red);
				Debug.Log ("HIT!");
				CommitToAttack(target);
			}
			else{
				CommitToAttack(hit.collider.gameObject);
			}
		}
	}

	public void CommitToAttack(GameObject target){
		Attack attack = new Attack (unit, target, true);
		StartCoroutine(Execute (attack));
	}

	public IEnumerator Execute(Action action){
		Debug.Log ("Executing...");
		FindObjectOfType<FieldReporter> ().addActionToTurn(action);
		yield return StartCoroutine(action.Execute());
		Reset ();
		FindObjectOfType<FieldReporter> ().checkToIncrememt ();
	}


}
