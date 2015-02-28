using UnityEngine;
using System.Collections;

// Simple script that runs in the splash screen
// It will assign master or client to the main script object
public class Bootstrap : MonoBehaviour 
{
	void Start ()
    {
        // Separate the master from the clients (replace later with XBOX and mobile)
        // For now assume we'll run master through editor and clients through standalone
        
#if UNITY_EDITOR

        gameObject.AddComponent<MasterGame>();
                
#elif UNITY_STANDALONE
       
        gameObject.AddComponent<ClientGameManager>();

#endif

        // No need to keep around the bootloader after it's created
        Destroy(this);
    }
}
