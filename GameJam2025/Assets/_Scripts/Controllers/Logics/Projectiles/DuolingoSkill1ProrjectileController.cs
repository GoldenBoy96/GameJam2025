using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DuolingoSkill1ProrjectileController : BaseProjectileController
{
    public Vector3 dir = Vector3.left;
    [SerializeField] string content = string.Empty;
    //[SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] List<GameObject> bulletBaseOnLevelList = new List<GameObject>();


    public TextMeshPro textMest;
    private void Start()
    {
        //textMest = GetComponentInChildren<TextMeshPro>();
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //spriteRenderer.transform.localScale = Vector3.one;
        //textMest.text = content;
        StartCoroutine(WaitToDestroy());
    }
    void Update()
    {
        transform.Translate(dir * 10 * Time.deltaTime);
    }
    IEnumerator WaitToDestroy()
    {
        //TODO: Chinh thoi gian mem trong model
        yield return new WaitForSeconds(20f);
        Destroy(gameObject);
    }

    public void SetLevel(int level)
    {
        foreach(GameObject go in bulletBaseOnLevelList)
        {
            go.SetActive(false);
        }
        bulletBaseOnLevelList[level].gameObject.SetActive(true);
    }
}
