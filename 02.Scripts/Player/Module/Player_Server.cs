using Unity.Netcode;

public partial class Player
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            GameUI.Instance.Init(this);
            EquipmentCompo.AddItemToInventory(weapon.id);
            _inputReader.GuardEvent += HandleGuardEvent;
        }

        GameManager.Instance.AddPlayer(ownerID, this);

        SubscribeHitEvent();
        SubscribeApplyDamageEvent();
        SetStateDictionary();
        HealthCompo.ChangedHealthEvent += HandleChangedHealthEvent;
        SkillCompo.Init(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner)
        {
            StateMachine.ClearState();
            _inputReader.GuardEvent -= HandleGuardEvent;
        }
    }
    
}