using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class CastingSystem : Singleton<CastingSystem>
{

    [SerializeField] private string _correctAnswer = "HELLO"; // 정답 예시
    [SerializeField] private float _typingDuration = 5f;      // 제한 시간(초)

    private bool _isTyping = false;   
    private float _currentDuration = 0f; 
    private string _typedString = ""; 

    public void Update()
    {
        if (!_isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            StartTyping();
            return;
        }

        if (!_isTyping) return;

        _currentDuration -= Time.deltaTime;
        if (_currentDuration <= 0f || Input.GetKeyDown(KeyCode.Return))
        {
            CancelTyping();
            return;
        }

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
                {
                    string inputChar = keyCode.ToString();
                    string testString = _typedString + inputChar;

                    if (_correctAnswer.StartsWith(testString))
                    {
                        _typedString = testString;

                        OnValidKeyInput(inputChar, _typedString);

                        if (_typedString.Length == _correctAnswer.Length)
                        {
                            OnSuccessTyping();
                            return;
                        }
                    }
                    else
                    {
                        // 오타 -> 실패 처리
                        OnFailTyping();
                        return;
                    }
                }
            }
        }
    }

    private void StartTyping()
    {
        _isTyping = true;
        _currentDuration = _typingDuration;
        _typedString = "";
        Debug.Log("타이핑 시작!");

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingStartEventArgs
        {
            
        });
    }

    private void EndTyping()
    {
        _isTyping = false;
        _typedString = "";
        Debug.Log("타이핑 종료!");
    }

    private void CancelTyping()
    {
        Debug.Log("제한 시간 만료로 타이핑 취소");
        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingStartEventArgs
        {
            answer = _correctAnswer,
            typedString = _typedString
        });
        EndTyping();
    }

    private void OnFailTyping()
    {
        Debug.Log("오타 발생! 실패 처리");
        GameEventSystem.Instance.Publish((int)SystemEvents.CasingEnd, new CastingEndEventArgs
        {
            answer = _correctAnswer,
            typedString = _typedString,
            isSuccess = false,
            failedCode = (int)CastingFailedTypes.FailedTyping
        });
        EndTyping();
    }

    private void OnSuccessTyping()
    {
        Debug.Log("정답 성공! 성공 처리");

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingEnd, new CastingEndEventArgs
        {
            answer = _correctAnswer,
            typedString = _typedString,
            isSuccess = true,
        });
        EndTyping();
    }

    private void OnValidKeyInput(string latestChar, string currentString)
    {
        Debug.Log($"유효 입력 발생: {latestChar}, 현재 입력 누적: {currentString}");

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingInputEventArgs
        {
            answer = _correctAnswer,
            typedString = _typedString
        });
    }
}
