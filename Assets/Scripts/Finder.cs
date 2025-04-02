using UnityEngine;

public static class Finder
{
    public static Transform NearestObject(Transform transform, string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        Transform nearestObject = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject obj in taggedObjects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestObject = obj.transform;
            }
        }

        return nearestObject;
    }
}
