using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : Singleton<Background> {
    public Material[] materials;

    private new MeshRenderer renderer;

    void Start() {
        renderer = GetComponent<MeshRenderer>();
        renderer.material = materials[GameController.Instance.currentMaterialsIndex];
    }

    void Update() {
        
    }

    public IEnumerator ChangeColor(float duration) {
        var materialToChange = renderer.material;
        var endValue = materials[GameController.Instance.currentMaterialsIndex].color;

        float time = 0;
        var startValue = materialToChange.color;
        while (time < duration) {
            materialToChange.color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        materialToChange.color = endValue;
    }
}
