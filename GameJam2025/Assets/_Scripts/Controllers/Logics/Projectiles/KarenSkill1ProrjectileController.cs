using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class KarenSkill1ProrjectileController : BaseCharacterController
    {
    public Vector3 dir = Vector3.left;
    [SerializeField] string content = string.Empty;


    public TextMeshPro textMest;
    private void Start()
    {
        textMest = GetComponentInChildren<TextMeshPro>();
        textMest.text = content;    
        StartCoroutine(WaitToDestroy());
    }
    void Update()
    {
        transform.Translate(dir * 10 * Time.deltaTime);
    }
    IEnumerator WaitToDestroy()
    {
        //TODO: Chinh thoi gian mem trong model
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    }
