using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;

    [Header("Reference")]
    public LineRenderer lr;
    public Transform guntip, cam, player;
    public LayerMask whatIsGrappleable;
    private PlayerMovmentX pm;
    private Grappling grappling;

    [Header("Swinging")]
    private float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;

    [Header("OaGear")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    [Header("Perdiction")]
    public RaycastHit perdictionHit;
    public float perdictionSphereCastRaduis;
    public Transform perdictionPoint;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovmentX>();
        grappling = GetComponent<Grappling>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(swingKey)) startSwing();
        if (Input.GetKeyUp(swingKey) || pm.grounded == true)stopSwing();
        if (pm.grounded == false || pm.swinging == false ) CheckForSwingPoint();
            
        if (joint != null) OaGearMovment();
        
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void startSwing()
    {
        //return if hit point not found
        if (perdictionHit.point == Vector3.zero) return;
        
        //stop grappling
        if(grappling != null)
        {
            grappling.StopGrapple();
            perdictionPoint.gameObject.SetActive(false);
        }

        pm.ResetRestrictions();

        pm.swinging = true;

        swingPoint = perdictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        //distance between grappling point and gun tip
        joint.maxDistance = distanceFromPoint * 0.4f;
        joint.minDistance = distanceFromPoint * 0.25f;

        //swing value
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = guntip.position;

    }

    void DrawRope()
    {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, guntip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    void CheckForSwingPoint()
    {
        if (joint != null) return;

        RaycastHit SphereCastHit;
        Physics.SphereCast(cam.position, perdictionSphereCastRaduis, cam.forward, out SphereCastHit, maxSwingDistance, whatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, whatIsGrappleable);

        Vector3 realHitPoint;

        //Direct hit
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;

        //Perdiction hit with Sphere Cast
        else if (SphereCastHit.point != Vector3.zero)
            realHitPoint = SphereCastHit.point;

        //miss the shot
        else
            realHitPoint = Vector3.zero;

        // when hit points is found
        if (realHitPoint != Vector3.zero)
        {
            perdictionPoint.gameObject.SetActive(true);
            perdictionPoint.position = realHitPoint;
        }

        //when hit point not found 
        else 
        {
            perdictionPoint.gameObject.SetActive(false);
        }

        perdictionHit = raycastHit.point == Vector3.zero ? SphereCastHit : raycastHit;
    }

    void OaGearMovment()
    {
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    } 

    public void stopSwing()
    {

        pm.swinging = false;

        lr.positionCount = 0;

        Destroy(joint);
    }
}
