using UnityEngine;
using System.Collections;

public class Bootstrap : MonoBehaviour 
{
	void Start ()
    {
        // Separate the master from the clients (replace later with XBOX and mobile)
        // For now assume we'll run master through editor and clients through standalone
        
#if UNITY_EDITOR

        gameObject.AddComponent<MasterGameManager>();
                
#elif UNITY_STANDALONE
       
        gameObject.AddComponent<ClientGameManager>();

#endif

        // No need to keep around the bootloader after it's created
        Destroy(this);
    }
}
