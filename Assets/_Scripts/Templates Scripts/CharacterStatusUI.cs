using System.Text;
using TMPro;
using UnityEngine;

public class CharacterStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _uiText;
    [SerializeField] private Transform _playerBody;

    private float _higherPosition = 0f;

    private void Update()
    {
        _uiText.text = GetUiText();
    }

    private string GetUiText()
    {
        StringBuilder stringBuilder = new();

        float currentPos = _playerBody.position.y;
        
        if (_higherPosition < currentPos)
            _higherPosition = currentPos;

        stringBuilder.AppendLine($"Higher Vertical Position: {_higherPosition:N2}");
        stringBuilder.Append($"Current Vertical Position: {currentPos:N2}");
        return stringBuilder.ToString();
    }
}
