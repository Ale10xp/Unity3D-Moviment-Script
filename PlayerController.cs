using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public Transform lantern;

    private GameObject cam;
    private Rigidbody rb;
    private Transform LookPoint;

    public float speed = 2f;
    public float jumpforce = 10;
    [Range(1,5)] public float runSpeedMultiplier;
    private float startspeed;
    private Vector2 MoveVelocity;


    public CharacterController characterController;
    Vector3 velocity;
    bool isGrounded;

    public float gravity = -10f;
    public float jumpHeight = 5f;

    private float startHeight,actualHeight;
    private float startCenterY, actualCenterY;
    private float startRadius, actualRadius;
    private float turnSmoothVelocity;

    [Range(0,0.2f)]public float turnSmoothTime = 0.1f;
    public Animator anim;

    void Start () {

        cam = GameObject.FindWithTag("MainCamera");
        rb = GetComponent<Rigidbody>();

        LookPoint = GameObject.Find("LookPoint").GetComponent<Transform>();
        startspeed = speed;

        startHeight = characterController.height;
        startCenterY = characterController.center.y;
        startRadius = characterController.radius;
    }
	
	// Update is called once per frame
	void Update () {

        actualHeight = characterController.height;
        actualCenterY = characterController.center.y;
        actualRadius = characterController.radius;

        isGrounded = characterController.isGrounded;

        if (isGrounded) { anim.SetBool("isJumping", false); anim.SetFloat("AirMode 0", 0f); }
        else anim.SetFloat("AirMode 0", 1f);


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 camRotation = new Vector3(0, cam.transform.eulerAngles.y, 0);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        //andando
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camRotation.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);


            transform.rotation = Quaternion.Euler(0, angle, 0);


            anim.SetBool("isWalking", true);




            Vector3 moveDir = Quaternion.Euler(0, targetAngle,0) * Vector3.forward;

        characterController.Move(moveDir.normalized * speed * Time.deltaTime);


        }
        else anim.SetBool("isWalking", false);



        




        /*

            transform.Translate(new Vector3(MoveVelocity.x, 0, MoveVelocity.y), Space.Self);
            */



        //Jump
        if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetTrigger("TakeOff");
            anim.SetBool("isJumping", true);
        }

        //Gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        //Run
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = startspeed * runSpeedMultiplier;

            if (direction != new Vector3(0, 0, 0))
            {
                anim.SetBool("isRunning", true);
            }
            else anim.SetBool("isRunning", false);
        }
        else {

            speed = startspeed;
            anim.SetBool("isRunning", false);
        }

        speed = Mathf.Clamp(speed, speed, startspeed * runSpeedMultiplier);


        //AGACHAR
        if (Input.GetKey(KeyCode.LeftControl))
        {
            anim.SetBool("isCounching", true);

            //diminuir speed
            speed = startspeed / 2;
            speed = Mathf.Clamp(speed, startspeed/2, speed);

            //abaixar altura
            actualHeight = characterController.height / 2;
            actualHeight = Mathf.Clamp(actualHeight, startHeight / 2, startHeight);

            //abaixarCentro
            float centerY = characterController.center.y - startHeight / 4;
            centerY = Mathf.Clamp(centerY, startCenterY - startHeight/4  , startCenterY );
            actualCenterY = centerY;

            //aumentar Radius
            actualRadius = Mathf.Lerp(actualRadius, startRadius*2, 5f*Time.deltaTime);
            actualRadius = Mathf.Clamp(actualRadius, startRadius, startRadius * 2);
            characterController.radius = actualRadius;
        }
        else
        {
            if (!CheckPlafformerUp())
            {
                actualHeight = startHeight;
                actualCenterY = startCenterY;
                actualRadius = startRadius;
                anim.SetBool("isCounching", false);
            }
        }


        characterController.center = new Vector3(characterController.center.x, actualCenterY, characterController.center.z);
        characterController.height = actualHeight;
        characterController.radius = actualRadius;
        // Ligar/Desligar Lanterna
        if (Input.GetKeyDown(KeyCode.L))
        {

            bool stateLantern = lantern.gameObject.activeSelf;

            stateLantern = !stateLantern;

            lantern.gameObject.gameObject.SetActive(stateLantern);

        }


        
        

    }


    public bool CheckPlafformerUp()
    {
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, Vector3.up, out hitInfo, 2f);

        if (hitInfo.collider)
        {

            return true;
        }
        else { return false;  }
        
    }

    public void DoPickUpAnimation()
    {

        anim.SetTrigger("PickUp");

    }




}
