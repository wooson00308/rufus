using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class CastingSystem : Singleton<CastingSystem>
{

    [SerializeField] private string _correctAnswer = "HELLO"; // ���� ����
    [SerializeField] private float _typingDuration = 5f;      // ���� �ð�(��)

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
                        // ��Ÿ -> ���� ó��
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
        Debug.Log("Ÿ���� ����!");

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingStartEventArgs
        {
            
        });
    }

    private void EndTyping()
    {
        _isTyping = false;
        _typedString = "";
        Debug.Log("Ÿ���� ����!");
    }

    private void CancelTyping()
    {
        Debug.Log("���� �ð� ����� Ÿ���� ���");
        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingStartEventArgs
        {
            answer = _correctAnswer,
            typedString = _typedString
        });
        EndTyping();
    }

    private void OnFailTyping()
    {
        Debug.Log("��Ÿ �߻�! ���� ó��");
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
        Debug.Log("���� ����! ���� ó��");

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
        Debug.Log($"��ȿ �Է� �߻�: {latestChar}, ���� �Է� ����: {currentString}");

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingInputEventArgs
        {
            answer = _correctAnswer,
            typedString = _typedString
        });
    }
}
