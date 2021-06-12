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
    private Rigidbody2D rb = default;

    bool isAiming = false;
    private Vector2? destination = null;

    private IEtherealEffect fireEffect = null;
    private IEtherealEffect waterEffect = null;
    private IEtherealEffect windEffect = null;
    private IEtherealEffect earthEffect = null;

    private IEtherealEffect selectedEffect = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        canvas = FindObjectOfType<Canvas>();
        movement = GetComponent<Movement>();

        fireEffect = new FireEffect(this, 10f, 1f);
        waterEffect = new FireEffect(this, 10f, 1f);
        windEffect = new FireEffect(this, 10f, 1f);
        earthEffect = new FireEffect(this, 10f, 1f);
    }

    protected virtual void Start()
    {
        ethereal.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (destination.HasValue) { AutomaticallyMoveToDestination(); }
        else { MoveBasedOnInput(); }

        DrawChangeStateUI(isAiming);
        if (selectedEffect != null && Input.GetMouseButtonDown(0)) 
        {
            if (isAiming) { ShootEthereal(); }
            else { GoToEthereal(); }
        }
        
        if (selectedEffect != null && Input.GetMouseButtonDown(1)) 
        {
            if (isAiming) { DropEthereal(); }
            else { PullEthereal();  }
        }

        if (!ethereal.IsActive() && Input.GetKeyDown(KeyCode.Alpha1))
        { 
            selectedEffect = fireEffect;
            isAiming = true;
        }

        if (!ethereal.IsActive() && Input.GetKeyDown(KeyCode.Alpha2)) 
        { 
            selectedEffect = waterEffect;
            isAiming = true;
        }

        if (!ethereal.IsActive() && Input.GetKeyDown(KeyCode.Alpha3))
        { 
            selectedEffect = windEffect;
            isAiming = true;
        }

        if (!ethereal.IsActive() && Input.GetKeyDown(KeyCode.Alpha4)) 
        { 
            selectedEffect = earthEffect;
            isAiming = true;
        }
    }

    private void AutomaticallyMoveToDestination()
    {
        if (Vector2.Distance((Vector2)transform.position, destination.Value) < 0.5f)
        {
            Debug.Log("arrived at destination");
            destination = null;
            rb.velocity = Vector2.zero;
            movement.MoveSpeed = 10f;
            ethereal.gameObject.SetActive(false);
            return;
        }

        Vector2 direction = destination.Value - (Vector2)transform.position;
        movement.Move(direction.normalized);
    }

    private void MoveBasedOnInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            y = 1;
        }

        input = new Vector2(x, y);
        movement.Move(input.x, input.y == 1);
    }

    private void ShootEthereal()
    {
        ethereal.Shoot(this, selectedEffect);
        isAiming = false;
    }

    private void DropEthereal()
    {
        ethereal.Drop(this, selectedEffect);
        isAiming = false;
    }

    private void PullEthereal()
    {
        ethereal.Pull(this);
    }

    private void GoToEthereal()
    {
        destination = (Vector2)ethereal.transform.position;
        movement.MoveSpeed = 30f;
        ethereal.Goto(this);
    }

    private void DrawChangeStateUI(bool _active)
    {
        foreach (IRangeIndicator indicator in indicators)
        {
            indicator.Radius = 5f;
            indicator.SetActive(_active);
        }
    }    
}
