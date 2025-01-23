using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CastingSystem : Singleton<CastingSystem>
{
    [SerializeField] private Database _database;

    private List<SkillData> _skillDatas;
    public List<SkillData> SkillDatas => _skillDatas;

    private List<Cast> _castList = new();

    private Dictionary<List<Cast>, SkillData> _castToSkillMap;

    private bool _isTyping = false;   
    private string _typedString = "";

    private static readonly CastingStartEventArgs StartEventArgs = new();
    private static readonly CastingEndEventArgs FailEventArgs = new()
    {
        isSuccess = false,
        resultCode = (int)CastingResultCode.Error_FailedTyping
    };

    protected override void Initialize()
    {
        _skillDatas = _database.GetDatas<SkillData>();
        _castToSkillMap = _skillDatas.ToDictionary(skill => skill.Casts);
    }

    public void Update()
    {
        if (!_isTyping)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartTyping();
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
                                _castList.Add((Cast)cast);
                                _typedString = string.Empty;
                                break;
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
        Debug.Log("Ÿ���� ����!");

        _castList.Clear();

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingStart, StartEventArgs);
    }

    private void RemoveTyping()
    {
        if(_typedString.Length > 0)
        {
            _typedString = "";
        }
        else if(_castList.Count > 0)
        {
            _castList.RemoveAt(_castList.Count - 1);
        }
        
        GameEventSystem.Instance.Publish((int)SystemEvents.CastingRemove);
    }

    private void EndTyping()
    {
        _isTyping = false;
        _typedString = "";
        Debug.Log("Ÿ���� ����!");
    }

    private void OnFailTyping()
    {
        Debug.Log("��Ÿ �߻�! ���� ó��");
        GameEventSystem.Instance.Publish((int)SystemEvents.CastingEnd, FailEventArgs);
        EndTyping();
    }

    private void OnSuccessTyping(SkillData data)
    {
        Debug.Log("���� ����! ���� ó��");

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
        Debug.Log($"��ȿ �Է� �߻�: {latestChar}, ���� �Է� ����: {currentString}");

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingInput, new CastingInputEventArgs
        {
            typedString = currentString
        });
    }
}
