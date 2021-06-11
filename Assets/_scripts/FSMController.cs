using Elysium.Utils.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FSMController : MonoBehaviour
{
    [SerializeField, ReadOnly] private Vector2 input = Vector2.zero;
    [SerializeField] private Ethereal ethereal;
    [RequireInterface(typeof(IRangeIndicator))]
    [SerializeField] private UnityEngine.Object[] indicators = new UnityEngine.Object[0];        

    private Canvas canvas = default;
    private Movement movement = default;

    bool isRequestingShoot = false;
    

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        movement = GetComponent<Movement>();
    }

    protected virtual void Start()
    {
        ethereal.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            y = 1;
        }

        input = new Vector2(x, y);
        if (input != Vector2.zero) { movement.Move(input.x, input.y == 1); }

        DrawChangeStateUI(isRequestingShoot);

        if (isRequestingShoot && Input.GetMouseButtonDown(0))
        {            
            ShootEthereal();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            RequestToShootEthereal();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropEthereal();
        }
    }

    private void DrawChangeStateUI(bool _active)
    {
        foreach (IRangeIndicator indicator in indicators)
        {
            indicator.Radius = 5f;
            indicator.SetActive(_active);
        }
    }

    private void RequestToShootEthereal()
    {
        isRequestingShoot = !isRequestingShoot;
    }    

    private void ShootEthereal()
    {
        var worldPos = GetCurrentMousePosition();
        isRequestingShoot = false;

        DropEthereal();
        ethereal.MoveToPosition(new Vector3(worldPos.x, worldPos.y, 0));
    }

    private void DropEthereal()
    {
        ethereal.gameObject.SetActive(true);
        ethereal.transform.position = transform.position;
    }

    private Vector2 GetCurrentMousePosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        var worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 10.0f));
        return worldPosition;
    }
}
