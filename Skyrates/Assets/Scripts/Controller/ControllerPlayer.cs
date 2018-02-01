using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(CharacterController))]
public class ControllerPlayer : MonoBehaviour
{

    public InputKey movementForward;
    public InputKey movementStrafe;
    public InputKey interact;
    public float speed = 5;
    public Transform spawn;

    public ControllerCamera cameraController;
    public ControllerMesh mesh;

    private Rigidbody physics;
    //private CharacterController charControls;

    public PlayerData data;
    private Coroutine routinePassiveDrain;

    private void Start()
    {
        this.physics = this.GetComponent<Rigidbody>();
        //this.charControls = this.GetComponent<CharacterController>();

        this.routinePassiveDrain = StartCoroutine(this.data.PassiveDrainHealth());
    }

    private void Update()
    {

        this.data.centerOfMass = this.transform.position + Vector3.up * 0.25f;

        if (!this.data.isFrozen())
        {
            Vector3 playerForward = this.transform.forward;
            Vector3 playerStrafe = this.transform.right;
            Vector3 cameraForward = this.cameraController.transform.forward.Flatten(Vector3.up);
            Vector3 cameraStrafe = this.cameraController.transform.right.Flatten(Vector3.up);
            Vector3 meshForward = this.mesh.transform.forward;
            Vector3 meshStrafe = this.mesh.transform.right;

            Vector3 movementForward = cameraForward * this.movementForward.getAxis();
            Vector3 movementStrafe = cameraStrafe * this.movementStrafe.getAxis();
            Vector3 movement = movementForward + movementStrafe;

            //this.physics.AddForce(movement, ForceMode.Acceleration);
            this.physics.velocity = movement.normalized * this.speed;
            //this.charControls.Move(movement);

            if (movement.sqrMagnitude > 0)
            {
                Vector3 directionMovement = this.transform.position;
                //directionMovement += cameraForward;
                directionMovement += movement.normalized;
                this.mesh.transform.LookAt(directionMovement);

                // TODO: Fix animation rotation of mesh on flip
                //float step = 2.0f * Time.deltaTime;
                //Vector3 newDir = Vector3.RotateTowards(transform.forward, movement.normalized, step, 0.0F);
                //Debug.DrawRay(transform.position, newDir, Color.red);
                //this.mesh.transform.rotation = Quaternion.LookRotation(newDir);
            }

        }


        if (this.interact.onDown())
        {
            if (!this.data.isFrozen() && this.data.hasInteractable() && (!this.data.nearestInteractable.defrosted || this.data.points >= this.data.pointsMax))
            {
                //Debug.Log("Interact with " + this.data.nearestInteractable.dialogue.npcName);
                this.data.StartDialogue();
            }
            else if (this.data.inDialogue)
            {
                this.data.DoNextMessageOrState();
            }
        }

        if (!this.data.inDialogue)
        {
            if (this.data.health <= 0)
            {
                this.data.OnGameEvent(PlayerData.GameEvent.END);
                this.transform.position = this.spawn.position;
                this.transform.rotation = this.spawn.rotation;
            }

            if (this.data.npcDialogue.Count == 0)
            {
                this.data.OnGameEvent(PlayerData.GameEvent.END);
            }
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.data.OnGameEvent(PlayerData.GameEvent.START);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= this.OnSceneLoaded;
    }

}
