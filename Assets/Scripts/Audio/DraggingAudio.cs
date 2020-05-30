using UnityEngine;

// Play dragging audio once dragging started
public class DraggingAudio : MonoBehaviour {
    Vector3 startPosition;
    Vector3 lastPosition;
    bool played;

    void Update() {

        // Save mouse start position
        if (Input.GetMouseButtonDown(0)) {
            startPosition = Input.mousePosition;
        }

        // Dragging is started
        if (Input.GetMouseButton(0)) {
            if (!played) {
                Vector2 dragLine = startPosition - Input.mousePosition;
                if (dragLine.magnitude > 20) {
                    played = true;
                    AudioManager.instance.Play("Drag");
                }
            }
        }

        // Cleanup variables on dragging finished
        if (Input.GetMouseButtonUp(0)) {
            startPosition = Vector3.zero;
            played = false;
            // Stop audio delayed
            Invoke("StopAudio", 0.2f);
        }
    }

    private void StopAudio() {
        AudioManager.instance.Stop("Drag");
    }
}
