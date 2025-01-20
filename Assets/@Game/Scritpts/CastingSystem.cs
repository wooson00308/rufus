using UnityEngine;

public class CastingSystem : Singleton<CastingSystem>
{
    // -------------------------------
    // [1] 타이핑 관련 설정
    // -------------------------------
    [SerializeField] private string _correctAnswer = "HELLO"; // 정답 예시
    [SerializeField] private float _typingDuration = 5f;      // 제한 시간(초)

    // -------------------------------
    // 내부적으로 사용할 변수들
    // -------------------------------
    private bool _isTyping = false;   // 타이핑 모드 활성화 여부
    private float _currentDuration = 0f; // 현재 남은 제한 시간
    private string _typedString = ""; // 현재까지 입력한 문자열

    // -------------------------------
    // [2] Update()에서 로직 처리
    // -------------------------------
    public void Update()
    {
        // 1) 특정 키로 타이핑 모드 시작 (예: Space)
        if (!_isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            StartTyping();
            return;
        }

        // 타이핑 모드가 아니면 입력 처리 안 함
        if (!_isTyping) return;

        // 2) 제한 시간 체크
        _currentDuration -= Time.deltaTime;
        if (_currentDuration <= 0f)
        {
            // 제한 시간 만료 -> 취소 처리
            CancelTyping();
            return;
        }

        // 3) 키보드 입력 처리
        //    (구버전 Input에서는 보통 Enum.GetValues(typeof(KeyCode))로 전체 키를 순회)
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                // (a) 엔터(Submit) 처리
                if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
                {
                    // 최종 비교 (정답과 일치 여부)
                    if (_typedString == _correctAnswer)
                        OnSuccessTyping();
                    else
                        OnFailTyping();
                    return;
                }

                // (b) 원하는 문자의 범위만 처리 (예: A~Z)
                if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
                {
                    // 입력된 키를 문자열로 변환 (대문자로 가정)
                    string inputChar = keyCode.ToString(); // "A", "B", ...

                    // 임시로 typedString + 새 입력까지 붙여서 검사
                    string testString = _typedString + inputChar;

                    // 1) 우선 정답의 시작과 일치하는지 확인 (오타인지 아닌지)
                    //    오타가 아니라면 => typedString에 입력 확정
                    if (_correctAnswer.StartsWith(testString))
                    {
                        // typedString 갱신
                        _typedString = testString;

                        // "오타가 아닌 입력"이 성공적으로 들어왔으므로,
                        // 매 입력마다 이벤트 발생 (메서드 호출)
                        OnValidKeyInput(inputChar, _typedString);

                        // 2) 길이가 정답과 정확히 일치한다면 자동 성공 처리
                        //    (엔터를 기다리지 않고도 완성되는 즉시 성공)
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

                // (c) 숫자, 특수문자 등 다른 키를 처리하고 싶으면
                //     조건문을 더 추가해 처리하면 됩니다.
            }
        }
    }

    // -------------------------------
    // [3] 타이핑 모드 시작/종료 관련 메서드
    // -------------------------------
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

    // -------------------------------
    // [4] 오타/성공 이벤트 처리 메서드들
    // -------------------------------
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

    // -------------------------------
    // [5] 오타가 아닌 입력(유효 입력)이 들어올 때마다 호출
    // -------------------------------
    private void OnValidKeyInput(string latestChar, string currentString)
    {
        Debug.Log($"유효 입력 발생: {latestChar}, 현재 입력 누적: {currentString}");
        // 여기서 이펙트, 사운드, UI 업데이트 등 필요한 로직을 넣으시면 됩니다.

        GameEventSystem.Instance.Publish((int)SystemEvents.CasingStart, new CastingInputEventArgs
        {
            answer = _correctAnswer,
            typedString = _typedString
        });
    }
}
