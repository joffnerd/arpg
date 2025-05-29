using ARPG.Resources;
using ARPG.Scripts.Autoload;
using Godot;
using Godot.Collections;

namespace ARPG.Scripts.Objects;

public partial class Collectable : Area2D
{
    [Export]
    public InventoryItem ItemResource;

    public AudioStream AudioCollect;
    public Dictionary<string, bool> MetaData = [];

    public override void _Ready()
    {
        AudioCollect = ResourceLoader.Load<AudioStream>("res://Audio/Effects/Gold1.wav");
    }

    public void OnAreaEntered(Area2D area)
    {
        var obj = area.GetParent();
        if (obj is not Player.Player)
        {
            return;
        }

        BuildMeta(this);

        var isCollectable = (bool)GetMeta("isCollectable");
        if (isCollectable)
        {
            Collect();
        }
    }

    public virtual void Collect()
    {
        var inventory = SceneManager.Instance.Player.Inventory;

        MetaData.TryGetValue("isWeapon", out bool isWeapon);
        MetaData.TryGetValue("isHealth", out bool isHealth);
        MetaData.TryGetValue("isInvItem", out bool isInvItem);
        MetaData.TryGetValue("isConsumable", out bool isConsumable);

        if (isInvItem)
        {
            inventory.Insert(ItemResource);
        }

        if (isWeapon)
        {
            SceneManager.Instance.Player.ToggleWeapon(true);
        }

        if (!isInvItem && isHealth && isConsumable)
        {
            var amount = ((InventoryHealth)ItemResource).HealthAmount;
            SceneManager.Instance.Player.UpdateHealth(amount);
        }

        PlaySound();

        QueueFree();
    }

    public virtual void PlaySound()
    {
        SceneManager.Instance.Main.Effects.Stream = AudioCollect;
        SceneManager.Instance.Main.Effects.Play();
    }

    public void BuildMeta(Area2D area)
    {
        var meta = area.GetMetaList();
        foreach (var item in meta)
        {
            var val = (bool)area.GetMeta(item.ToString());
            MetaData.Add(item, val);
        }
    }
}
