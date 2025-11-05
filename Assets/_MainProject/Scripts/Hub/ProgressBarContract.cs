
public interface ProgressBarContract
{
    public void SetMin(float v);
    public void SetMax(float v);
    public void SetValue(float v);
    public void SetActive(bool v);
    public bool IsActive();
    public float GetNormalValue();

}
