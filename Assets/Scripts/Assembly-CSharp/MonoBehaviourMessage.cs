using UnityEngine;

public class MonoBehaviourMessage : MonoBehaviour
{
	public void Awake()
	{
		MonoBehaviour.print("Awake : " + base.name);
	}

	public void Start()
	{
		MonoBehaviour.print("Start : " + base.name);
	}

	public void OnEnable()
	{
		MonoBehaviour.print("OnEnable : " + base.name);
	}

	public void OnDisable()
	{
		MonoBehaviour.print("OnDisable : " + base.name);
	}

	public void OnDestroy()
	{
		MonoBehaviour.print("OnDestroy : " + base.name);
	}

	public void OnApplicationFocus(bool isFocus)
	{
		MonoBehaviour.print("OnApplicationFocus : " + base.name);
		MonoBehaviour.print("isFocus : " + isFocus);
	}

	public void OnApplicationPause(bool isPause)
	{
		MonoBehaviour.print("OnApplicationPause : " + base.name);
		MonoBehaviour.print("isPause : " + isPause);
	}

	public void OnApplicationQuit()
	{
		MonoBehaviour.print("OnApplicationQuit : " + base.name);
	}

	public void OnTransformChildrenChanged()
	{
		MonoBehaviour.print("OnTransformChildrenChanged : " + base.name);
	}

	public void OnTransformParentChanged()
	{
		MonoBehaviour.print("OnTransformParentChanged : " + base.name);
	}

	public void OnValidate()
	{
		MonoBehaviour.print("OnValidate : " + base.name);
	}

	public void Reset()
	{
		MonoBehaviour.print("Reset : " + base.name);
	}

	public void OnAnimatorIK()
	{
		MonoBehaviour.print("OnAnimatorIK : " + base.name);
	}

	public void OnAnimatorMove()
	{
		MonoBehaviour.print("OnAnimatorMove : " + base.name);
	}

	public void OnAudioFilterRead(float[] data, int channels)
	{
		MonoBehaviour.print("OnAudioFilterRead : " + base.name);
	}

	public void OnJointBreak()
	{
		MonoBehaviour.print("OnJointBreak : " + base.name);
	}

	public void OnParticleCollision()
	{
		MonoBehaviour.print("OnParticleCollision : " + base.name);
	}

	public void FixedUpdate()
	{
		MonoBehaviour.print("FixedUpdate : " + base.name);
	}

	public void Update()
	{
		MonoBehaviour.print("Update : " + base.name);
	}

	public void LateUpdate()
	{
		MonoBehaviour.print("LateUpdate : " + base.name);
	}

	public void OnConnectedToServer()
	{
		MonoBehaviour.print("OnConnectedToServer : " + base.name);
	}

	public void OnDisconnectedFromServer()
	{
		MonoBehaviour.print("OnDisconnectedFromServer : " + base.name);
	}

	public void OnFailedToConnect()
	{
		MonoBehaviour.print("OnFailedToConnect : " + base.name);
	}

	public void OnFailedToConnectToMasterServer()
	{
		MonoBehaviour.print("OnFailedToConnectToMasterServer : " + base.name);
	}

	public void OnMasterServerEvent()
	{
		MonoBehaviour.print("OnMasterServerEvent : " + base.name);
	}

	public void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		MonoBehaviour.print("OnNetworkInstantiate : " + base.name);
		MonoBehaviour.print("NetworkMessageInfo : " + info);
	}

	public void OnPlayerConnected()
	{
		MonoBehaviour.print("OnPlayerConnected : " + base.name);
	}

	public void OnPlayerDisconnected()
	{
		MonoBehaviour.print("OnPlayerDisconnected : " + base.name);
	}

	public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		MonoBehaviour.print("OnSerializeNetworkView : " + base.name);
		MonoBehaviour.print("BitStream : " + stream);
		MonoBehaviour.print("NetworkMessageInfo : " + info);
	}

	public void OnServerInitialized()
	{
		MonoBehaviour.print("OnServerInitialized : " + base.name);
	}

	public void OnMouseDown()
	{
		MonoBehaviour.print("OnMouseDown : " + base.name);
	}

	public void OnMouseUp()
	{
		MonoBehaviour.print("OnMouseUp : " + base.name);
	}

	public void OnMouseUpAsButton()
	{
		MonoBehaviour.print("OnMouseUpAsButton : " + base.name);
	}

	public void OnMouseDrag()
	{
		MonoBehaviour.print("OnMouseDrag : " + base.name);
	}

	public void OnMouseEnter()
	{
		MonoBehaviour.print("OnMouseEnter : " + base.name);
	}

	public void OnMouseExit()
	{
		MonoBehaviour.print("OnMouseExit : " + base.name);
	}

	public void OnMouseOver()
	{
		MonoBehaviour.print("OnMouseOver : " + base.name);
	}

	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		MonoBehaviour.print("OnControllerColliderHit : " + hit);
	}

	public void OnTriggerEnter(Collider col)
	{
		MonoBehaviour.print("OnTriggerEnter : " + col);
	}

	public void OnTriggerExit(Collider col)
	{
		MonoBehaviour.print("OnTriggerExit : " + col);
	}

	public void OnTriggerStay(Collider col)
	{
		MonoBehaviour.print("OnTriggerStay : " + col);
	}

	public void OnCollisionEnter(Collision col)
	{
		MonoBehaviour.print("OnCollisionEnter : " + col.gameObject);
	}

	public void OnCollisionExit(Collision col)
	{
		MonoBehaviour.print("OnCollisionExit : " + col.gameObject);
	}

	public void OnCollisionStay(Collision col)
	{
		MonoBehaviour.print("OnCollisionStay : " + col.gameObject);
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		MonoBehaviour.print("OnTriggerEnter2D : " + col);
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		MonoBehaviour.print("OnTriggerExit2D : " + col);
	}

	public void OnTriggerStay2D(Collider2D col)
	{
		MonoBehaviour.print("OnTriggerStay2D : " + col);
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		MonoBehaviour.print("OnCollisionEnter2D : " + col.gameObject);
	}

	public void OnCollisionExit2D(Collision2D col)
	{
		MonoBehaviour.print("OnCollisionExit2D : " + col.gameObject);
	}

	public void OnCollisionStay2D(Collision2D col)
	{
		MonoBehaviour.print("OnCollisionStay2D : " + col.gameObject);
	}

	public void OnPreCull()
	{
		MonoBehaviour.print("OnPreCull : " + base.name);
	}

	public void OnBecameVisible()
	{
		MonoBehaviour.print("OnBecameVisible : " + base.name);
	}

	public void OnBecameInvisible()
	{
		MonoBehaviour.print("OnBecameInvisible : " + base.name);
	}

	public void OnWillRenderObject()
	{
		MonoBehaviour.print("OnWillRenderObject : " + base.name);
	}

	public void OnPreRender()
	{
		MonoBehaviour.print("OnPreRender : " + base.name);
	}

	public void OnRenderObject()
	{
		MonoBehaviour.print("OnRenderObject : " + base.name);
	}

	public void OnPostRender()
	{
		MonoBehaviour.print("OnPostRender : " + base.name);
	}

	public void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		MonoBehaviour.print("OnRenderImage : " + base.name);
		MonoBehaviour.print("src : " + src);
		MonoBehaviour.print("dest : " + dest);
	}

	public void OnGUI()
	{
		MonoBehaviour.print("OnGUI : " + base.name);
	}

	public void OnDrawGizmos()
	{
		MonoBehaviour.print("OnDrawGizmos : " + base.name);
	}

	public void OnDrawGizmosSelected()
	{
		MonoBehaviour.print("OnDrawGizmosSelected : " + base.name);
	}
}
