using TMPro;
using UnityEngine;

/// <summary>
/// 솔직히 자식 만들고 상속 받아서 하면 되는데.. 
/// </summary>
public class HintHudView : BaseView
{
    public enum Texts
    {
        HintText 
    }
    public enum HintType
    {
        Control,
        Skill
    }

    [field: SerializeField] public HintType Type { get; private set; }

    private bool _isSkillUseable;
    private string _skillDisplayName;
    private string _skillColorHtmlString;

    public override void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void Awake()
    {
        BindUI();

        GameEventSystem.Instance.Subscribe((int)SkillEvents.UseSkill, OnUseSkill);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.CastingStart, OnCastingStart);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.Casting, Casting);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.CastingRemove, OnCastingRemove);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.CastingInput, CastingInput);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.CastingEnd, CastingEnd);
    }

    public void OnUseSkill(object gameEvent)
    {
        string text;
        if (Type == HintType.Skill)
        {
            text = $"<color=#FFFFFF>[1] - 화염구</color>\n" +
                   $"<color=#808080>[2] - 비어 있음</color>\n" +
                   $"<color=#808080>[3] - 비어 있음</color>\n" +
                   $"<color=#808080>[4] - 비어 있음</color>";
        }
        else
        {
            text = $"<color=#FFFF00>[W][A][S][D]</color> - 이동\n" +
                   $"<color=#FFFF00>[J]</color> - 기본 공격\n" +
                   $"<color=#FFFF00>[Shift]</color> - 영창 모드";
        }

        Get<TextMeshProUGUI>((int)Texts.HintText).SetText(text);
    }

    private void OnCastingStart(object gameEvent)
    {
        if (Type != HintType.Control) return;

        string text = $"<color=#FFFF00>[Shift]</color> - 이동 모드";

        if(_isSkillUseable)
        {
            text += $"\n\n" +
                $"<color=#FFFF00>[Backspace]</color> - 이전 소절\n" +
                $"<color=#FFFF00>[Enter]</color> - <color=#{_skillColorHtmlString}>발동:{_skillDisplayName}</color>";
        }

        Get<TextMeshProUGUI>((int)Texts.HintText).SetText(text);
    }

    private void CastingEnd(object gameEvent)
    {
        var args = gameEvent as CastingEndEventArgs;

        if (Type != HintType.Control) return;

        string text = $"<color=#FFFF00>[W][A][S][D]</color> - 이동\n" +
                      $"<color=#FFFF00>[J]</color> - 기본 공격\n" +
                      $"<color=#FFFF00>[Shift]</color> - 영창 모드"; ;

        if (args.isSuccess)
        {
            _isSkillUseable = false;
        }
        else if (_isSkillUseable)
        {
            text += $"\n\n" +
                $"<color=#FFFF00>[Backspace]</color> - 이전 소절\n" +
                $"<color=#FFFF00>[Enter]</color> - <color=#{_skillColorHtmlString}>발동:{_skillDisplayName}</color>";
        }

        Get<TextMeshProUGUI>((int)Texts.HintText)
            .SetText(text);
    }

    private void Casting(object gameEvent)
    {
        if (Type != HintType.Control) return;
    }

    private void OnCastingRemove(object gameEvent)
    {
        if (Type != HintType.Control) return;

        var args = gameEvent as CastingInputEventArgs;

        string text = $"<color=#FFFF00>[W][A][S][D]</color> - 이동\n" +
                      $"<color=#FFFF00>[J]</color> - 기본 공격\n" +
                      $"<color=#FFFF00>[Shift]</color> - 영창 모드";

        if (args.castLevel > 1)
        {
            var levelData = args.skillData.GetSkillLevelData(args.succesLevel);
            _skillColorHtmlString = ColorUtility.ToHtmlStringRGB(levelData.Color);
            _skillDisplayName = levelData.DisplayName;

            text += $"\n\n" +
                $"<color=#FFFF00>[Backspace]</color> - 이전 소절\n" +
                $"<color=#FFFF00>[Enter]</color> - <color=#{_skillColorHtmlString}>발동:{_skillDisplayName}</color>";
        }
        else
        {
            _isSkillUseable = false;
        }

        Get<TextMeshProUGUI>((int)Texts.HintText)
            .SetText(text);
    }

    private void CastingInput(object gameEvent)
    {
        if (Type != HintType.Control) return;

        _isSkillUseable = true;

        var args = gameEvent as CastingInputEventArgs;
        var levelData = args.skillData.GetSkillLevelData(args.succesLevel);

        _skillColorHtmlString = ColorUtility.ToHtmlStringRGB(levelData.Color);
        _skillDisplayName = levelData.DisplayName;

        Get<TextMeshProUGUI>((int)Texts.HintText)
            .SetText($"<color=#FFFF00>[Shift]</color> - 이동 모드\n" +
                     $"\n" +
                     $"<color=#FFFF00>[Backspace]</color> - 이전 소절\n" +
                     $"<color=#FFFF00>[Enter]</color> - <color=#{_skillColorHtmlString}>발동:{_skillDisplayName}</color>");
    }
}
