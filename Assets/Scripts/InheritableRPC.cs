using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

/*
 * From http://whydoidoit.com/2012/06/29/inheritable-rpc-calls-for-unity/
 *
 * Lets us use inherited RPCs via reflection :D
 * 
 * Modified to use PUN
 */

[AddComponentMenu("System/Inheritable RPC Handler")]
public class InheritableRPC : MonoBehaviour
{
	public class CachedRoutine
	{
		public MethodInfo routine;
		public MonoBehaviour behaviour;
	}
	
	private Dictionary<string, CachedRoutine> cache = new Dictionary<string, CachedRoutine>();
	
	[RPC]
	void PerformRPCCall(string routineName, string parameters)
	{
		var b = new BinaryFormatter();
		using(var s = new MemoryStream(Convert.FromBase64String(parameters)))
		{
			var p = (object[])b.Deserialize(s);
			
			if(!cache.ContainsKey(routineName))
			{   
                List<CachedRoutine> foundRoutines = GetComponents<MonoBehaviour>()
                     .Select(m => new CachedRoutine { routine = m.GetType().GetMethod(routineName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance), behaviour = m })
                     .Where(r => r.routine != null && r.routine.IsDefined(typeof(RPC), true))
                     .ToList();

                // Only one method per RPC!
                if (foundRoutines.Count == 1)
                    cache[routineName] = foundRoutines[0];
                else
                    Debug.LogError("InheritableRPC Error: did not find exactly one method " + routineName + ", found " + foundRoutines.Count, this);
			}

            if(cache.ContainsKey(routineName))
            {
                CachedRoutine m = cache[routineName];
                if (m.behaviour == null)
                    cache.Remove(routineName);
                else
                    m.routine.Invoke(m.behaviour, p);
            }
		}
	}
}


public static class InheritableRPCExtensions
{	
	public static void RPCEx(this PhotonView view, string routineName, PhotonTargets targets, params object[] parameters)
	{
        using(var m = new MemoryStream())
		{
			var b = new BinaryFormatter();
			b.Serialize(m, parameters);
			m.Flush();
			var s = Convert.ToBase64String(m.GetBuffer());
            view.RPC("PerformRPCCall", targets, routineName, s);
		}
	}
	
	public static void RPCEx(this PhotonView view, string routineName, PhotonPlayer player, params object[] parameters)
	{
		using(var m = new MemoryStream())
		{
			var b = new BinaryFormatter();
			b.Serialize(m, parameters);
			m.Flush();
			var s = Convert.ToBase64String(m.GetBuffer());
			view.RPC("PerformRPCCall", player, routineName, s);
		}
	}
}


