using System;
using UnityEngine;

public class CastingSystem : Singleton<CastingSystem>
{
    private Unit _player;
    private Skill _skill;
    private SkillLevelData _currentLevelData;

    private int _successLevel;
    private int _castLevel;
    private bool _isCasting;

    private bool _isTyping = false;
    private string _typedString = "";
    private string _castString = "";

    private static readonly CastingEndEventArgs CancelEventArgs = new()
    {
        isSuccess = false,
        resultCode = (int)CastingResultCode.Error_CancelTyping
    };

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

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
        if (_isCasting) return;
        _isCasting = true;

        var args = gameEvent as SkillEventArgs;

        _successLevel = 0;
        _castLevel = 1;
        _player = args.publisher;
        _skill = _player.GetSkill(args.data.Id);
        _currentLevelData = _skill.Data.GetSkillLevelData(_castLevel);
        _castString = _currentLevelData.Cast;
    }

    public void Update()
    {
        if (!_isCasting) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isTyping = !_isTyping;

            if (_isTyping)
            {
                StartCasting();
            }
            else
            {
                EndCasting(CancelEventArgs);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (_successLevel > 0)
            {
                SuccessCasting();
            }
        }

        if (_isTyping)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z || keyCode == KeyCode.Space || keyCode == KeyCode.Comma || keyCode == KeyCode.Period)
                    {
                        string inputChar = keyCode.ToString();

                        if (keyCode == KeyCode.Space)
                        {
                            inputChar = " ";
                        }
                        if (keyCode == KeyCode.Comma)
                        {
                            inputChar = ",";
                        }
                        if (keyCode == KeyCode.Period)
                        {
                            inputChar = ".";
                        }
                        
                        _typedString += inputChar;

                        bool isTypo = _typedString.Length > 0 && _typedString != _castString[.._typedString.Length];

                        OnCastingKeyInput(inputChar, isTypo);

                        if (_castString.Equals(_typedString))
                        {
                            OnValidKeyInput(inputChar, _typedString);
                            break;
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

        GameTime.TimeScale = 0.25f;

        _currentLevelData = _skill.Data.GetSkillLevelData(_castLevel);
        _castString = _currentLevelData.Cast;

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingStart, new CastingStartEventArgs
        {
            skillData = _skill.Data,
            level = _castLevel,
            castString = _castString,
            typedString = _typedString,
        });
    }

    private void EndTyping(bool isPause)
    {
        GameTime.TimeScale = 1f;

        if (!isPause)
        {
            _typedString = "";
            _successLevel = 0;
            _castLevel = 1;
        }
        
        _isTyping = false;

        Debug.Log("타이핑 정지!");
    }

    private void EndCasting(CastingEndEventArgs args)
    {
        args.level = _castLevel;
        args.castString = _castString;
        args.typedString = _typedString;
        GameEventSystem.Instance.Publish((int)SystemEvents.CastingEnd, args);
        var isPause = args.Equals(CancelEventArgs);
        EndTyping(isPause);
    }

    private void SuccessCasting()
    {
        Debug.Log("캐스팅 성공! 성공 처리");

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingEnd, new CastingEndEventArgs
        {
            skillData = _skill.Data,
            level = _successLevel,
            castString = _castString,
            typedString = _typedString,
            isSuccess = true,
            resultCode = (int)CastingResultCode.Success,
        });

        EndTyping(false);
    }

    private void OnCastingKeyInput(string key, bool isTypo = false)
    {
        if(isTypo) _typedString = _typedString[..^1];

        GameEventSystem.Instance.Publish((int)SystemEvents.Casting, new CastingInputEventArgs
        {
            skillData = _skill.Data,
            level = _castLevel,
            keyString = key,
            isTypo = isTypo,
            typedString = _typedString,
            castString = _castString,
        });
    }

    private void OnValidKeyInput(string latestChar, string currentString)
    {
        Debug.Log($"유효 입력 발생: {latestChar}, 현재 입력 문자열: {currentString}");

        _typedString = string.Empty;
        _successLevel++;

        if (_castLevel >= _skill.Level)
        {
            SuccessCasting();
        }
        else
        {
            _currentLevelData = _skill.Data.GetSkillLevelData(++_castLevel);
            _castString = _currentLevelData.Cast;

            GameEventSystem.Instance.Publish((int)SystemEvents.CastingInput, new CastingInputEventArgs
            {
                skillData = _skill.Data,
                level = _castLevel,
                typedString = currentString,
                castString = _castString,
            });
        }
    }
}
