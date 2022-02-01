using UnityEngine;
using static Candid;
/// <summary>
/// Class with a JS Plugin functions for WebGL.
/// </summary>

public class DemoScript : MonoBehaviour
{
    private void Start()
    {
        Candid.Init();

        Candid.Insert("demo_key", "demo_description", "demo_model", (response, error) =>
        {
            Debug.Log(response + "\n" + error);
            Candid.Lookup("demo_key", (response, error) =>
            {
                Debug.Log(response + "\n" + error);
            });
        });
    }
}