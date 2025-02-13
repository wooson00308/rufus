using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiShotSkillFxEventData", menuName = "Scriptable Objects/MultiShotSkillFxEventData")]
public class MultiShotSkillFxEventData : SkillFxEventData
{
    [field:SerializeField] public ProjectileData ProjectileData { get; private set; }
    [field:SerializeField] public float HomingDelay { get; private set; }
    [field:SerializeField] public int ProjectileCount { get; private set; }

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.StartCoroutine(MultiShot(owner));
    }

    private IEnumerator MultiShot(Unit owner)
    {
        var targets = owner.Targets;
        int count = ProjectileCount;

        for (int index = 0; index < count; index++)
        {
            yield return null;

            if (targets == null)
            {
                OnFireProjectile(owner);
                continue;
            }

            if (index >= targets.Count && index > 0)
            {
                count -= targets.Count;
                index = 0;
            }

            Unit target = targets[index];

            OnFireProjectile(owner, target);
        }
    }

    private void OnFireProjectile(Unit owner, Unit target = null)
    {
        if (ProjectileData == null) return;

        var projectile = GameFactory.Instance.CreateProjectile(ProjectileData, owner.transform.position);

        // 0 ~ 360도 중 하나의 랜덤 각도 구하기
        float randomAngle = Random.Range(0f, 360f);

        // 랜덤 각도를 Vector2로 변환 (탑다운 2D 기준, Z축 회전)
        Vector2 randomDirection = new(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        );

        // 구한 무작위 방향을 세팅
        projectile.SetDirection(randomDirection);
        projectile.OnFire(owner, ProjectileData);

        // 추후 호밍(목표 추적) 기능을 쓰려면, target이 유효할 때만 코루틴 실행
        if (target != null)
        {
            owner.StartCoroutine(WaitForHomingDelay(projectile, target));
        }
    }

    private IEnumerator WaitForHomingDelay(Projectile projectile, Unit target)
    {
        yield return new WaitForSeconds(HomingDelay);

        if (target == null) yield break;

        projectile.SetTarget(target.transform);
    }
}
