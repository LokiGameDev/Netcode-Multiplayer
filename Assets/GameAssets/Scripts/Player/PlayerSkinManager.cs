using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

/// <summary>Synchronizes and equips the player's selected skin.</summary>
public class PlayerSkinManager : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> SkinID = new NetworkVariable<FixedString32Bytes>();

    [Header("Skin References")]
    [Tooltip("Database containing the available player skins.")]
    [SerializeField] private SkinDatatbase skinDatatbase;

    [Tooltip("Updates animations for the equipped skin.")]
    [SerializeField] private PlayerAnimationManager playerAnimationManager;
    [Tooltip("Network animator used by the equipped skin.")]
    [SerializeField] private NetworkAnimator networkAnimator;

    [Tooltip("Parent transform for the equipped skin instance.")]
    [SerializeField] private Transform playerSkinRoot;

    /// <summary>Equips the initial skin when the player starts.</summary>
    private void Start()
    {
        EquipNewSkin();
    }

    /// <summary>Sets and equips a skin by its identifier.</summary>
    /// <param name="id">Identifier of the skin to equip.</param>
    public void Initialize(string id)
    {
        if(SkinID.Value == id) return;

        SkinID.Value = id;

        EquipNewSkin();
    }

    /// <summary>Subscribes to synchronized skin changes.</summary>
    public override void OnNetworkSpawn()
    {
        SkinID.OnValueChanged += PlayerSkinIDChanged;
    }

    /// <summary>Unsubscribes from synchronized skin changes.</summary>
    public override void OnNetworkDespawn()
    {
        SkinID.OnValueChanged -= PlayerSkinIDChanged;
    }

    /// <summary>Re-equips the skin when the networked identifier changes.</summary>
    private void PlayerSkinIDChanged(FixedString32Bytes oldID, FixedString32Bytes newID)
    {
        if(SkinID.Value == newID) return;

        SkinID.Value = newID;

        EquipNewSkin();
    }

    /// <summary>Replaces the current skin model with the selected skin.</summary>
    private void EquipNewSkin()
    {
        SkinData currentSkin = skinDatatbase.Get(SkinID.Value.ToString());

        Debug.Log($"{SkinID.Value}: Got {currentSkin.skinID}");

        List<GameObject> skins = new List<GameObject>();

        for(int i = 0;i < playerSkinRoot.childCount; i++)
        {
            if(playerSkinRoot.GetChild(i).CompareTag("PlayerSkin"))
            {
                skins.Add(playerSkinRoot.GetChild(i).gameObject);
            }
        }

        foreach(var skin in skins)
        {
            Destroy(skin);
        }

        var skinAnimator = Instantiate(currentSkin.skinPrefab, playerSkinRoot).GetComponent<Animator>();

        playerAnimationManager.SetAnimator(skinAnimator);

        networkAnimator.Animator = skinAnimator;
    }
}
