using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    //given by enemyController
    [HideInInspector]
    public bool walking;

    public GameObject enemyMesh;

    //default set to 0.1f
    public float walkAnimationSpeed;
    public float idleAnimationSpeed;

    public Mesh[] walkingFrames = new Mesh[8];
    public Mesh[] idleFrames = new Mesh[4];

    private void Start()
    {
        StartCoroutine(Animation());
    }
    private IEnumerator Animation()
    {
        int frame = 0;
        while (true)
        {
            if (walking)
            {
                //When frame reaches end of list, start back at beginning of list
                //placed at start because if animation goes from walking to idle, walking has more frames so index will be out of range
                if (frame >= walkingFrames.Length - 1)
                {
                    frame = 0;
                }

                //Select next walking frame(Mesh) from list
                enemyMesh.GetComponent<MeshFilter>().mesh = walkingFrames[frame];
                yield return new WaitForSeconds(walkAnimationSpeed);
                frame++;
            }
            //if not walking, swinging, or jumping, play idle animation
            else
            {
                //When frame reaches end of list, start back at beginning of list
                //placed at start because if animation goes from walking to idle, walking has more frames so index will be out of range
                if (frame >= idleFrames.Length - 1)
                {
                    frame = 0;
                }
                //Select next walking frame(Mesh) from list
                enemyMesh.GetComponent<MeshFilter>().mesh = idleFrames[frame];
                yield return new WaitForSeconds(idleAnimationSpeed);
                frame++;
            }

            yield return null;
        }
    }
}
