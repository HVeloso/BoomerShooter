public interface IWeaponHandler
{
    public string WeaponName { get; }

    public void SetActive(bool active);
    public void AttackInputed(bool inputValue);
}
