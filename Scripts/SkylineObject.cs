

using System.Collections.Generic;
using UnityEngine;

public class SkylineObject : MonoBehaviour
{
	[SerializeField, Min(1f)]
	float extents;


    public SkylineObject Next
	{ get; set; }

    [SerializeField]
	FloatRange gapY;

    [System.NonSerialized]
	Stack<SkylineObject> pool;



	#if UNITY_EDITOR
        static List<Stack<SkylineObject>> pools;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ClearPools ()
        {
            if (pools == null)
            {
                pools = new();
            }
            else
            {
                for (int i = 0; i < pools.Count; i++)
                {
                    pools[i].Clear();
                }
            }
        }
    #endif
    public float MaxX => transform.localPosition.x + extents;
	
	public Vector3 PlaceAfter (Vector3 position)
	{
		position.x += extents;
		transform.localPosition = position;
		position.x += extents;
		return position;
	}

	public virtual void Check (Runner runner) { }

    public SkylineObject GetInstance ()
	{
		if (pool == null)
		{
			pool = new();
            #if UNITY_EDITOR
                pools.Add(pool);
            #endif
		}
		if (pool.TryPop(out SkylineObject instance))
		{
			instance.gameObject.SetActive(true);
		}
		else
		{
			instance = Instantiate(this);
			instance.pool = pool;
		}
		return instance;
	}

    public void FillGap (Vector3 position, float gap)
	{
		extents = gap * 0.5f;
		position.x += extents;
		transform.localPosition = position;
	}
    public SkylineObject Recycle ()
	{
		pool.Push(this);
		gameObject.SetActive(false);
		SkylineObject n = Next;
		Next = null;
		return n; 
	}

    public FloatRange GapY => gapY.Shift(transform.localPosition.y);
}