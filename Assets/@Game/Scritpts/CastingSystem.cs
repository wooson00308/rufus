using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CastingSystem : Singleton<CastingSystem>
{
    [SerializeField] private List<SkillData> _skillDatas;

    private List<Cast> _castList = new();

    private bool _isTyping = false;   
    private string _typedString = ""; 

    public void Update()
    {
        if (!_isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            StartTyping();
            return;
        }

        if (!_isTyping) return;

        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
                {
                    SkillData data = null;
                    foreach (var skillData in _skillDatas)
                    {
                        if (_castList.SequenceEqual(skillData.Casts))
                        {
                            data = skillData;
                            break;
                        }
                    }

                    if (data == null)
                    {
                        OnFailTyping();
                    }
                    else
                    {
                        OnSuccessTyping(data);
                    }

                    return;
                }

                if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
                {
                    string inputChar = keyCode.ToString();
                    string testString = _typedString + inputChar;

                    foreach(var cast in Enum.GetValues(typeof(Cast)))
                    {
                        if (cast.ToString().StartsWith(testString))
                        {
                            _typedString = testString;

                            OnValidKeyInput(inputChar, _typedString);

                            if (_typedString.Length == cast.ToString().Length)
                            {
                                _castList.Add((Cast)cast);
                                _typedString = string.Empty;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    private void StartTyping()
    {
        _isTyping = true;
        _typedString = "";
        Debug.Log("타이핑 시작!");

        _castList.Clear();

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingStartEventArgs());
    }

    private void EndTyping()
    {
        _isTyping = false;
        _typedString = "";
        Debug.Log("타이핑 종료!");
    }

    private void OnFailTyping()
    {
        Debug.Log("오타 발생! 실패 처리");
        GameEventSystem.Instance.Publish((int)SystemEvents.CasingEnd, new CastingEndEventArgs
        {
            isSuccess = false,
            resultCode = (int)CastingResultCode.Error_FailedTyping
        });
        EndTyping();
    }

    private void OnSuccessTyping(SkillData data)
    {
        Debug.Log("정답 성공! 성공 처리");

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingEnd, new CastingEndEventArgs
        {
            skillData = data,
            isSuccess = true,
            resultCode = (int)CastingResultCode.Success
        });
        EndTyping();
    }

    private void OnValidKeyInput(string latestChar, string currentString)
    {
        Debug.Log($"유효 입력 발생: {latestChar}, 현재 입력 누적: {currentString}");

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingInputEventArgs());
    }
}
