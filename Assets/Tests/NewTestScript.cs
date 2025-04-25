using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
public class InventoryTests
{
    public Inventory inventory;
    public GameObject inventoryObject;
    public PlayerData playerData;

    [SetUp]
    public void SetUp()
    {
        // 테스트용 Inventory 오브젝트 생성
        inventoryObject = new GameObject("Inventory");
        inventory = inventoryObject.AddComponent<Inventory>();

        // PlayerData 설정
        GameObject playerDataObject = new GameObject("PlayerData");
        playerData = playerDataObject.AddComponent<PlayerData>();
        playerData.items = new List<int> { 1, 2, 3 }; // 테스트용 아이템 ID 추가
        inventoryObject.tag = "PlayerData";

        // ItemHoldParent 설정
        GameObject itemHoldParent = new GameObject("ItemHoldParent");
        inventory.itemHoldParent = itemHoldParent.transform;

        // 아이템 슬롯 생성
        for (int i = 0; i < 3; i++)
        {
            GameObject slot = new GameObject($"Slot{i}");
            slot.transform.SetParent(itemHoldParent.transform);
            GameObject icon = new GameObject("Icon");
            icon.AddComponent<Image>();
            icon.transform.SetParent(slot.transform);
        }

        inventory.Awake();
        inventory.Start();
    }

    [Test]
    public void TestUpdateInventory()
    {
        // 인벤토리 업데이트
        inventory.UpdateInventory();

        // 아이템이 올바르게 업데이트되었는지 확인
        Assert.AreEqual(3, inventory.itemHoldList.Count, "아이템 슬롯 개수가 올바르지 않습니다.");
        Assert.AreEqual(1, inventory.itemHoldList[0].itemID, "첫 번째 아이템 ID가 올바르지 않습니다.");
        Assert.AreEqual(2, inventory.itemHoldList[1].itemID, "두 번째 아이템 ID가 올바르지 않습니다.");
        Assert.AreEqual(3, inventory.itemHoldList[2].itemID, "세 번째 아이템 ID가 올바르지 않습니다.");
    }

    [Test]
    public void TestClickToHoldItem()
    {
        // 특정 아이템 선택
        inventory.ClickToHoldItem(1);

        // 선택된 아이템 인덱스 확인
        Assert.AreEqual(1, inventory.currentHoldingItemIndex, "선택된 아이템 인덱스가 올바르지 않습니다.");
    }

    [Test]
    public void TestCheckItemIsInInventory()
    {
        // 인벤토리에 아이템이 존재하는지 확인
        bool itemExists = inventory.CheckItemIsInInventory(2);
        Assert.IsTrue(itemExists, "아이템이 인벤토리에 존재하지 않습니다.");

        // 존재하지 않는 아이템 확인
        itemExists = inventory.CheckItemIsInInventory(99);
        Assert.IsFalse(itemExists, "존재하지 않는 아이템이 인벤토리에 있다고 표시됩니다.");
    }

    [TearDown]
    public void TearDown()
    {
        // 테스트 후 오브젝트 정리
        Object.DestroyImmediate(inventoryObject);
        Object.DestroyImmediate(playerData.gameObject);
    }
}