using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerMovement : MonoBehaviour
{
    public float forwardForce = 200f;
    public float sideForce = 100f;
    public float jumpForce = 10000f;
    public List<Transform> tiles = new List<Transform>();
    public float tilesOnScreen = 4;
    private int order = -1;
    private bool jumped = false;
    private int playerScore = -1;
    public float tilesSpace = 10.0f;
    public TextMeshProUGUI scoreText;
    [SerializeField]
    float initalAngle;
    Rigidbody rb;
    public Transform tile;
    // Start is called before the first frame update
    void Start()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        rb = Player.GetComponent<Rigidbody>();
        
        for(int i = 1; i < tilesOnScreen;i++)
        { 
            Vector3 targetPos = transform.position;
            targetPos.z += i* tilesSpace;
            Transform newTile = Instantiate(tile, targetPos, Quaternion.identity) as Transform;
            
            tiles.Add(newTile);
        }
        scoreText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        
        CheckGameOver();
    }
	private void Move()
	{
        if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D))
            rb.AddForce(sideForce * Time.deltaTime,0,0);
        if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A))
            rb.AddForce(-sideForce * Time.deltaTime,0,0);
        if (Input.GetKey(KeyCode.Space) && jumped == false)
		{
            jumped = true;
            Vector3 p = tiles[order].position;
            float gravity = Physics.gravity.magnitude;
            float angle = initalAngle * Mathf.Deg2Rad;
            Vector3 planarTarget = new Vector3(0, 0, p.z);
            Vector3 planarPosition = new Vector3(0, 0, transform.position.z);

            float distance = Vector3.Distance(planarTarget, planarPosition);
            float yOffset = transform.position.y - p.y;
            float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
            Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

            float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPosition);
            Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
            rb.velocity = finalVelocity;
        }
	}
    
	private void OnCollisionEnter(Collision other)
	{
        jumped = false;
        if (other.transform.CompareTag("Tiles"))
		{
            order += 1;
            playerScore += 1;
            scoreText.text = playerScore.ToString("0");
            if ((tiles.Count - order) <tilesOnScreen)
			{
                Vector3 targetPos = tiles[tiles.Count - 1].transform.position;
                targetPos.z += tilesSpace;
                targetPos.x = Random.Range(-5.0f, 5.0f);
                Transform newTile = Instantiate(tile, targetPos, Quaternion.identity) as Transform;
                
                tiles.Add(newTile);
            }
            
		}

    }
    void MoveFoward()
	{
        rb.AddForce(0, 0, forwardForce * Time.deltaTime);
    }
    void CheckGameOver()
	{
        if(transform.position.y < -5)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
}
