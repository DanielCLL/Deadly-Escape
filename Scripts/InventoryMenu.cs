using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    public GameObject inventoryUI;                 // Canvas del inventario
    public MonoBehaviour[] scriptsToDisable;       // Scripts como movimiento y cámara

    private bool isInventoryOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isInventoryOpen)
                CloseInventory();
            else
                OpenInventory();
        }
    }

    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
        Time.timeScale = 0f;
        ToggleScripts(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isInventoryOpen = true;
    }

    public void CloseInventory()
    {
        inventoryUI.SetActive(false);
        Time.timeScale = 1f;
        ToggleScripts(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isInventoryOpen = false;
    }

    private void ToggleScripts(bool enabled)
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            script.enabled = enabled;
        }
    }
}
