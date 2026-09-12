using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AvalonShell
{
    // Stage 4: the self-updater layer (minimal proof). On start it checks the
    // live channel and reports status on the scene label. Proves networking +
    // the update-check plumbing in-engine on device.
    public class Updater : MonoBehaviour
    {
        const string ChannelUrl = "https://mcontwitter-glitch.github.io/avalon-3d-viewer/live/content.json";

        IEnumerator Start()
        {
            var label = GameObject.Find("Label");
            SetLabel(label, "STAGE 4 - UPDATER: CHECKING LIVE CHANNEL...");

            var req = UnityWebRequest.Get(ChannelUrl);
            req.timeout = 8;
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                int len = req.downloadHandler.text.Length;
                SetLabel(label, "STAGE 4 - UPDATER: LIVE CHANNEL REACHED (" + len + " BYTES)");
                Debug.Log("[UPDATER] channel reached, " + len + " bytes");
            }
            else
            {
                SetLabel(label, "STAGE 4 - UPDATER: CHANNEL OFFLINE (" + req.result + ")");
                Debug.Log("[UPDATER] channel offline: " + req.result);
            }
        }

        static void SetLabel(GameObject label, string msg)
        {
            if (label == null) return;
            var tm = label.GetComponent<TextMesh>();
            if (tm != null) tm.text = msg;
        }
    }
}
