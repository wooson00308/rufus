using UnityEngine;
using UnityEngine.UI;

public class PlayerHudView : BaseView
{
    private Unit _player;

    public enum Images
    {
        HPBar,
        MPBar
    }

    public void Awake()
    {
        BindUI();
    }

    public override void BindUI()
    {
        Bind<Image>(typeof(Images));
    }

    public void SetPlayer(Unit player)
    {
        _player = player;

        _player.Status.Health.OnValueChanged += UpdateHealthHUD;
        _player.Status.Mana.OnValueChanged += UpdateManaHUD;
    }

    public void UpdateHealthHUD(int value)
    {
        if (_player == null) return;
        if (_player.Status.IsDeath)
        {
            Get<Image>((int)Images.HPBar).rectTransform.localScale = new Vector3(0, 1, 1);
        }
        else
        {
            float health = (float)value / _player.Status.Health.Max;
            Get<Image>((int)Images.HPBar).rectTransform.localScale = new Vector3(health, 1, 1);
        }
    }

    public void UpdateManaHUD(int value)
    {
        if (_player == null) return;
        if (_player.Status.IsDeath) return;

        float mana = (float)value / _player.Status.Mana.Max;
        Get<Image>((int)Images.MPBar).rectTransform.localScale = new Vector3(mana, 1, 1);
    }
}
