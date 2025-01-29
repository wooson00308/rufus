using System.Collections;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.GPUSort;

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
    private bool _isProcessingTypoFx;

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
    }
    private void OnCastingStart(object args = null)
    {
        var startArgs = args as CastingStartEventArgs;

        _isCasting = true;
        _isCastingStart = true;

        UpdateCastingString(startArgs.typedString, startArgs.castString);
        UpdateText(Texts.CastResultText, $"영창 실행");
        GetText(Texts.CastResultText).color = Color.white;
    }

    private void OnCastingRemove(object args)
    {
        var inputArgs = args as CastingInputEventArgs;

        if(GetText(Texts.CastingText).text != string.Empty)
        {
            UpdateText(Texts.CastingText, string.Empty);
        }
        else
        {
            UpdateCastingString(inputArgs.typedString, inputArgs.castString);
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
            UpdateCastingString(inputArgs.typedString, inputArgs.castString, true);
            GetText(Texts.CastResultText).color = Color.red;

            UpdateText(Texts.CastResultText, $"영창 오류!!");
            StartCoroutine(ProcessTypoFx(inputArgs.typedString, inputArgs.castString));
        }
        else
        {
            UpdateCastingString(inputArgs.typedString, inputArgs.castString);
        }
    }

    private void UpdateCastingString(string typedString, string castString, bool isTypo = false)
    {
        if (typedString.Length > castString.Length)
            return;

        string remainingText = castString[typedString.Length..];
        string colorCodeText = !isTypo ? "FFFFFF" : "FF0000";
        string formattedText = $"<color=#{colorCodeText}>{typedString}</color><color=#808080>{remainingText}</color>";
        UpdateText(Texts.CastingText, formattedText);
    }

    private IEnumerator ProcessTypoFx(string typedString, string castString)
    {
        if (_isProcessingTypoFx) yield break;
        _isProcessingTypoFx = true;

        float time = 0; 
        while(time < .8f)
        {
            if(!_isProcessingTypoFx)
            {
                break;
            }

            if (!_isCasting) yield break;

            time += GameTime.DeltaTime;
            yield return null;
        }

        UpdateCastingString(typedString, castString);
        GetText(Texts.CastResultText).color = Color.white;
        UpdateText(Texts.CastResultText, $"영창 실행");
    }

    private void OnCastingInput(object args)
    {
        var inputArgs = args as CastingInputEventArgs;

        _isCastingInput = true;

        UpdateCastingString("", inputArgs.castString);
    }

    private void OnCastingEnd(object args)
    {
        _isCasting = false;
        _isCastingStart = false;

        var endArgs = args as CastingEndEventArgs;
        var skillData = endArgs.skillData;
        string levelString;

        if (endArgs.isSuccess)
        {
            GetText(Texts.CastingText).color = Color.green;
            GetText(Texts.CastResultText).color = Color.green;

            var levelData = skillData.GetSkillLevelData(endArgs.level);
            levelString = levelData.DisplayName != string.Empty ? $" | {levelData.DisplayName}" : "";

            UpdateText(Texts.CastingText, skillData.CastResult(endArgs.level));
            UpdateText(Texts.CastResultText, $"스킬: {skillData.DisplayName}{levelString}");

            StartCoroutine(OnUseSkillTextClearProcess());
        }
        else
        {
            //UpdateText(Texts.CastingText, string.Empty);
            UpdateText(Texts.CastResultText, $"영창 중지");
            GetText(Texts.CastResultText).color = Color.gray;
        }
    }

    private IEnumerator OnUseSkillTextClearProcess()
    {
        float time = 0;

        while (time < 1.5f)
        {
            if (_isCastingStart)
            {
                _isCastingStart = false;
                yield break;
            }

            time += Time.deltaTime;
            yield return null;
        }

        GetText(Texts.CastingText).color = Color.white;
        GetText(Texts.CastResultText).color = Color.white;

        UpdateText(Texts.CastingText, string.Empty);
        UpdateText(Texts.CastResultText, string.Empty);
    }
}
