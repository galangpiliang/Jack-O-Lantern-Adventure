using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.CompareTag("Player")){
			return;
		}
		if(col.gameObject.CompareTag("Enemy")){
			col.gameObject.SendMessage("TakeDamage",1);
		}
		//Destroy(this.gameObject);
		//Debug.Log("DEstroy");
	}
}
