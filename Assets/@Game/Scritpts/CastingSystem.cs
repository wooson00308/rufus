using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CastingSystem : Singleton<CastingSystem>
{
    [SerializeField] private Database _database;

    private Unit _player;
    private Skill _skill;
    private SkillLevelData _levelData;

    private bool _isTyping = false;
    private string _typedString = "";
    private string _castString = "";

    private static readonly CastingStartEventArgs StartEventArgs = new();
    private static readonly CastingEndEventArgs FailEventArgs = new()
    {
        isSuccess = false,
        resultCode = (int)CastingResultCode.Error_FailedTyping
    };
    private static readonly CastingEndEventArgs CancelEventArgs = new()
    {
        isSuccess = false,
        resultCode = (int)CastingResultCode.Error_CancelTyping
    };


    protected override void Initialize()
    {
        GameEventSystem.Instance.Subscribe((int)SkillEvents.UseSkill, OnUseSkill);
    }

    public void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)SkillEvents.UseSkill, OnUseSkill);
    }

    private void OnUseSkill(object gameEvent)
    {
        var args = gameEvent as SkillEventArgs;

        _player = args.publisher;
        _skill = _player.GetSkill(args.data.Id);
        _levelData = _skill.CurrentLevelData;
    }

    public void Update()
    {
        if (!_isTyping)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _isTyping = !_isTyping;

                if(_isTyping)
                {
                    StartCasting();
                }
                else
                {
                    PauseCasting(CancelEventArgs);
                }

                StartCasting();
            }
        }
        else
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
                    {

                    }
                    else if (keyCode == KeyCode.Backspace)
                    {
                        RemoveTyping();
                    }
                    else if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
                    {
                        string inputChar = keyCode.ToString();
                        _typedString += inputChar;

                        OnCastingKeyInput(inputChar);

                        foreach (var cast in Enum.GetValues(typeof(Cast)))
                        {
                            if (cast.ToString().Equals(_typedString))
                            {
                                OnValidKeyInput(inputChar, _typedString);
                                _typedString = string.Empty;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void StartCasting()
    {
        _isTyping = true;
        Debug.Log("타이핑 시작!");

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingStart, StartEventArgs);
    }

    private void RemoveTyping()
    {
        if (_typedString.Length > 0)
        {
            _typedString = "";
            _castString = "";
        }

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingRemove);
    }

    private void EndTyping(bool isClearTypedString = true)
    {
        if(isClearTypedString) _typedString = "";
        _isTyping = false;

        Debug.Log("타이핑 종료!");
    }

    private void PauseCasting(CastingEndEventArgs args)
    {
        Debug.Log("타이핑 일시정지!");
        GameEventSystem.Instance.Publish((int)SystemEvents.CastingEnd, args);
        EndTyping(false);
    }

    private void SuccessCasting(SkillData data)
    {
        Debug.Log("캐스팅 성공! 성공 처리");

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingEnd, new CastingEndEventArgs
        {
            skillData = data,
            isSuccess = true,
            resultCode = (int)CastingResultCode.Success
        });
        EndTyping();
    }

    private void OnCastingKeyInput(string key)
    {
        GameEventSystem.Instance.Publish((int)SystemEvents.Casting, new CastingInputEventArgs
        {
            typedString = key
        });
    }

    private void OnValidKeyInput(string latestChar, string currentString)
    {
        Debug.Log($"유효 입력 발생: {latestChar}, 현재 입력 문자열: {currentString}");

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingInput, new CastingInputEventArgs
        {
            typedString = currentString
        });
    }
}
