using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

    public class KarenSkill2ProjectileController : BaseCharacterController
{
        public Vector3 dir = Vector3.left;
        private void Start()
        {
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
