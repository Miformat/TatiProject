using UnityEngine;
using System.Collections;

public class ActionScript : MonoBehaviour {

	/*TODO*/
	/*head velocity/gravity*/
	/*jump*/
	/*kick*/

	public enum part
	{
		HEAD,
		TORSO,
		LEGS
	};

	ActionScript head;
	ActionScript torso;
	ActionScript legs;

	public float distance;
	[Header("General")]
	public part ID;
	public bool isActive = false;
	[Header("Torso")]
	public bool isSticky = false;
	//public bool locked = false;

	Vector3 speedVector;
	Vector3 jumpVector;

	float speed = 5;
	float jump = 100;
	float minimalDist = 2;

	public bool isJumping = false;
	public bool isGrounded = false;
	Vector3 startPos;
	Vector3 endPos;
	float jumpDist = 10;
	float direction = 1;
	float hopeHeight = 100;
	float timerThrow = 0;
	
	// Use this for initialization
	void Start () 
	{
		Settings (ID);
		Coloring ();
		speedVector = new Vector3 (speed, 0, 0);
		jumpVector = new Vector3 (0, jump, 0);
		if (ID == part.LEGS){isActive = true;}
		ActionScript[] tabPart = GameObject.FindObjectsOfType<ActionScript>();
		foreach(ActionScript elem in tabPart)
		{
			if (elem.ID == part.HEAD){head = elem;}
			else if (elem.ID == part.TORSO){torso = elem;}
			else if (elem.ID == part.LEGS){legs = elem;}
		}
	}

	void Coloring()
	{
		switch (ID) 
		{
			case part.HEAD:
				this.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
				break;
			case part.LEGS:
				this.gameObject.GetComponent<Renderer>().material.color = Color.magenta;
				break;
			case part.TORSO:
				this.gameObject.GetComponent<Renderer>().material.color = Color.green;
				break;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (isJumping)
		{
			Jumping();
		}
		if (isActive)
		{
			if (ID == part.HEAD){this.GetComponent<Rigidbody>().velocity = Vector3.zero;}
			if (Input.GetKeyDown(KeyCode.Alpha1)){this.Activate(1);}
			if (Input.GetKeyDown(KeyCode.Alpha2)){this.Activate(2);}
			if (Input.GetKeyDown(KeyCode.Alpha3)){this.Activate(3);}
			if (Input.GetButtonDown("Fire1")) 
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					float headDist = Vector3.Distance(this.gameObject.transform.position, head.transform.position);
					float torsoDist = Vector3.Distance(this.gameObject.transform.position, torso.transform.position);
					if (ID == part.LEGS)
					{
						if (headDist < minimalDist)
						{
							Debug.Log("Kick head by legs at : ");
							Vector3 hitpoint = hit.point;
							if (hitpoint.y <= 0) {hitpoint.y = 0.5f;}
							hitpoint.z = 0;
							head.setJump(0,0,5,0,hitpoint);
						}
						if (torsoDist < minimalDist)
						{
							Debug.Log("Kick torso by legs at : ");
							Vector3 hitpoint = hit.point;
							if (hitpoint.y < 0) {hitpoint.y = 0.5f;}
							hitpoint.z = 0;
							torso.setJump(0,0,5,0,hitpoint);
						}
						//Debug.Log(Input.mousePosition);
					}
					if (ID == part.TORSO && headDist < minimalDist)
					{
						Debug.Log("Throw head by torso at : ");
						Vector3 hitpoint = hit.point;
						if (hitpoint.y <= 0) {hitpoint.y = 0.5f;}
						hitpoint.z = 0;
						head.setJump (0, 0, 5, 0, hitpoint);
					}
				}
			}
			//if (Input.GetKeyDown(KeyCode.A)){Previous();}
			//if (Input.GetKeyDown(KeyCode.E)){Next();}
			
			if (Input.GetKeyDown(KeyCode.Z) && ID == part.LEGS && isGrounded)
			{
				setJump(jumpDist, 0, 10, 0, Vector3.zero);
			}
			if (Input.GetKey(KeyCode.Q) && !isJumping)
			{
				//if (locked && direction == -1) {} 
				//else{
					this.transform.position -= speedVector * Time.deltaTime;
				//}
				direction = -1;
			}
			if (Input.GetKeyDown(KeyCode.S) && ID == part.TORSO){isSticky = !isSticky;}
			if (Input.GetKey(KeyCode.D) && !isJumping)
			{
				//if (locked && direction == 1) {} 
				//else {
					this.transform.position += speedVector * Time.deltaTime;
				//}
				direction = 1;
			}
		}
	}

	void setJump(float xOff, float yOff, float HH, float TT, Vector3 EP)
	{
		startPos = this.gameObject.transform.position;
		if (EP == Vector3.zero){endPos = startPos;}
		else{endPos = EP;}
		endPos.x += xOff * direction;
		endPos.y -= yOff;
		hopeHeight = HH;
		timerThrow = TT;
		isGrounded = false;
		isJumping = true;
	}

	void Jumping()
	{
		distance = Vector3.Distance(this.gameObject.transform.position, endPos);
		if (distance > 0.5f)
		{
			float height = Mathf.Sin(Mathf.PI * timerThrow) * hopeHeight;
			this.gameObject.transform.position = Vector3.Lerp(startPos, endPos, timerThrow) + Vector3.up * height; 
			timerThrow += Time.deltaTime /(distance/10f);
		}
		/*else if(!isGrounded)
		{
			setJump(0.25f,0.5f,0,timerThrow, Vector3.zero);
		}*/
		else
		{
			this.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			isJumping = false;
		}
	}

	void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.tag == "Ground")
		{
			isGrounded = true;
			this.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		}
		isJumping = false;
	}

	void Activate(int partID)
	{
		if (partID == 1 && ID != part.HEAD)
		{
			head.isActive = true;
			isActive = false;
		}
		else if (partID == 2 && ID != part.TORSO)
		{
			torso.isActive = true;
			isActive = false;
		}
		else if (partID == 3 && ID != part.LEGS)
		{
			legs.isActive = true;
			isActive = false;
		}
	}
	
	void Settings(part parID)
	{
		if (parID == part.HEAD)
		{
			speed *= 2;
		}
		else if (parID == part.TORSO)
		{
			speed /= 2;
		}
		else if (parID == part.LEGS)
		{
			speed *= 1;
		}
	}

	/*void Next()
	{
		if (ID == part.HEAD)
		{
			torso.isActive = true;
			this.isActive = false;
		}
		else if (ID == part.TORSO)
		{
			legs.isActive = true;
			this.isActive = false;
		}
		else if (ID == part.LEGS)
		{
			head.isActive = true;
			this.isActive = false;
		}
	}*/

	/*void Previous()
	{
		Debug.Log (ID);
		if (ID == part.HEAD){legs.wait = true;legs.waitTime = waitmax;Debug.Log("legs previous");}
		else if (ID == part.TORSO){head.wait = true;head.waitTime = waitmax;Debug.Log("head previous");}
		else if (ID == part.LEGS){torso.wait = true;torso.waitTime = waitmax;Debug.Log("torso previous");}
		isActive = false;
	}*/
}
