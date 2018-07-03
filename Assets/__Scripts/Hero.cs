using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	static public Hero		S;


    [Header("Set in inspector")]
	public float	speed = 30;
	public float	rollMult = -45;
	public float  	pitchMult=30;

	public float gameRestartDelay = 2f; 

	public GameObject projectilePrefab;
	public float projectilespeed = 40;


    [Header("Set dynamically")]
	[SerializeField]
	private float	_shieldLevel = 1; 

	private GameObject lastTriggerGo = null;

	public delegate void WeaponFireDelegate ();
	public WeaponFireDelegate fireDelegate;



    private void Awake()
    {
        if (S == null)
        {
            S = this;
        } else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
		//fireDelegate += TempFire;
    }


	
	// Update is called once per frame
	void Update () {
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;
		
		// rotate the ship to make it feel more dynamic


		transform.rotation =Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult,0);


		//if (Input.GetKeyDown (KeyCode.Space)) {
			//TempFire ();

			if (Input.GetAxis ("Jump") == 1 && fireDelegate != null) {
			fireDelegate ();
			}
		}


	void TempFire (){
		GameObject projGo = Instantiate<GameObject> (projectilePrefab);
		projGo.transform.position = transform.position;
		Rigidbody rigidB = projGo.GetComponent <Rigidbody> ();
		//rigidB.velocity = Vector3.up * projectilespeed;


		Projectile proj = projGo.GetComponent<Projectile> ();
		proj.type = WeaponType.blaster;
		float tSpeed = Main.GetWeaponDefinition (proj.type).velocity;
		rigidB.velocity = Vector3.up * tSpeed;
	}

	


	void OnTriggerEnter(Collider other){
		Transform rootT = other.gameObject.transform.root;
		GameObject go = rootT.gameObject;
		print ("Triggered: " + go.name);


		if (go == lastTriggerGo) {
			return;

		}
		lastTriggerGo = go;

		if (go.tag == "Enemy") {
			shieldLevel--;
			Destroy (go);
		} else {
			print ("Triggered by non-Enemy: "+go.name);
		}
	}

	public float shieldLevel {
		get {
			return (_shieldLevel);
		}
		set {
			_shieldLevel = Mathf.Min (value, 4);
			if (value < 0) {


				Destroy (this.gameObject);

				Main.S.DelayedRestart (gameRestartDelay);

			}
		}
	}
}