using UnityEngine;
using System.Collections;

public class MechanicScript : MonoBehaviour {

	/*TODO*/
	/*platform child movment*/
	/*turning platform*/
	/*end platform*/

	public enum MechaType
	{
		TRIGGER,
		DOOR,
		HPLAT,
		VPLAT,
		TPLAT,
		END
	};

	[Header("General")]
	public MechaType type;
	public int ID;
	public int nbAffected;
	public bool isActive;
	[Header("Platform")]
	public float range;
	public float speed;

	MechanicScript[] affectedMecha;
	Vector3 startPos;
	GameObject actor = null;

	// Use this for initialization
	void Start () 
	{
		startPos = this.gameObject.transform.position;
		Coloring ();
		if (nbAffected != 0)
		{
			affectedMecha = new MechanicScript[nbAffected];
			MechanicScript[] tempMecha = GameObject.FindObjectsOfType<MechanicScript> ();
			int index = 0;
			foreach(MechanicScript current in tempMecha)
			{
				if (current.ID == ID && current != this.gameObject.GetComponent<MechanicScript>())
				{
					affectedMecha[index] = current;
					index++;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Vector3.Distance(this.gameObject.transform.position,startPos) > range){speed = -speed;}
		if (isActive)
		{
			if(type == MechaType.HPLAT){this.gameObject.transform.position += new Vector3(speed,0,0) * Time.deltaTime;}
			if(type == MechaType.VPLAT){this.gameObject.transform.position += new Vector3(0,speed,0) * Time.deltaTime;}
			if(type == MechaType.DOOR)
			{
				this.gameObject.GetComponent<BoxCollider>().enabled = false;
				this.gameObject.GetComponent<MeshRenderer>().enabled = false;
			}
		}
		else
		{
			if(type == MechaType.DOOR)
			{
				this.gameObject.GetComponent<BoxCollider>().enabled = true;
				this.gameObject.GetComponent<MeshRenderer>().enabled = true;
			}
		}
	}

	void Coloring()
	{
		switch(ID)
		{
		case 0:
			this.gameObject.GetComponent<Renderer>().material.color = Color.red;
			break;
		case 1:
			this.gameObject.GetComponent<Renderer>().material.color = Color.blue;
			break;
		case 2:
			this.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
			break;
		case 3:
			this.gameObject.GetComponent<Renderer>().material.color = Color.green;
			break;
		case 4:
			this.gameObject.GetComponent<Renderer>().material.color = Color.cyan;
			break;
		case 5:
			this.gameObject.GetComponent<Renderer>().material.color = Color.magenta;
			break;

		}
	}

	void OnTriggerEnter(Collider other) 
	{
		if (type == MechaType.TRIGGER && other.GetComponent<ActionScript> () && actor == null)
		{
			actor = other.gameObject;
			foreach(MechanicScript current in affectedMecha)
			{
				current.isActive = true;
			}
		}
	}

	void OnTriggerStay(Collider other) 
	{
		if (type == MechaType.TRIGGER && other.GetComponent<ActionScript> () && actor == null)
		{
			actor = other.gameObject;
			foreach(MechanicScript current in affectedMecha)
			{
				current.isActive = true;
			}
		}		
	}

	void OnTriggerExit(Collider other) 
	{
		if (type == MechaType.TRIGGER && other.GetComponent<ActionScript> () && other.gameObject == actor)
		{
			actor = null;
			foreach(MechanicScript current in affectedMecha)
			{
				current.isActive = false;
			}
		}
	}

	void Dading(GameObject dad, GameObject child, bool enter)
	{
		Quaternion rot = Quaternion.identity;//child.transform.rotation;
		Vector3 scale = Vector3.zero;
		if (enter) 
		{
			scale.x = child.transform.localScale.x / dad.transform.localScale.x;
			scale.y = child.transform.localScale.y / dad.transform.localScale.y;
			scale.z = child.transform.localScale.z / dad.transform.localScale.z;
			child.transform.parent = dad.transform;
		}
		else 
		{
			scale.x = child.transform.localScale.x * dad.transform.localScale.x;
			scale.y = child.transform.localScale.y * dad.transform.localScale.y;
			scale.z = child.transform.localScale.z * dad.transform.localScale.z;
			child.transform.parent = null;
		}
		child.transform.rotation = rot;
		child.transform.localScale = scale;
	}

	void OnCollisionEnter(Collision other) 
	{
		if (type == MechaType.HPLAT && other.gameObject.GetComponent<ActionScript> ().ID != ActionScript.part.TORSO) 
		{
			Dading (this.gameObject, other.gameObject, true);
		}
		else if (other.gameObject.GetComponent<ActionScript> ().isSticky && other.gameObject.GetComponent<ActionScript> ().ID == ActionScript.part.TORSO) 
		{
			Debug.Log ("1");
			Dading (this.gameObject, other.gameObject, true);
		}
	}

	void OnCollisionStay(Collision other) 
	{
		if (other.gameObject.GetComponent<ActionScript> ().isSticky && other.gameObject.transform.parent != this.gameObject.transform && other.gameObject.GetComponent<ActionScript> ().ID == ActionScript.part.TORSO) 
		{
			Debug.Log ("2");
			Dading (this.gameObject, other.gameObject, true);
			//other.gameObject.GetComponent<ActionScript> ().locked = false;
		}
		else if (other.gameObject.GetComponent<ActionScript> ().isSticky == false && other.gameObject.transform.parent == this.gameObject.transform && other.gameObject.GetComponent<ActionScript> ().ID == ActionScript.part.TORSO) 
		{
			Debug.Log ("3");
			Dading (this.gameObject, other.gameObject, false);
			//other.gameObject.GetComponent<ActionScript> ().locked = false;
		}
	}

	void OnCollisionExit(Collision other) 
	{
		if (type == MechaType.HPLAT && other.gameObject.GetComponent<ActionScript> ().ID != ActionScript.part.TORSO) 
		{
			Dading (this.gameObject, other.gameObject, false);
		}
		else if (other.gameObject.GetComponent<ActionScript> ().isSticky == false && other.gameObject.transform.parent == this.gameObject.transform && other.gameObject.GetComponent<ActionScript> ().ID == ActionScript.part.TORSO) 
		{
			Debug.Log ("4");
			Dading (this.gameObject, other.gameObject, false);
		}
		else if (other.gameObject.GetComponent<ActionScript> ().isSticky && other.gameObject.transform.parent == this.gameObject.transform && other.gameObject.GetComponent<ActionScript> ().ID == ActionScript.part.TORSO) 
		{
			Debug.Log ("5");
			//other.gameObject.GetComponent<ActionScript> ().locked = true;
		}
	}
}
