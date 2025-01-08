using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Unit _owner;
    private bool _isInitialized;

    [SerializeField] private readonly List<Transform> _equipSlots = new();
    private List<Item> _items = new();

    private int _currentItemSlots = 0;
    private int _maxItemSlots = 6;

    private void OnDisable()
    {
        _isInitialized = false;

        foreach(var item in _items)
        {
            Drop(item);
        }

        _currentItemSlots = 0;
    }

    public void Initialized(Unit unit)
    {
        _owner = unit;
        _isInitialized = true;
    }

    public Item GetItemToEquipType(EquipType type)
    {
        return _items.Find(x => x.Data.EquipType == type);
    }

    public void Equip(ItemData data)
    {
        if (!_isInitialized) return;

        var equalsEquipItem = _items.Find(x=> x.Data.EquipType.Equals(data.EquipType));

        if (!data.EquipType.Equals(EquipType.None) && equalsEquipItem != null)
        {
            Drop(equalsEquipItem);
        }
        else
        {
            if(++_currentItemSlots >= _maxItemSlots)
            {
                _currentItemSlots--;
                return;
            }
        }

        Transform slot = _equipSlots.Find(x => x.name == data.EquipType.ToString()) ?? transform;
        GameObject itemObj = ResourceManager.Instance.Spawn(data.Prefab.gameObject, slot);
        Item item = itemObj.GetComponent<Item>();
        item.Initialized(data);
        _items.Add(item);

        _owner.UpdateStats(data.Id.ToString(), data);
    }

    public void Drop(Item item)
    {
        if (!_isInitialized) return;

        ResourceManager.Instance.Destroy(item.gameObject);
        _items.Remove(item);

        _owner.ResetStats(item.Data.Id.ToString());
    }
}