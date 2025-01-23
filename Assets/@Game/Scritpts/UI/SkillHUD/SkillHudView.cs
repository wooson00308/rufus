using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class SkillHudView : BaseView
{
    private enum Texts
    {
        CastingText,
        CastInputText
    }

    private bool _isCastingStart;
    private bool _isCastingInput;

    private const string CASTING_TEXT_NULL = "주문을 입력하세요";

    public void Awake()
    {
        BindUI();
    }

    public void OnEnable()
    {
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

    private void UpdateCastingText(Texts type, string message)
    {
        GetText(type).text = message;
    }

    private void OnCastingStart(object args = null)
    {
        _isCastingStart = true;

        ResetCastingText();
        UpdateCastingText(Texts.CastInputText, string.Empty);
    }

    private void ResetCastingText()
    {
        _isCastingStart = true;
        UpdateCastingText(Texts.CastingText, CASTING_TEXT_NULL);
    }

    private void OnCastingRemove(object args)
    {
        if(!GetText(Texts.CastingText).text.Equals(CASTING_TEXT_NULL))
        {
            ResetCastingText();
        }
        else
        {
            var castInputText = GetText(Texts.CastInputText).text;
            if (castInputText.Length > 0)
            {
                var split = castInputText.Split(" + ").ToList();
                split.RemoveAt(split.Count - 1);
                ResetCastingText();
                UpdateCastingText(Texts.CastInputText, string.Join(" + ", split));
            }
        }
    }

    private void OnCasting(object args)
    {
        if (_isCastingStart || _isCastingInput)
        {
            _isCastingStart = false;
            _isCastingInput = false;
            UpdateCastingText(Texts.CastingText, string.Empty);
        }

        var castingInputEventArgs = args as CastingInputEventArgs;
        UpdateCastingText(Texts.CastingText, GetText(Texts.CastingText).text + castingInputEventArgs.typedString.ToUpper());
    }

    private void OnCastingInput(object args)
    {
        var castingInputEventArgs = args as CastingInputEventArgs;

        _isCastingInput = true;

        var castInputText = GetText(Texts.CastInputText).text;
        if (!string.IsNullOrEmpty(castInputText))
        {
            UpdateCastingText(Texts.CastInputText, castInputText + " + ");
        }

        ResetCastingText();
        UpdateCastingText(Texts.CastInputText, GetText(Texts.CastInputText).text + castingInputEventArgs.typedString);
    }

    private void OnCastingEnd(object args)
    {
        var endEventArgs = args as CastingEndEventArgs;

        UpdateCastingText(Texts.CastingText, string.Empty);

        if (endEventArgs.isSuccess)
        {
            UpdateCastingText(Texts.CastInputText, "스킬: " + endEventArgs.skillData.DisplayName);
        }
        else
        {
            UpdateCastingText(Texts.CastInputText, "캐스팅 실패!");
        }

        _isCastingStart = false;
        StartCoroutine(OnUseSkillTextClearProcess());
    }

    private IEnumerator OnUseSkillTextClearProcess()
    {
        float time = 0;

        while (time < 0.8f)
        {
            if (_isCastingStart)
            {
                yield break;
            }

            time += Time.deltaTime;
            yield return null;
        }

        UpdateCastingText(Texts.CastingText, string.Empty);
        UpdateCastingText(Texts.CastInputText, string.Empty);
    }
}
