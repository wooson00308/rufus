using System;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class CastingSystem : Singleton<CastingSystem>
{
    private Unit _player;
    private Skill _skill;
    private SkillLevelData _currentLevelData;

    private int _successLevel;
    private int _castLevel;
    private bool _isCasting;

    private bool _isTyping = false;
    private bool _isMaxLevel = false;
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

        bool isCastable = _player.Status.Mana.Value >= _player.Status.Mana.Max / 20;
        bool isSuccess = _successLevel > 0;

        if (Input.GetKeyDown(KeyCode.LeftShift) && isCastable && !_isMaxLevel)
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
            if (isSuccess)
            {
                SuccessCasting();
            }
        }

        if (isSuccess && Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoveTyping();
        }

        if (_isTyping)
        {
            bool isOveload = _player.Status.Mana.Value <= 0;

            if (isOveload)
            {
                _typedString = "";
                EndCasting(CancelEventArgs);
                return;
            }

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

        GameTime.TimeScale = 0.25f;

        _currentLevelData = _skill.Data.GetSkillLevelData(_castLevel);
        _castString = _currentLevelData.Cast;

        _player.Status.SetManaGeneration(-_currentLevelData.ManaCost);

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingStart, new CastingStartEventArgs
        {
            skillData = _skill.Data,
            succesLevel = _successLevel,
            castLevel = _castLevel,
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
            _isMaxLevel = false;
        }

        _player.Status.SetManaGeneration();

        _isTyping = false;
    }

    private void RemoveTyping()
    {
        if (_successLevel <= 0) return;
        _successLevel--;
        _typedString = "";

        if (_isMaxLevel)
        {
            _isMaxLevel = false;
        }
        else
        {
            if (_castLevel > 1)
            {
                _currentLevelData = _skill.Data.GetSkillLevelData(--_castLevel);
                _castString = _currentLevelData.Cast;
            }
        }

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingRemove, new CastingInputEventArgs
        {
            skillData = _skill.Data,
            typedString = _typedString,
            castString = _castString,
            succesLevel = _successLevel,
            castLevel = _castLevel
        });
    }

    private void EndCasting(CastingEndEventArgs args)
    {
        args.succesLevel = _castLevel;
        args.castString = _castString;
        args.typedString = _typedString;
        GameEventSystem.Instance.Publish((int)SystemEvents.CastingEnd, args);
        var isPause = args.Equals(CancelEventArgs);
        EndTyping(isPause);
    }

    private void SuccessCasting()
    {
        GameEventSystem.Instance.Publish((int)SystemEvents.CastingEnd, new CastingEndEventArgs
        {
            skillData = _skill.Data,
            succesLevel = _successLevel,
            castLevel = _castLevel,
            castString = _castString,
            typedString = _typedString,
            isSuccess = true,
            resultCode = (int)CastingResultCode.Success,
        });

        EndTyping(false);
    }

    private void OnCastingKeyInput(string key, bool isTypo = false)
    {
        if(isTypo)
        {
            _typedString = _typedString[..^1];

            int overloadTypoDamage = _player.Status.Mana.Value - _currentLevelData.FailedManaCost;
            _player.UpdateMana(-_currentLevelData.FailedManaCost);

            if(overloadTypoDamage < 0)
            {
                overloadTypoDamage = Math.Abs(overloadTypoDamage);
                _player.OnHit(overloadTypoDamage, _player);
            }
        }

        GameEventSystem.Instance.Publish((int)SystemEvents.Casting, new CastingInputEventArgs
        {
            skillData = _skill.Data,
            succesLevel = _successLevel,
            castLevel = _castLevel,
            keyString = key,
            isTypo = isTypo,
            typedString = _typedString,
            castString = _castString,
        });
    }

    private void OnValidKeyInput(string latestChar, string currentString)
    {
        _successLevel++;

        if (_castLevel < _skill.Level)
        {    
            _castLevel++;

            _typedString = string.Empty;

            _isMaxLevel = false;
            _currentLevelData = _skill.Data.GetSkillLevelData(_castLevel);
            _player.Status.SetManaGeneration(-_currentLevelData.ManaCost);

            _castString = _currentLevelData.Cast;
        }
        else
        {
            _isMaxLevel = true;
            EndCasting(CancelEventArgs);
        }

        GameEventSystem.Instance.Publish((int)SystemEvents.CastingInput, new CastingInputEventArgs
        {
            skillData = _skill.Data,
            succesLevel = _successLevel,
            castLevel = _castLevel,
            typedString = currentString,
            castString = _castString,
        });
    }
}
