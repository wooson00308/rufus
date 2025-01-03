using UnityEngine;

public interface IPopup : IElementBase
{
    public void Show();
    public void Hide();
    public void ClosePopupUI();

}