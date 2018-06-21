using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	int _dmg;
	DamageType _dmgType;
	float _range;

	public GameObject explosion;

	SphereCollider targetCollider;

	MagicEffect _magicEffect;

	private void OnTriggerEnter (Collider collider)
	{
		if (collider == targetCollider)
		{
			Destroy (targetCollider.gameObject);
			Explode ();
		}
	}

	public void Initialize (Vector3 target, float speed, int damage, DamageType damageType, float rangeAOE, MagicEffect magicEffect)
	{
		_dmg = damage;
		_dmgType = damageType;
		_range = rangeAOE;
		_magicEffect = magicEffect;

		//target.y = transform.position.y;
		targetCollider = new GameObject ("Target").AddComponent<SphereCollider> ();
		targetCollider.transform.position = target;
		targetCollider.radius = 0.2f;
		targetCollider.isTrigger = true;
		Vector3 direction = target - transform.position;
		GetComponent<Rigidbody> ().velocity = new Vector3 (direction.x, direction.y, direction.z).normalized * speed;
	}

	void Explode ()
	{
		if (explosion != null) {
			GameObject instance = Instantiate (explosion, transform.position, Quaternion.identity) as GameObject;
			instance.transform.localScale *= _range;
			Destroy (instance, 3f);
		}
		Collider[] colliders = Physics.OverlapSphere (transform.position, _range);
		foreach (Collider collider in colliders)
		{
			if (collider.GetComponent<Character> () != null)
			{
				collider.GetComponent<Character> ().TakeDamage (_dmg, _dmgType);

				// Add modifier with timer to enemies
				if (_magicEffect)
				{
					collider.GetComponent<Character>().AddMagicEffect(_magicEffect);
				}
				
			}
		}

		Destroy (gameObject);
	}
}
