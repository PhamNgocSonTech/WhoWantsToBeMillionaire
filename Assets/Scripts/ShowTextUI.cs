using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class ShowTextUI : MonoBehaviour
    {
        public TextMeshProUGUI textMeshPro;
        // Start is called before the first frame update
        void Start()
        {
            textMeshPro.text = "Hello World";
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
