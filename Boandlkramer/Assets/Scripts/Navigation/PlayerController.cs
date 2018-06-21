using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent (typeof (Character))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	Camera cam;
	[SerializeField]
	LayerMask mask;

    // For playing sound effects
    [SerializeField]
    AudioManager audioManager;

    public GameObject indicator;

	NavMeshAgent agent;
	Character character;

	IEnumerator adaptMovement;
	IEnumerator attack;

	[SerializeField]
	float focusDistance = 10f;
	public Enemy focus;

	// range within player collects gold and potions automatically
	public float autoInteractionRange = 1f;

	void Start () {

		if (cam == null)
			cam = Camera.main;

		agent = GetComponent<NavMeshAgent> ();
		character = GetComponent<Character> ();

		adaptMovement = AdaptMovement (0.1f, 3);
	}

	void Update () {

		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			return;

       if (Input.GetMouseButtonDown(0))
       {
           GeneralRaycast();
       }
       if (Input.GetMouseButtonUp(0))
       {
			if (adaptMovement != null)
				StopCoroutine (adaptMovement);
			if (attack != null)
				StopCoroutine (attack);
       }

       if (Input.GetMouseButtonDown(1))
        {
			StopAllCoroutines ();
			agent.SetDestination (transform.position);
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, mask))
            {
				Defocus (focus);
				if (hit.collider.GetComponent<Enemy> () != null) {
					focus = hit.collider.GetComponent<Enemy> ();
					focus.bHighlighted = true;
				}
				character.SecondaryAttack(hit.point, hit.collider.GetComponent<Enemy> ());
            }
        }

	   // lose focus of enemy if outside focus distance

		if (focus != null && Vector3.Distance(transform.position, focus.transform.position) > focusDistance)
           Defocus(focus);

		// check for items that can be picked up automatically
		// get all colliders within the awareness radius
		Collider[] collidingObjects = Physics.OverlapSphere(transform.position, autoInteractionRange);
		foreach (Collider coll in collidingObjects)
		{
			// we found a player within the awareness radius - this object will become the new target
			if (coll.tag == "Pickup")
			{
				Debug.Log("Pickup in range!");
				Pickup pu = coll.GetComponent<Pickup>();
				if (pu.item.bAutoInteract)
				{
					pu.Interact(character);
				}
			}
		}

        // Play footsteps if player is walking
        if (agent.desiredVelocity.magnitude > 0)
        {
            if (!audioManager.IsPlaying("Footsteps"))
            {
                audioManager.Play("Footsteps");
            }
        }
        else
        {
            if (audioManager.IsPlaying("Footsteps"))
            {
                audioManager.Stop("Footsteps");
            }
        }

	}

	void GeneralRaycast () {

		StopAllCoroutines ();

		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 100, mask)) {

			if (hit.collider.GetComponent<Interactable> () != null) {

				StartCoroutine (MoveToInteract (hit.collider.GetComponent<Interactable> ()));
			}
			else if (hit.collider.GetComponent<Enemy> () != null) {

				// defocus current enemy
				Defocus(focus);

				// focus new enemy
				focus = hit.collider.GetComponent<Enemy> ();
				attack = Attack (hit.collider.GetComponent<Enemy> ());

				// highlight focused enemy
				focus.bHighlighted = true;

				StartCoroutine (attack);
			}
			else {

				indicator.transform.position = hit.point;
				indicator.SetActive (true);
				StartCoroutine (adaptMovement);
			}

			agent.SetDestination (hit.point);
		}
	}

	bool MovementRaycast (bool indicate) {

		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		bool success = Physics.Raycast (ray, out hit, 100, mask);

		if (success) {

			if (indicate) {

				indicator.transform.position = hit.point;
				indicator.SetActive (true);
			}
			agent.SetDestination (hit.point);
		}

		return success;
	}

	void Defocus (Enemy enemy) {

		// remove highlighting from enemy
		if (enemy != null)
			enemy.bHighlighted = false;

		focus = null;
	}

	IEnumerator AdaptMovement (float waitTime, int n) {

		bool notDisplayed = false;

		while (true) {

			yield return new WaitForSeconds (waitTime);

			for (int i = 0; i < n; i++) {

				MovementRaycast (false);
				yield return new WaitForSeconds (waitTime);
			}

			notDisplayed = true;
			while (notDisplayed) {

				notDisplayed = !MovementRaycast (true);

				yield return null;
			}
		}
	}

	IEnumerator MoveToInteract<T> (T target) where T : Interactable {

		//while (agent.remainingDistance > target.interactionRange) {
		while (Vector3.Distance (transform.position, target.transform.position) > target.interactionRange) {
			
			yield return null;
		}

		target.Interact(GetComponent<Character>());

		agent.SetDestination (transform.position);
	}

	IEnumerator Attack (Enemy target) {

		while (true) {

			if (target == null)
				break;

			//if (agent.remainingDistance <= 2) { // TODO Attack Range
			if (Vector3.Distance (transform.position, target.transform.position) < 2f) {

				character.Attack (target);
				agent.SetDestination (transform.position);
			}
			else {

				agent.SetDestination (target.transform.position);
			}

			yield return null;
		}
	}
}
