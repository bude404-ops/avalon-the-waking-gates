using UnityEngine;

namespace AvalonShell
{
    // Stage 2 scene life proof: if the run loop is pumping, this turns.
    public class Spin : MonoBehaviour
    {
        void Update()
        {
            transform.Rotate(0f, 40f * Time.deltaTime, 0f, Space.Self);
        }
    }
}
