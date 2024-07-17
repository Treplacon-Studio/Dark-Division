
using Photon.Pun;
using UnityEngine;

public class PhotonAnimating : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        else
            animator.Play((int)stream.ReceiveNext());
    }
}
