using UnityEngine;

[DisallowMultipleComponent]
public class RPGCameraControl : MonoBehaviour {
    [Header("Target")][Tooltip("Target to look around")]public Transform Target;
    private Transform NowAround;
        [Header("Sensitivity")]
    [Tooltip("The Sensivity of View")] public float  ViewSensitivity;
    [Tooltip("The Sensivity of Zoom")] public float ScrollSensitivity;
        [Header("Zoom")] 
    [Tooltip("Minimum distance of the Camera to Target")] public float MinDistance;
    [Tooltip("Maximum distance of the Camera to Target")] public float MaxDistance;
    [Tooltip("Some Adjustments in View")] public Vector3 Offset;
        [Header("Angle of View")]
    [Tooltip("Minimum Angle of view")] public float MinYAngle = -45;
    [Tooltip("Maximum Angle of view")] public float MaxYAngle = 80;
    
    private float currentX = 0f, currentY = 1.5f, distance = 10f, realMinDistance, realMaxDistance, realScrollSensitivity;


        [Header("Mouse Button")]
    [Tooltip("Enable/Disable Left drag Click")]public bool LeftMouseButtonClickToControl; 
    [Tooltip("Enable/Disable Right drag Click")]public bool RightMouseButtonClickToControl;
    [Header("Details")]
    public bool hideCursor; 
    [Tooltip("Enable/Disable the proportional switches of values by the Medium Scale of the Target")]
    public bool SwitchWithScale;
    [Range(0.001f, 1)]
    [Tooltip("Smooth Modifier")] public float SmoothSpeed = .125f;
    private bool stateCursor;

    public bool CanMoveCamera = true;

    private void Start()
    {
        stateCursor = !hideCursor;

        if (SwitchWithScale)
            ChooseTarget();

        else {
            realMinDistance = MinDistance;
            realMaxDistance = MaxDistance;
            realScrollSensitivity = ScrollSensitivity;
        }

    }
    void Update()
    {
        if (CanMoveCamera)
        {
            MouseControl();
        }

        transform.position = SmoothedPosition();

        transform.LookAt(Target.position + Offset);

        if (Target != NowAround) ChooseTarget();

    }





    private void MouseControl()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel") * realScrollSensitivity;

        distance = Mathf.Clamp(distance, realMinDistance, realMaxDistance);



        if (LeftMouseButtonClickToControl)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                currentX += Input.GetAxis("Mouse X") * ViewSensitivity;
                currentY -= Input.GetAxis("Mouse Y") * ViewSensitivity;

                currentY = Mathf.Clamp(currentY, MinYAngle, MaxYAngle);

            }

            if (hideCursor)
            {



                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    stateCursor = !stateCursor;
                }

                Cursor.visible = stateCursor;

                if (!stateCursor)
                    Cursor.lockState = CursorLockMode.Locked;
            }


        }

        if (RightMouseButtonClickToControl)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                currentX += Input.GetAxis("Mouse X") * ViewSensitivity;
                currentY -= Input.GetAxis("Mouse Y") * ViewSensitivity;

                currentY = Mathf.Clamp(currentY, MinYAngle, MaxYAngle);

            }

            if (hideCursor)
            {



                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    stateCursor = !stateCursor;
                }

                Cursor.visible = stateCursor;

                if (!stateCursor)
                    Cursor.lockState = CursorLockMode.Locked;
            }

        }


        if (RightMouseButtonClickToControl == false && LeftMouseButtonClickToControl == false)
        {

            currentX += Input.GetAxis("Mouse X") * ViewSensitivity;
            currentY -= Input.GetAxis("Mouse Y") * ViewSensitivity;

            currentY = Mathf.Clamp(currentY, MinYAngle, MaxYAngle);

            if (hideCursor)
            {



                if (Input.GetKeyDown(KeyCode.Escape))
                    stateCursor = !stateCursor;
                    

                if (stateCursor==false)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                }

            }

        }

    }


    public Vector3 SmoothedPosition()
    {
        Vector3 dir = new Vector3(Offset.x,Offset.y, -distance + Offset.z);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

         Vector3 desiredPosition = Target.position + (rotation * dir) ;


        Vector3 SmoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);

        return SmoothedPosition;

    }
    




    public void ChooseTarget()
    {
        if (SwitchWithScale)
        {
            float LossyScaleMedium = (Target.lossyScale.x + Target.lossyScale.y + Target.lossyScale.z) / 3f;
            realMinDistance = MinDistance * LossyScaleMedium;
            realMaxDistance = MaxDistance * LossyScaleMedium;
            realScrollSensitivity = ScrollSensitivity * LossyScaleMedium;
        }

        NowAround = GetComponent<RPGCameraControl>().Target;

    }


    public void CheckRayCollison()
    {
        float newDistance = transform.position.magnitude;

        Vector3 dir2target = (Target.position - transform.position);

        Ray ray = new Ray(transform.position, dir2target);

        RaycastHit hit;

        if (Physics.Linecast(transform.position, Target.position, out hit))
        {
            transform.position = SmoothedPosition() + new Vector3(0, 0, -(hit.distance*1.2f));
        }

 

        Debug.DrawRay(transform.position, dir2target, Color.yellow);



    }

}
