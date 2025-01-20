using UnityEngine;

public class CastingSystem : Singleton<CastingSystem>
{
    // -------------------------------
    // [1] Ÿ���� ���� ����
    // -------------------------------
    [SerializeField] private string _correctAnswer = "HELLO"; // ���� ����
    [SerializeField] private float _typingDuration = 5f;      // ���� �ð�(��)

    // -------------------------------
    // ���������� ����� ������
    // -------------------------------
    private bool _isTyping = false;   // Ÿ���� ��� Ȱ��ȭ ����
    private float _currentDuration = 0f; // ���� ���� ���� �ð�
    private string _typedString = ""; // ������� �Է��� ���ڿ�

    // -------------------------------
    // [2] Update()���� ���� ó��
    // -------------------------------
    public void Update()
    {
        // 1) Ư�� Ű�� Ÿ���� ��� ���� (��: Space)
        if (!_isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            StartTyping();
            return;
        }

        // Ÿ���� ��尡 �ƴϸ� �Է� ó�� �� ��
        if (!_isTyping) return;

        // 2) ���� �ð� üũ
        _currentDuration -= Time.deltaTime;
        if (_currentDuration <= 0f)
        {
            // ���� �ð� ���� -> ��� ó��
            CancelTyping();
            return;
        }

        // 3) Ű���� �Է� ó��
        //    (������ Input������ ���� Enum.GetValues(typeof(KeyCode))�� ��ü Ű�� ��ȸ)
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                // (a) ����(Submit) ó��
                if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
                {
                    // ���� �� (����� ��ġ ����)
                    if (_typedString == _correctAnswer)
                        OnSuccessTyping();
                    else
                        OnFailTyping();
                    return;
                }

                // (b) ���ϴ� ������ ������ ó�� (��: A~Z)
                if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
                {
                    // �Էµ� Ű�� ���ڿ��� ��ȯ (�빮�ڷ� ����)
                    string inputChar = keyCode.ToString(); // "A", "B", ...

                    // �ӽ÷� typedString + �� �Է±��� �ٿ��� �˻�
                    string testString = _typedString + inputChar;

                    // 1) �켱 ������ ���۰� ��ġ�ϴ��� Ȯ�� (��Ÿ���� �ƴ���)
                    //    ��Ÿ�� �ƴ϶�� => typedString�� �Է� Ȯ��
                    if (_correctAnswer.StartsWith(testString))
                    {
                        // typedString ����
                        _typedString = testString;

                        // "��Ÿ�� �ƴ� �Է�"�� ���������� �������Ƿ�,
                        // �� �Է¸��� �̺�Ʈ �߻� (�޼��� ȣ��)
                        OnValidKeyInput(inputChar, _typedString);

                        // 2) ���̰� ����� ��Ȯ�� ��ġ�Ѵٸ� �ڵ� ���� ó��
                        //    (���͸� ��ٸ��� �ʰ� �ϼ��Ǵ� ��� ����)
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

                // (c) ����, Ư������ �� �ٸ� Ű�� ó���ϰ� ������
                //     ���ǹ��� �� �߰��� ó���ϸ� �˴ϴ�.
            }
        }
    }

    // -------------------------------
    // [3] Ÿ���� ��� ����/���� ���� �޼���
    // -------------------------------
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

    // -------------------------------
    // [4] ��Ÿ/���� �̺�Ʈ ó�� �޼����
    // -------------------------------
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

    // -------------------------------
    // [5] ��Ÿ�� �ƴ� �Է�(��ȿ �Է�)�� ���� ������ ȣ��
    // -------------------------------
    private void OnValidKeyInput(string latestChar, string currentString)
    {
        Debug.Log($"��ȿ �Է� �߻�: {latestChar}, ���� �Է� ����: {currentString}");
        // ���⼭ ����Ʈ, ����, UI ������Ʈ �� �ʿ��� ������ �����ø� �˴ϴ�.

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingInputEventArgs
        {
            answer = _correctAnswer,
            typedString = _typedString
        });
    }
}
