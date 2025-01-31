using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillHudView : BaseView
{
    private enum Texts
    {
        CastingText,
        CastResultText,
        CastSkillText
    }

    private bool _isCasting;
    private bool _isCastingStart;
    private bool _isCastingInput;

    [field: SerializeField] public List<Image> LevelUIs { get; private set; }

    private Color _darkGray = new(0.25f, 0.25f, 0.25f);

    public void Awake()
    {
        BindUI();
    }

    public void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)SkillEvents.UseSkill, OnUseSkill);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.CastingStart, OnCastingStart);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.Casting, OnCasting);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.CastingRemove, OnCastingRemove);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.CastingInput, OnCastingInput);
        GameEventSystem.Instance.Subscribe((int)SystemEvents.CastingEnd, OnCastingEnd);
    }

    public override void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private TextMeshProUGUI GetText(Texts type)
    {
        return Get<TextMeshProUGUI>((int)type);
    }

    private void UpdateText(Texts type, string message)
    {
        GetText(type).text = message;
    }

    private void OnUseSkill(object gameEvent)
    {
        var args = gameEvent as SkillEventArgs;

        UpdateText(Texts.CastSkillText, $"영창 시동: {args.data.DisplayName}");
        GetText(Texts.CastSkillText).color = args.data.Color;
    }
    private void OnCastingStart(object gameEvent = null)
    {
        var args = gameEvent as CastingStartEventArgs;

        _isCasting = true;
        _isCastingStart = true;

        UpdateCastingString(args.castLevel, args.typedString, args.castString);
        UpdateText(Texts.CastResultText, $"영창 모드");
        GetText(Texts.CastResultText).color = Color.white;
    }

    private void OnCastingRemove(object gameEvent)
    {
        var args = gameEvent as CastingInputEventArgs;

        UpdateCastingString(args.castLevel, "", args.castString);
        UpdateSkillLevelUI(args.succesLevel, args.skillData);
    }

    private void OnCasting(object gameEvent)
    {
        if (!_isCasting) return;

        var args = gameEvent as CastingInputEventArgs;

        if (_isCastingStart || _isCastingInput)
        {
            _isCastingStart = false;
            _isCastingInput = false;
        }

        UpdateCastingString(args.castLevel, args.typedString, args.castString, args.isTypo);
    }

    private void UpdateCastingString(int level, string typedString, string castString, bool isTypo = false)
    {
        if (typedString.Length > castString.Length)
            return;

        string remainingText = castString[typedString.Length..];
        string nextChar = remainingText.Length > 0 ? remainingText[..1] : "";
        string restChars = remainingText.Length > 1 ? remainingText[1..] : "";
        string nextCharColor = !isTypo ? "FFFF00" : "FF0000";

        if (_isCasting)
        {
            if(nextChar == " ")
            {
                nextChar = "_";
            }
        }
        else
        {
            nextCharColor = "808080";
        }

        string formattedText =
            $"<color=#404040>{level}소절)</color>" +
            $"<color=#FFFFFF>{typedString}</color>" +
            $"<color=#{nextCharColor}>{nextChar}</color>" +
            $"<color=#808080>{restChars}</color>";

        UpdateText(Texts.CastingText, formattedText);
    }

    private void OnCastingInput(object gameEvent)
    {
        var args = gameEvent as CastingInputEventArgs;
        var skillData = args.skillData;

        UpdateSkillLevelUI(args.succesLevel, skillData);

        if (args.succesLevel >= args.castLevel)
        {
            var levelData = skillData.GetSkillLevelData(args.succesLevel);

            UpdateText(Texts.CastingText, $"{skillData.CastResult(args.succesLevel)}");
            GetText(Texts.CastingText).color = levelData.Color;

            UpdateText(Texts.CastResultText, $"이동 모드");
            GetText(Texts.CastResultText).color = _darkGray;
        }
        else
        {
            _isCastingInput = true;

            UpdateCastingString(args.castLevel, "", args.castString);
            UpdateText(Texts.CastResultText, $"영창 모드");
        }
    }

    private void UpdateSkillLevelUI(int level = 0, SkillData data = null)
    {
        int index = 0;
        foreach (var ui in LevelUIs)
        {
            bool isActive = index < level;
            ui.gameObject.SetActive(isActive);
            
            if(isActive && data != null)
            {
                var levelData = data.GetSkillLevelData(index + 1);
                ui.color = levelData.Color;
            }

            index++;
        }
    }

    private void OnCastingEnd(object gameEvent)
    {
        _isCasting = false;
        _isCastingStart = false;

        var args = gameEvent as CastingEndEventArgs;
        var skillData = args.skillData;

        if (args.isSuccess)
        {
            UpdateSkillLevelUI(args.succesLevel, args.skillData);

            var levelData = skillData.GetSkillLevelData(args.succesLevel);

            UpdateText(Texts.CastingText, $"{levelData.DisplayName}) {skillData.CastResult(args.succesLevel)}");
            GetText(Texts.CastingText).color = levelData.Color;

            StartCoroutine(OnUseSkillTextClearProcess());
        }
        else
        {
            UpdateCastingString(args.succesLevel, args.typedString, args.castString);
        }

        UpdateText(Texts.CastResultText, $"이동 모드");
        GetText(Texts.CastResultText).color = _darkGray;
    }

    private IEnumerator OnUseSkillTextClearProcess()
    {
        float time = 0;

        while (time < 1.5f)
        {
            if (_isCastingStart)
            {
                _isCastingStart = false;
                UpdateSkillLevelUI();
                yield break;
            }

            time += Time.deltaTime;
            yield return null;
        }

        UpdateText(Texts.CastingText, string.Empty);
        GetText(Texts.CastingText).color = Color.white;
        UpdateSkillLevelUI();
    }
}
