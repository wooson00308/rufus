using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillHudView : BaseView
{
    private enum Texts
    {
        CastingText,
        CastInputText,
        CastResultText
    }

    private bool _isCastingStart;
    private bool _isCastingInput;
    private bool _isProcessingTypoFx;

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
        var startArgs = args as CastingStartEventArgs;

        _isCastingStart = true;

        UpdateCastingText(Texts.CastingText, startArgs.typedString);
        UpdateCastingText(Texts.CastInputText, startArgs.castingString);
    }

    private void OnCastingRemove(object args)
    {
        var inputArgs = args as CastingInputEventArgs;

        if(GetText(Texts.CastingText).text != string.Empty)
        {
            UpdateCastingText(Texts.CastingText, string.Empty);
        }
        else
        {
            UpdateCastingText(Texts.CastInputText, inputArgs.castingString);
        }
    }

    private void OnCasting(object args)
    {
        _isProcessingTypoFx = false;
        var inputArgs = args as CastingInputEventArgs;

        if (_isCastingStart || _isCastingInput)
        {
            _isCastingStart = false;
            _isCastingInput = false;
        }

        if (inputArgs.isTypo)
        {
            GetText(Texts.CastingText).color = Color.red;
        }
        else
        {
            UpdateCastingText(Texts.CastingText, GetText(Texts.CastingText).text + inputArgs.typedString.ToUpper());
        }
    }

    private IEnumerator ProcessTypoFx()
    {
        if (_isProcessingTypoFx) yield break;
        _isProcessingTypoFx = true;

        float time = 0; 
        while(time < .2f)
        {
            if(!_isProcessingTypoFx)
            {
                break;
            }

            time += GameTime.DeltaTime;
            yield return null;
        }

        GetText(Texts.CastingText).color = Color.white;
    }

    private void OnCastingInput(object args)
    {
        var inputArgs = args as CastingInputEventArgs;

        _isCastingInput = true;

        UpdateCastingText(Texts.CastingText, string.Empty);
        UpdateCastingText(Texts.CastInputText, inputArgs.castingString);
    }

    private void OnCastingEnd(object args)
    {
        _isCastingStart = false;

        var endArgs = args as CastingEndEventArgs;
        var skillData = endArgs.skillData;
        string levelString;

        UpdateCastingText(Texts.CastingText, string.Empty);
        UpdateCastingText(Texts.CastInputText, string.Empty);

        if (endArgs.isSuccess)
        {
            var levelData = skillData.GetSkillLevelData(endArgs.level);
            levelString = levelData.DisplayName != string.Empty ? $" | {levelData.DisplayName}" : "";
            UpdateCastingText(Texts.CastResultText, $"스킬: {skillData.DisplayName}{levelString}");

            StartCoroutine(OnUseSkillTextClearProcess());
        }
        else
        {
            UpdateCastingText(Texts.CastResultText, string.Empty);
        }
    }

    private IEnumerator OnUseSkillTextClearProcess()
    {
        float time = 0;

        while (time < 0.8f)
        {
            if (_isCastingStart)
            {
                _isCastingStart = false;
                yield break;
            }

            time += Time.deltaTime;
            yield return null;
        }

        UpdateCastingText(Texts.CastingText, string.Empty);
        UpdateCastingText(Texts.CastInputText, string.Empty);
        UpdateCastingText(Texts.CastResultText, string.Empty);
    }
}
