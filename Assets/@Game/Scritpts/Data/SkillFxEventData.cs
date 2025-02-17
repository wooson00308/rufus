public abstract class SkillFxEventData : FxEventData
{
    public override void OnEvent(Unit owner, object args = null)
    {
        var skill = (Skill)args;
        OnSkillEvent(owner, skill);
    }

    public virtual void OnEndEvent(Unit owner, Skill skill)
    {

    }

    public abstract void OnSkillEvent(Unit owner, Skill skill);
}
