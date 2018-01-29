using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	Animator anim; //animator dari player
	Rigidbody2D rigid; //rigidbody 2d dari player
	
	public GameObject GameOverCanvas;
	public GameObject MainmenuCanvas;
	public GameObject PauseCanvas;

	public GameObject StartPosition;

	/*
	public GameObject projectile; //objek peluru
	public Vector2 projectileVelocity = new Vector2 (50,0); //kecepatan peluru
	public Vector2 projectileOffset = new Vector2 (0.75f,-0.104f); //jarak posisi peluru dari player
	*/
	
	public float cooldown = 1f; //jeda waktu untuk menembak	
	bool isCanShoot = true; //memastikan kondisi kapan bisa menembak
	bool isDie = false;
	public bool isAttacking = false;


	public bool isGrounded = false; //untuk menyimpan state apakah karakter berada di ground
	public bool isFacingRight = true; //untuk mengetahui arah hadap dari player
	public float jumpForce = 200f; //besar gaya untuk mengangkat player ke atas
	public float walkForce = 15f; //besar gaya untuk mendorong karakter ke samping
	public float maxSpeed = 1.5f; //kecepatan maksimum dari karakter utama

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody2D>();
		Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isDie)
			InputHandler();
		anim.SetInteger("Speed",(int)rigid.velocity.x);
		if(transform.position.y < -7){
			//SceneManager.LoadScene(0);
			Gameover();
		}
	}

	public void Gameover(){
		GameOverCanvas.SetActive(true);
	}

	public void QuitGame(){
		Application.Quit();
		 UnityEditor.EditorApplication.isPlaying = false;
	}

	public void RestartGame(){
		isDie = false;
		gameObject.transform.position = StartPosition.transform.position;
		MainmenuCanvas.SetActive(false);
		Time.timeScale = 1;
	}

	public void PlayResume(){
		Time.timeScale = 1;
		PauseCanvas.SetActive(false);
		MainmenuCanvas.SetActive(false);
	}

	public void Home(){
		isDie = false;
		gameObject.transform.position = StartPosition.transform.position;
		Time.timeScale = 0;
		MainmenuCanvas.SetActive(true);
	}

	public void Pause(){
		Time.timeScale = 0;
		PauseCanvas.SetActive(true);
	}

	void InputHandler(){
		if(Input.GetKey(KeyCode.LeftArrow)){
			MoveLeft();
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			MoveRight();
		}
		if(Input.GetKeyDown(KeyCode.UpArrow) && isGrounded){
			Jump();
		}
		if(Input.GetKeyDown(KeyCode.DownArrow) && isCanShoot){
			anim.SetTrigger("Attack");
			//Fire();
			StartCoroutine(CanShoot()); 
		}
	}

	/*
	void Fire(){
		//jika player dapat menembak
		if(isCanShoot){
			anim.SetTrigger("Shoot");

			//membuat projectile baru
			GameObject bullet = (GameObject)Instantiate(projectile,(Vector2)transform.position
			+projectileOffset*transform.localScale.x,Quaternion.identity);

			//mengatur kecepatan dari projectile
			Vector2 velocity = new Vector2(projectileVelocity.x*transform.localScale.x,projectileVelocity.y);
			bullet.GetComponent<Rigidbody2D>().velocity = velocity;

			//menyesuaikan scale dari projectile dengan scale karakter
			Vector3 scale = transform.localScale;
			bullet.transform.localScale = scale;
			
		}
	}
	*/
	 
	

	IEnumerator CanShoot(){
		isCanShoot = false;
		yield return new WaitForSeconds (cooldown);
		isCanShoot = true;
	}

	void MoveLeft(){
		if (rigid.velocity.x * -1 < maxSpeed)
			rigid.AddForce(Vector2.left * walkForce);

		//membalik arah karakter apabila menghadap ke arah yang berlawanan dari seharusnya
		if(isFacingRight){
			Flip();
		}
	}

	void MoveRight(){
		if (rigid.velocity.x * 1 < maxSpeed)
			rigid.AddForce(Vector2.right * walkForce);

		//membalik arah karakter apabila menghadap ke arah yang berlawanan dari seharusnya
		if(!isFacingRight){
			Flip();
		}
	} 

	void Jump(){
		rigid.AddForce(Vector2.up * jumpForce);
	}

	void Flip(){
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		isFacingRight = !isFacingRight;
	}

	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.CompareTag("Ground")){
			anim.SetBool("IsGrounded",true);
			isGrounded = true;
		}

		if(col.gameObject.CompareTag("Finish")){
			Gameover();
		}
	}

	//digunakan untuk mengecek apakah Player masih diatas tanah atau tidak
	void OnCollisionStay2D(Collision2D col){
		if(col.gameObject.CompareTag("Ground")){
			anim.SetBool("IsGrounded",true);
			isGrounded = true;
		}
	}

	//digunakan untuk memberi tahu Player bahwa sudah tidak diatas tanah
	void OnCollisionExit2D(Collision2D col){
		if(col.gameObject.CompareTag("Ground")){
			anim.SetBool ("IsGrounded",false);
			isGrounded = false;
		}
	}

	public void PlayerDie(){
		if (!isDie && !isAttacking){
			rigid.velocity = Vector2.zero;
			anim.SetTrigger("Dead");
			isDie = true;
			
		}			
	}
}
