public interface IGunHandler
{
    public string GunName { get; }

    public void SetActive(bool active);
    public void ShootInputed(bool inputValue);
}
