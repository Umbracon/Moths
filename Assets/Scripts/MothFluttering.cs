using System.Collections;
using UnityEngine;

public class MothFluttering : MonoBehaviour {
    SkinnedMeshRenderer smr;
    public bool shouldFlicker = true;
    public float delay = 0.05f;
    void Awake() {
        smr = GetComponent<SkinnedMeshRenderer>();
        StartCoroutine(FlickerWings());
    }

    IEnumerator FlickerWings() {
        while (shouldFlicker) {
            smr.SetBlendShapeWeight(0, Random.Range(0f, 100f));
            yield return new WaitForSeconds(delay);
        }
    }
}