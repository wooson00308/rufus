using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemData _data;
    public ItemData Data => _data;

    public void Initialized(ItemData data)
    {
        _data = data;
    }
}
