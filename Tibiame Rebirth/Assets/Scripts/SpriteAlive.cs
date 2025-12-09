using UnityEngine;

public class SpriteAlive : MonoBehaviour
{
    // ความแรงของการขยับ
    public float moveAmplitude = 0.05f;
    // ความเร็วในการขยับ
    public float moveSpeed = 2f;

    // ความแรงของการสเกล
    public float scaleAmplitude = 0.05f;
    // ความเร็วในการสเกล
    public float scaleSpeed = 2f;

    private Vector3 initialPosition;
    private Vector3 initialScale;

    void Start()
    {
        // เก็บตำแหน่งและสเกลเริ่มต้น
        initialPosition = transform.localPosition;
        initialScale = transform.localScale;
    }

    void Update()
    {
        // ทำให้ Sprite ขยับขึ้นลงเล็กน้อย
        float yOffset = Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
        transform.localPosition = initialPosition + new Vector3(0, yOffset, 0);

        // ทำให้ Sprite ขยาย/หดเล็กน้อย
        float scaleOffset = 1 + Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
        transform.localScale = initialScale * scaleOffset;
    }
}
