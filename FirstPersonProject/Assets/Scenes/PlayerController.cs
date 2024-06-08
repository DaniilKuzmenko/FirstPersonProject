using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float hitScaleSpeed = 15f;
    public const string EQUIPE_NOT_SELECTED_TEXT = "EquipeNotSelected";
    private const float gravityScale = 9.8f, speedScale = 5f, jumpForce = 8f, turnSpeed = 90f;
    private float verticalSpeed, mouseX, mouseY, currentCameraAngleX;
    private int inversion = -1;
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private GameObject playerCamera;

    [SerializeField]
    private GameObject particleBlockObject;
    private GameObject currentEquipedItem;
    
    private float hitLastTime;

    [HideInInspector]
    public List<ItemData> inventoryItems, currentChestItems;
    private bool canMove = true;
    [HideInInspector]
    public string itemYouCanEquipeName = EQUIPE_NOT_SELECTED_TEXT;
    [SerializeField]
    private GameObject[] equipableItems;
 

    private RaycastHit hit;

    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
    }
                                        

    private void Start()
    {
        inventoryItems = new List<ItemData>();
        currentChestItems = new List<ItemData>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InventoryManager.instance.CreateItem(0, inventoryItems);
        EquipItem("Pickaxe");
    }

    

    private void FixedUpdate()
    {
        if (canMove)
        {
        Rotate();
        }
    }

    private void Update()
    {
        if (canMove)
        {
            Move();
            
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
            {
                if (Input.GetMouseButton(0))
                {
                    ObjectInteraction(hit.transform.gameObject);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    ItemAbility();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && !InventoryManager.instance.GetInventoryPanel().activeSelf)
        {
            OpenInventory();
        }
        else if (Input.GetKeyDown (KeyCode.Q)) 
        {
            CloseInventoryPanel();
        }
        else if (Input.GetKeyDown(KeyCode.E) && InventoryManager.instance.GetInventoryPanel().activeSelf && itemYouCanEquipeName != EQUIPE_NOT_SELECTED_TEXT)
        {
            EquipItem(itemYouCanEquipeName);
        }
    }
    private void Rotate()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(new Vector3(0f, mouseX * turnSpeed * Time.fixedDeltaTime, 0f));

        currentCameraAngleX += mouseY * Time.fixedDeltaTime * turnSpeed * inversion;
        currentCameraAngleX = Mathf.Clamp(currentCameraAngleX, -60f, 60f);
        playerCamera.transform.localEulerAngles = new Vector3(currentCameraAngleX, 0f, 0f);
    }

    private void Move()
    {
        Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        velocity = transform.TransformDirection(velocity) * speedScale;
        if (characterController.isGrounded)
        {
            verticalSpeed = 0f;
            if(Input.GetKeyDown(KeyCode.Space))
            {
                verticalSpeed = jumpForce;
            }
        }
        verticalSpeed -= gravityScale * Time.deltaTime;
        velocity.y = verticalSpeed;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Dig(Block block)
    {
        if (Time.time - hitLastTime > 1 / hitScaleSpeed)
        {
            currentEquipedItem.GetComponent<Animator>().SetTrigger("attack");
            hitLastTime = Time.time;

            Tool currentToolInfo = null;
            if(currentEquipedItem.TryGetComponent<Tool>(out currentToolInfo))
            {
                block.health -= currentToolInfo.damageToBlock;
            }
            else
            {
                block.health -= 1;
            }

            
            GameObject particleObject = Instantiate(particleBlockObject, block.transform.position, Quaternion.identity);
            particleObject.GetComponent<ParticleSystemRenderer>().material = block.GetComponent<MeshRenderer>().material;

            if (block.health <= 0) 
            {
                block.DestroyBehavior();
            }
        }
    }
    private void ObjectInteraction(GameObject currentObj)
    {
        switch (currentObj.tag)
        {
            case "Block":
                if (Vector3.Distance(transform.position, currentObj.transform.position) < 5f)
                {
                    Dig(currentObj.GetComponent<Block>());
                }    
                break;
            case "Enemy":
                Attack(currentObj);
                break;
            case "Chest":
                if (Vector3.Distance(transform.position, currentObj.transform.position) < 5f)
                {
                    currentChestItems = currentObj.GetComponent<Chest>().chestItems;
                    OpenChest();
                }    
                break;        
        }
    }

    private void Attack(GameObject currentObj)
    {
        if (Time.time - hitLastTime > 1 / hitScaleSpeed)
        {
            hitLastTime = Time.time;
            var enemy = currentObj.GetComponent<Enemy>();
            enemy.TakeDamage(currentEquipedItem.GetComponent<Tool>().damageToEnemy);
        }
    }

    private void OpenInventory()
    {
        if (!InventoryManager.instance.GetInventoryPanel().activeSelf) 
        {
            

            InventoryManager.instance.GetInventoryPanel().SetActive(true);
            if (inventoryItems.Count > 0)
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    InventoryManager.instance.InstantiateItem(inventoryItems[i], InventoryManager.instance.GetInventoryContent().transform, InventoryManager.instance.inventorySlots);
                }
            }
        }
    }

    private void OpenChest()
    {

        if (!InventoryManager.instance.GetChestPanel().activeSelf)
        {
            SwitchCursor(true, CursorLockMode.Confined);
            InventoryManager.instance.GetChestPanel().SetActive(true);
            for (int i = 0; i < currentChestItems.Count; i++)
            {
                InventoryManager.instance.InstantiateItem(currentChestItems[i], InventoryManager.instance.GetChestContent(). transform, InventoryManager.instance.currentChestSlots);

            }
        }
    }

    private void SwitchCursor(bool active, CursorLockMode lockMode) 
    {
        Cursor.visible = true;
        Cursor.lockState = lockMode;
        canMove = !active;
    }

    private void CloseInventoryPanel()
    {
        var inventoryManager = InventoryManager.instance;
        SwitchCursor(false, CursorLockMode.Locked);
        foreach (GameObject slot in inventoryManager.currentChestSlots)
        {
            Destroy(slot);
        }
        foreach (GameObject slot in inventoryManager.inventorySlots)
        {
            Destroy(slot);
        }
        inventoryManager.currentChestSlots.Clear();
        inventoryManager.inventorySlots.Clear();
        inventoryManager.GetChestPanel().SetActive(false);
        inventoryManager.GetInventoryPanel().SetActive(false);
    }

    private void EquipItem (string toolName)
    {
        foreach (GameObject tool in equipableItems)
        {
            if (toolName == tool.name)
            {
                tool.SetActive(true);
                currentEquipedItem = tool;
                toolName = EQUIPE_NOT_SELECTED_TEXT;
            }
            else
            {
                tool.SetActive(false);
            }
        }
    }

    private void ItemAbility()
    {
        switch (currentEquipedItem.name)
        {
            case "Ground":
                CreateBlock();
                break;
            case "Meat":
                break;
            default:
                break;        
        }
    }

    private void CreateBlock()
    {
        GameObject blockPref = Resources.Load<GameObject>("Ground");
        Vector3 temPos = hit.transform.gameObject.transform.position;
        Vector3 newBlockPos = Vector3.zero;
        if (hit.transform.gameObject.tag == "Block")
        {
            GameObject currentBlock = Instantiate(blockPref);
            if (hit.point.y == temPos.y + 0.5f)
            {
                newBlockPos = new Vector3(temPos.x, temPos.y + 1, temPos.z);
            }
            else if (hit.point.y == temPos.y - 0.5f)
            {
                newBlockPos = new Vector3(temPos.x, temPos.y - 1, temPos.z);
            }

            if (hit.point.x == temPos.x + 0.5f)
            {
                newBlockPos = new Vector3(temPos.x + 1, temPos.y, temPos.z);
            }
            else if (hit.point.y == temPos.y - 0.5f)
            {
                newBlockPos = new Vector3(temPos.x - 1, temPos.y, temPos.z);
            }

            if (hit.point.y == temPos.y + 0.5f)
            {
                newBlockPos = new Vector3(temPos.x, temPos.y, temPos.z + 1);
            }
            else if (hit.point.y == temPos.y - 0.5f)
            {
                newBlockPos = new Vector3(temPos.x, temPos.y, temPos.z - 1);
            }
            currentBlock.transform.position = newBlockPos;
            currentBlock.transform.SetParent(hit.transform.gameObject.transform.parent);
            ModifyItemCount("Ground");
        }
    }

    private void ModifyItemCount(string itemName)
    {
        foreach(ItemData item in inventoryItems)
        {
            if(item.name == itemName)
            {
                item.count--;
                if(item.count <= 0)
                {
                    inventoryItems.Remove(item);
                    EquipItem(inventoryItems[0].name);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name.StartsWith("mini"))
        {
            InventoryManager.instance.CreateItem(2, inventoryItems);
            Destroy(col.gameObject);
        }
    }
}
