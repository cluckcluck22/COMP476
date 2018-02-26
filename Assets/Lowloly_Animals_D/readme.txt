simple AI for wandering.
This is the very first version, in the future it will be further developed.
The basis of AI is made through IEnumerator.
You can set the time for each action here:
		float rotTime = Random.Range (0.1f, 0.5f);
		int rotateWait = Random.Range (0,1);
		int rotateLorR = Random.Range (1, 3);
		float walkWait = Random.Range (waitMin, waitMax);
		float walkTime = Random.Range (walkMin, walkMax);
		float sitTime = Random.Range (sitMin, sitMax);
		float walkRot = Random.Range (0.2f, 1.5f);
		int sleepTime = Random.Range (sleepMin, sleepMax);
In order for animals to react to triggers of sleep and eating, you need to add tags "EatTr" and "SleepTr" in Unity and zones to call these tags. 
For each zone in the layers we set "Ignore raycast".

Restriction of the movement zone is carried out on this line:
if (Vector3.Distance (startpos, transform.position) > wanderRange) {
			//transform.Rotate (transform.up*Time.deltaTime*50); (You can use this line for a smooth rotation, but bugs may occur)
			transform.LookAt (startpos);
Bypassing the obstacles is due to the start of the beam:
Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.SphereCast (ray, 1.0f, out hit)) {
			if (hit.distance < obstacleRange) {
				float ColRotSpeed = Random.Range (100f,200f);
				transform.Rotate (transform.up * Time.deltaTime * ColRotSpeed);
			}
		}

Full code:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimalAI_lite_b : MonoBehaviour {
	public float moveSpeed = 2f;
	public float rotSpeed = 100f;
	public float obstacleRange = 5.0f;
	public float wanderRange = 10f;
	public GameObject animal;
	[Header("Wait Time")]
	public float waitMin = 1f;
	public float waitMax = 2f;
	[Header("Walk Time")]
	public float walkMin = 1f;
	public float walkMax = 2f;
	[Header("Sit Time")]
	public float sitMin = 1f;
	public float sitMax = 2f;
	[Header("Sleep Time")]
	public int sleepMin = 1;
	public int sleepMax = 2;
	[Header("Eat Time")]
	public int eatTime = 2;

	private bool isWandering = false;
	private bool isRotateLeft = false;
	private bool isRotateRight = false;
	private bool isWalking = false;
	private bool Sit = false;
	private bool WalkRotate = false;
	private bool sleep = false;
	private bool eating = false;
	private float angle_rotation;
	private IEnumerator coroutine;

	private Animator anim;
	private Vector3 startpos;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		startpos = transform.position;
	}
	// Update is called once per frame
	void Update () 
	{           
		
		if (isWandering == false) 
		{
			StartCoroutine (Wander ());	
		}
		// walking range
		if (Vector3.Distance (startpos, transform.position) > wanderRange) {
			//transform.Rotate (transform.up*Time.deltaTime*50);
			transform.LookAt (startpos);
		}
		// rotate
		if (isRotateRight == true) 
		{
			transform.Rotate (transform.up * Time.deltaTime * rotSpeed);
		}
		if (isRotateLeft == true) 
		{
			transform.Rotate (transform.up * Time.deltaTime * -rotSpeed);
		}
		// avoidance of obstacles
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.SphereCast (ray, 1.0f, out hit)) {
			if (hit.distance < obstacleRange) {
				float ColRotSpeed = Random.Range (100f,200f);
				transform.Rotate (transform.up * Time.deltaTime * ColRotSpeed);
			}
		}
		// idle
		if (isRotateRight == false & isRotateLeft == false & isWalking == false & Sit == false & sleep == false & eating == false) {
			anim.SetBool ("IsIdle", true);
			anim.SetBool ("IsEat", false);
			anim.SetBool ("IsWalk", false);
			anim.SetBool ("IsSleep", false);
			anim.SetBool ("IsSit", false);
		}
		// Sitting or idle_2
		else if (Sit == true & isWalking == false) {
			anim.SetBool ("IsWalk", false);
			anim.SetBool ("IsIdle", false);
			anim.SetBool ("IsSleep", false);
			anim.SetBool ("IsSit", true);
			anim.SetBool ("IsEat", false);
		}
		// Sleeping
		if (sleep == true) {
			isWalking = false;
			WalkRotate = false;
			isRotateRight = false;
			isRotateLeft = false;
			anim.SetBool ("IsSleep", true);
			anim.SetBool ("IsWalk", false);
			anim.SetBool ("IsIdle", false);
			anim.SetBool ("IsSit", false);
			anim.SetBool ("IsEat", false);
		}
		// Eating
		if (eating == true) {
			isWalking = false;
			WalkRotate = false;
			isRotateRight = false;
			isRotateLeft = false;
			anim.SetBool ("IsEat", true);
			anim.SetBool ("IsSleep", false);
			anim.SetBool ("IsWalk", false);
			anim.SetBool ("IsIdle", false);
			anim.SetBool ("IsSit", false);
		}
		// walking
		if (isWalking == true && sleep == false) {
			transform.Translate (0,0,moveSpeed*Time.deltaTime);
			anim.SetBool ("IsWalk", true);
			anim.SetBool ("IsIdle", false);
			anim.SetBool ("IsSleep", false);
			anim.SetBool ("IsSit", false);
			anim.SetBool ("IsEat", false);
			}
		if (WalkRotate == true) {
			transform.Rotate (transform.up * Time.deltaTime * rotSpeed);
		}
	}
	IEnumerator Wander ()
	{
		float rotTime = Random.Range (0.1f, 0.5f);
		int rotateWait = Random.Range (0,1);
		int rotateLorR = Random.Range (1, 3);
		float walkWait = Random.Range (waitMin, waitMax);
		float walkTime = Random.Range (walkMin, walkMax);
		float sitTime = Random.Range (sitMin, sitMax);
		float walkRot = Random.Range (0.2f, 1.5f);
		int sleepTime = Random.Range (sleepMin, sleepMax);


		isWandering = true;

		// sit Time
		if (walkWait > 1) {
			Sit = true;
			yield return new WaitForSeconds (sitTime);
			Sit = false;
		}
		// sleep Time
		if (sleep == true) {
			isWalking = false;
			yield return new WaitForSeconds (sleepTime);
			sleep = false;
		}
		// eat Time
		if (eating == true) {
			isWalking = false;
			yield return new WaitForSeconds (eatTime);
			eating = false;
		}
		yield return new WaitForSeconds (walkWait);
		// walk time
		yield return new WaitForSeconds (walkTime);
		isWalking = true;
		yield return new WaitForSeconds (walkTime);
		// walk rotate
		if (walkTime >= 3) {
			WalkRotate = true;
			isWalking = true;
			if (rotateLorR == 1) 
			{
				isRotateRight = true;
				yield return new WaitForSeconds (walkRot);
				isRotateRight = false;
			}
			if (rotateLorR == 2) 
			{
				isRotateLeft = true;
				yield return new WaitForSeconds (walkRot);
				isRotateLeft = false;
			}
			WalkRotate = false;
		}
		yield return new WaitForSeconds (walkTime);
		isWalking = false;
		// stay rotate
		yield return new WaitForSeconds (rotateWait);
		if (rotateLorR == 1) 
		{
			isRotateRight = true;
			yield return new WaitForSeconds (rotTime);
			isRotateRight = false;
		}
		if (rotateLorR == 2) 
		{
			isRotateLeft = true;
			yield return new WaitForSeconds (rotTime);
			isRotateLeft = false;
		}
		isWandering = false;
	}
	// sleep and eat trigger
	void OnTriggerEnter(Collider animal){
		if (animal.CompareTag ("SleepTr")) {
			sleep = true;
			isWalking = false;
		}
		if (animal.CompareTag ("EatTr")) {
			eating = true;
			isWalking = false;
		}
	}
}
