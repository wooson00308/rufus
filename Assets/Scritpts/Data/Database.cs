using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Scriptable Objects/Database")]
public class Database : ScriptableObject
{
    [SerializeField] protected List<Data> _datas;

    public virtual T GetDataById<T>(int id) where T : Data
    {
        return _datas.Find(x => x.Id == id) as T;
    }

    public virtual List<T> GetDatas<T>() where T : Data
    {
        return _datas.OfType<T>().ToList();
    }
}
