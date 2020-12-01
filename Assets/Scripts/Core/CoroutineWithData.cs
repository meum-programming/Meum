using System.Collections;
using UnityEngine;

/*
 * @brief 코루틴(IEnumerator) 을 사용할 때 결과 값을 받을 수 있도록 해줌
 * @details https://answers.unity.com/questions/24640/how-do-i-return-a-value-from-a-coroutine.html
 * 링크한 웹을 참조함 <br>
 * 사용법은 CoroutineWithData 를 생성한 후 yield return coroutine; 을 통해 코루틴을 끝까지 실행,
 * result 를 알맞은 타입으로 캐스팅
 */
public class CoroutineWithData {
    public Coroutine coroutine { get; }
    public object result;
    private IEnumerator target;
    
    /*
     * @brief CoroutineWithData의 생성자
     * @details owner: 코루틴을 실행할 MonoBehaviour, target: 실행할 코루틴 함수
     */
    public CoroutineWithData(MonoBehaviour owner, IEnumerator target) {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }
 
    private IEnumerator Run() {
        while(target.MoveNext()) {
            result = target.Current;
            yield return result;
        }
    }
}