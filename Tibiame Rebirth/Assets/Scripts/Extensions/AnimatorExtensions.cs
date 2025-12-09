using UnityEngine;

/// <summary>
/// Extension Methods สำหรับ Animator
/// </summary>
public static class AnimatorExtensions
{
    /// <summary>
    /// ตรวจสอบว่า Animator มี Parameter ที่ระบุหรือไม่
    /// </summary>
    /// <param name="animator">Animator ที่ต้องการตรวจสอบ</param>
    /// <param name="parameterName">ชื่อ Parameter ที่ต้องการตรวจสอบ</param>
    /// <returns>true ถ้ามี Parameter นั้น, false ถ้าไม่มี</returns>
    public static bool HasParameter(this Animator animator, string parameterName)
    {
        if (animator == null || string.IsNullOrEmpty(parameterName))
            return false;
            
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName)
                return true;
        }
        
        return false;
    }
}
