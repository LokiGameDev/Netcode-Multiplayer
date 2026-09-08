using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerSkinManager : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> SkinID = new NetworkVariable<FixedString32Bytes>();

    [SerializeField] private SkinDatatbase skinDatatbase;

    [SerializeField] private PlayerAnimationManager playerAnimationManager;
    [SerializeField] private NetworkAnimator networkAnimator;

    [SerializeField] private Transform playerSkinRoot;

    private void Start()
    {
        EquipNewSkin();
    }

    public void Initialize(string id)
    {
        if(SkinID.Value == id) return;

        SkinID.Value = id;

        EquipNewSkin();
    }

    public override void OnNetworkSpawn()
    {
        SkinID.OnValueChanged += PlayerSkinIDChanged;
    }

    public override void OnNetworkDespawn()
    {
        SkinID.OnValueChanged -= PlayerSkinIDChanged;
    }

    private void PlayerSkinIDChanged(FixedString32Bytes oldID, FixedString32Bytes newID)
    {
        if(SkinID.Value == newID) return;

        SkinID.Value = newID;

        EquipNewSkin();
    }

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
