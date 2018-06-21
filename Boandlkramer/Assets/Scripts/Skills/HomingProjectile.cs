using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour {

	GameObject _target_obj;
	int _dmg;
	DamageType _dmgType;
	float _speed;

	public GameObject impact;

	MagicEffect _magicEffect;

	Collider targetCollider;

	IEnumerator move;

	private void OnTriggerEnter (Collider collider) {
		if (collider == targetCollider) {
			Explode ();
		}
	}

	public void Initialize (GameObject target, float speed, int damage, DamageType dmgType, MagicEffect magicEffect) {

		_dmg = damage;
		_dmgType = dmgType;
		_speed = speed;
		_target_obj = target;
		_magicEffect = magicEffect;
		targetCollider = target.GetComponent<Collider> ();
		StartCoroutine (Move ());
	}

	void Explode () {
		if (impact != null) {
			GameObject instance = Instantiate (impact, transform.position, Quaternion.identity) as GameObject;
			Destroy (instance, 3f);
		}
		_target_obj.GetComponent<Character> ().TakeDamage (_dmg, _dmgType);

		// Add modifier with timer to enemies
		if (_magicEffect)
		{
			targetCollider.GetComponent<Character>().AddMagicEffect(_magicEffect);
		}

		Destroy (gameObject);
	}

	IEnumerator Move () {
		while (true) {
			if (_target_obj == null)
				break;
			Vector3 direction = _target_obj.transform.position - transform.position;
			GetComponent<Rigidbody> ().velocity = new Vector3 (direction.x, direction.y, direction.z).normalized * _speed;
			yield return 0;
		}
	}
}
