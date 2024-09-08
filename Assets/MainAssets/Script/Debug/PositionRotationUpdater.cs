using UnityEngine;
using TMPro;

public class PositionRotationUpdater : MonoBehaviour
{
    public Transform head;
    public Transform rightHand;
    public Transform leftHand;

    public TMP_Text headText;
    public TMP_Text rightHandText;
    public TMP_Text leftHandText;

    // Update is called once per frame
    void Update()
    {
        // Aggiorna la posizione e rotazione della testa
        headText.text = $"Head Position: {head.position.ToString("F2")}, Rotation: {head.rotation.eulerAngles.ToString("F2")}";

        // Aggiorna la posizione e rotazione della mano destra
        rightHandText.text = $"Right Hand Position: {rightHand.position.ToString("F2")}, Rotation: {rightHand.rotation.eulerAngles.ToString("F2")}";

        // Aggiorna la posizione e rotazione della mano sinistra
        leftHandText.text = $"Left Hand Position: {leftHand.position.ToString("F2")}, Rotation: {leftHand.rotation.eulerAngles.ToString("F2")}";
    }
}
