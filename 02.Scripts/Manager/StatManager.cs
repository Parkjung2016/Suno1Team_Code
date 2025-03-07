using System.Collections.Generic;

public class StatManager : NetMonoSingleton<StatManager>
{
    private Player _owner;
    private List<Stat> _currentStats = new();

    public void Init(Player owner)
    {
        _owner = owner;
    }

    public void AddStat(Stat stat)
    {
        _currentStats.Add(stat);
    }
}