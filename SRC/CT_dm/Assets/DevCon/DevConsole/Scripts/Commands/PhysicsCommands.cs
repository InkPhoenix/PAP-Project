using UnityEngine;
using System.Collections.Generic; //test func
using UnityEditor;                //test func

namespace DevCon.Commands
{
    public class PhysicsCommands
    {
        [ConsoleMethod("Physics.gravity", "Changes the gravity value"), UnityEngine.Scripting.Preserve]
        public static void SetGravity(float grav)
        {
            Physics.gravity = new Vector3(Physics.gravity.x, grav, Physics.gravity.z);
        }

        [ConsoleMethod("Physics.gravity", "Returns the current gravity value"), UnityEngine.Scripting.Preserve]
        public static float GetGravity()
        {
            return Physics.gravity.y;
        }

        [ConsoleMethod("testFunc", "runs a test function"), UnityEngine.Scripting.Preserve]
        public static void testFunc()
        {
            int loadedObjCountTotal = 0;
            string result = " || Objects:     (click here)\n\n";

            List<GameObject> objectsInScene = new List<GameObject>();
            foreach (GameObject obj in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave) { continue; }
                //if (!EditorUtility.IsPersistent(obj.transform.root.gameObject)) { continue; }
                objectsInScene.Add(obj);

                loadedObjCountTotal++;
                result += obj.name.ToString() + "\n";
            }
            Debug.Log("Total objects loaded: " + loadedObjCountTotal + result);
        }
    }
}
