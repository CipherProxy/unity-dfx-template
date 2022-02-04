using UnityEngine;
using static Candid;

/// Example Script for using the candid interface on a game object.
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