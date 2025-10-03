using UnityEngine;

public static class VectorUtils
{
    public static Vector3 GetDirection(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }
    
    public static Vector2 GetDirection(Vector2 from, Vector2 to)
    {
        return (to - from).normalized;
    }
    
    public static float Distance(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to);
    }
    
    public static Quaternion RotateTowards(Transform transform, Vector3 target, float speed)
    {
        Vector3 direction = GetDirection(transform.position, target);
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        return Quaternion.Slerp(transform.rotation, lookRotation, speed * Time.deltaTime);
    }
    
    public static float RotateTowards2D(Transform transform, Vector2 target, float speed)
    {
        Vector2 direction = target - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Mathf.LerpAngle(transform.eulerAngles.z, angle, speed * Time.deltaTime);
    }
    
    public static Vector3 RandomInRadius(Vector3 center, float radius)
    {
        return center + UnityEngine.Random.insideUnitSphere * radius;
    }
    
    public static Vector2 RandomInRadius2D(Vector2 center, float radius)
    {
        return center + UnityEngine.Random.insideUnitCircle * radius;
    }
    
    public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
    {
        return Vector3.ClampMagnitude(vector, maxLength);
    }
    
    public static float AngleBetween(Vector3 from, Vector3 to)
    {
        return Vector3.Angle(from, to);
    }
    
    public static bool IsInRange(Vector3 from, Vector3 to, float range)
    {
        return Vector3.Distance(from, to) <= range;
    }
}